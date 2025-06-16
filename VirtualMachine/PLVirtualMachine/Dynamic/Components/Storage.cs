using System;
using System.Xml;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMStorage))]
  public class Storage : VMStorage, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Storage);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "AddItemEvent":
          AddItemEvent += (p1, p2) => target.RaiseFromEngineImpl(p1, p2);
          break;
        case "RemoveItemEvent":
          RemoveItemEvent += (p1, p2) => target.RaiseFromEngineImpl(p1, p2);
          break;
        case "ChangeItemEvent":
          ChangeItemEvent += (p1, p2) => target.RaiseFromEngineImpl(p1, p2);
          break;
      }
    }

    public override void Initialize(VMBaseEntity parent)
    {
      if (CrowdStorageCommandProcessor == null)
      {
        CrowdStorageCommandProcessor = new CrowdStorageCommandProcessor();
        AssyncProcessManager.RegistrAssyncUpdateableObject(CrowdStorageCommandProcessor);
      }
      base.Initialize(parent);
    }

    public override void Initialize(VMBaseEntity parent, IComponent engComponent)
    {
      if (CrowdStorageCommandProcessor == null)
      {
        CrowdStorageCommandProcessor = new CrowdStorageCommandProcessor();
        AssyncProcessManager.RegistrAssyncUpdateableObject(CrowdStorageCommandProcessor);
      }
      base.Initialize(parent, engComponent);
    }

    public static void ResetInstance()
    {
      CrowdStorageCommandProcessor = null;
    }

    public override void PickUpCombination(IBlueprintRef combinationObject)
    {
      DoPickUpCombinationByTemplate(combinationObject);
    }

    public override void PickUpCombinationWithDrop(IBlueprintRef combinationObject, bool dropIfBusy)
    {
      DoPickUpCombinationByTemplate(combinationObject, dropIfBusy);
    }

    public override void PickUpCombinationToInentoryByTemplate(
      IBlueprintRef combinationObject,
      IBlueprintRef containerTemplate)
    {
      DoPickUpCombinationToInventoryByTemplate(combinationObject, containerTemplate);
    }

    public override void PickUpCombinationToInentoryByTemplateWithDrop(
      IBlueprintRef combinationObject,
      IBlueprintRef containerTemplate,
      bool dropIfBusy)
    {
      DoPickUpCombinationToInventoryByTemplate(combinationObject, containerTemplate, dropIfBusy);
    }

    public override void PickUpToInentoryByTemplate(
      [Template] IEntity template,
      [Template] IEntity containerTemplate,
      int thingsCount)
    {
      try
      {
        if (template == null)
          Logger.AddError("PickUp thing entity is null !");
        else if (containerTemplate == null)
        {
          Logger.AddError("PickUp target container not defined !");
        }
        else
        {
          IInventoryComponent containerByTemplate = GetContainerByTemplate(containerTemplate);
          if (containerByTemplate == null)
            Logger.AddError(string.Format("PickUp target container name={0} id ={1} not found !", containerTemplate.Name, containerTemplate.Id));
          else if (((VMEntity) Parent).IsCrowdItem)
          {
            CrowdStorageCommand crowdStorageCommand = new CrowdStorageCommand();
            crowdStorageCommand.Initialize(EStorageCommandType.StorageCommandTypeAddItem, this, containerByTemplate.Owner, template, thingsCount);
            CrowdStorageCommandProcessor.MakeStorageCommand(crowdStorageCommand, true);
          }
          else
          {
            TestSimplePickUpMode = true;
            int num = 0;
            do
            {
              int iNeedCount = thingsCount - num;
              int inventoryByTemplate = DoAddItemsToInventoryByTemplate(containerByTemplate.Owner, template, iNeedCount);
              if (inventoryByTemplate != 0)
                num += inventoryByTemplate;
              else
                break;
            }
            while (num < thingsCount);
            TestSimplePickUpMode = false;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cannot pick up items {0} to {1} storage: {2}", template.Name, Parent.Name, ex));
      }
    }

    public int DoAddItemsToInventoryByTemplate(
      IEntity targetContainer,
      IEntity template,
      int iNeedCount)
    {
      IStorableComponent storableComponent = VMStorable.MakeStorableByTemplate(Parent, template);
      IEntity owner = storableComponent.Owner;
      Parent.Instance.GetComponent<IStorageComponent>();
      IStorableComponent component = owner.GetComponent<IStorableComponent>();
      int inventoryByTemplate = component.Max;
      if (inventoryByTemplate > iNeedCount)
        inventoryByTemplate = iNeedCount;
      component.Count = inventoryByTemplate;
      if (DoAddItemToStorage(Component, component, targetContainer.GetComponent<IInventoryComponent>(), true))
      {
        OnModify();
        return inventoryByTemplate;
      }
      storableComponent.Owner.Dispose();
      return 0;
    }

    public override void StateSave(IDataWriter writer)
    {
      base.StateSave(writer);
      SaveManagerUtility.SaveList(writer, "InnerContainerTags", innerContainerTags);
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        if (xmlNode.ChildNodes[i].Name == "InnerContainerTags")
          VMSaveLoadManager.LoadList((XmlElement) xmlNode.ChildNodes[i], innerContainerTags);
      }
    }

    private void DoPickUpCombinationByTemplate(IBlueprintRef combinationObject, bool dropIfBusy = true)
    {
      if (combinationObject == null)
      {
        Logger.AddError(string.Format("Combination object for adding items to storage {0} not defined", Parent.Name));
      }
      else
      {
        IBlueprint blueprint = combinationObject.Blueprint;
        if (blueprint != null)
        {
          if (blueprint.IsFunctionalSupport("Combination"))
          {
            IParam property = blueprint.GetProperty("Combination", EngineAPIManager.GetSpecialPropertyName(ESpecialPropertyName.SPN_COMBINATION_DATA, typeof (VMCombination)));
            if (property != null)
            {
              if (property.Value != null)
              {
                if (typeof (ObjectCombinationDataStruct) == property.Value.GetType())
                {
                  ObjectCombinationDataStruct combinationData = (ObjectCombinationDataStruct) property.Value;
                  if (combinationData.GetElementsCount() > 0)
                    ((GlobalStorageManager) VMGlobalStorageManager.Instance).AddCombinationToStorage(combinationData, this, null, null, dropIfBusy);
                  OnModify();
                }
                else
                  Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component ObjectCombinationDataStruct param value is invalid", blueprint.Name, Parent.Name));
              }
              else
                Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component not contains ObjectCombinationDataStruct param value", blueprint.Name, Parent.Name));
            }
            else
              Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component not contains ObjectCombinationDataStruct param", blueprint.Name, Parent.Name));
          }
          else
            Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} not contains Combination component", blueprint.Name, Parent.Name));
        }
        else
          Logger.AddError(string.Format("Combination object with guid={0} for adding items to storage {1} not found", combinationObject.Name, Parent.Name));
      }
    }

    public override bool CheckExistCombinationElement(IBlueprintRef combinationTemplate)
    {
      IBlueprint blueprint = null;
      if (combinationTemplate != null && combinationTemplate.Blueprint != null)
        blueprint = combinationTemplate.Blueprint;
      if (blueprint == null)
      {
        Logger.AddError(string.Format("Combination for checking element containing in storage {0} not defined at {1} !", Parent.Name, DynamicFSM.CurrentStateInfo));
        return false;
      }
      if (blueprint.IsFunctionalSupport("Combination"))
      {
        IParam property = blueprint.GetProperty("Combination", EngineAPIManager.GetSpecialPropertyName(ESpecialPropertyName.SPN_COMBINATION_DATA, typeof (VMCombination)));
        if (property != null)
        {
          if (property.Value != null)
          {
            if (typeof (ObjectCombinationDataStruct) == property.Value.GetType())
            {
              ObjectCombinationDataStruct combinationDataStruct = (ObjectCombinationDataStruct) property.Value;
              if (combinationDataStruct.GetElementsCount() > 0)
              {
                foreach (IStorableComponent storableComponent in Component.Items)
                {
                  if (storableComponent != null && !storableComponent.IsDisposed)
                  {
                    Guid templateId = storableComponent.Owner.TemplateId;
                    IWorldBlueprint engineTemplateByGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(templateId);
                    if (engineTemplateByGuid == null)
                      Logger.AddError(string.Format("Combination element checking in storage {0} error: template {1} id = {2} not registered in vm", Parent.Name, storableComponent.Owner.Name, templateId));
                    else if (combinationDataStruct.ContainsItem(engineTemplateByGuid))
                      return true;
                  }
                }
              }
            }
            else
              Logger.AddError(string.Format("Combination object {0} for checking element containing in storage {1}  not contains Combination data at", blueprint.Name, Parent.Name, DynamicFSM.CurrentStateInfo));
          }
          else
            Logger.AddError(string.Format("Combination object {0} for checking element containing in storage {1}  not contains Combination data at", blueprint.Name, Parent.Name, DynamicFSM.CurrentStateInfo));
        }
        else
          Logger.AddError(string.Format("Combination object {0} for checking element containing in storage {1}  not contains ObjectCombinationDataStruct param", blueprint.Name, Parent.Name, DynamicFSM.CurrentStateInfo));
      }
      else
        Logger.AddError(string.Format("Combination object {0} for checking element containing in storage {1}  not contains Combination component at {2}", blueprint.Name, Parent.Name, DynamicFSM.CurrentStateInfo));
      return false;
    }

    private void DoPickUpCombinationToInventoryByTemplate(
      IBlueprintRef combinationObject,
      IBlueprintRef containerTemplate,
      bool dropIfBusy = true)
    {
      if (combinationObject == null)
      {
        Logger.AddError(string.Format("Combination object for adding items to storage {0} not defined", Parent.Name));
      }
      else
      {
        if (((VMEntity) Parent).IsCrowdItem)
          GlobalStorageManager.AssyncStorageGroupCommandMode = true;
        IBlueprint blueprint = combinationObject.Blueprint;
        if (blueprint != null)
        {
          if (blueprint.IsFunctionalSupport("Combination"))
          {
            IParam property = blueprint.GetProperty("Combination", EngineAPIManager.GetSpecialPropertyName(ESpecialPropertyName.SPN_COMBINATION_DATA, typeof (VMCombination)));
            if (property != null)
            {
              if (property.Value != null)
              {
                if (typeof (ObjectCombinationDataStruct) == property.Value.GetType())
                {
                  ObjectCombinationDataStruct combinationData = (ObjectCombinationDataStruct) property.Value;
                  if (combinationData.GetElementsCount() > 0)
                  {
                    OperationTagsInfo containerTypes = new OperationTagsInfo();
                    if (containerTemplate != null && containerTemplate.Blueprint != null)
                      containerTypes.AddTag(containerTemplate.Blueprint.Name);
                    ((GlobalStorageManager) VMGlobalStorageManager.Instance).AddCombinationToStorage(combinationData, this, containerTypes, null, dropIfBusy);
                    OnModify();
                  }
                }
                else
                  Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component ObjectCombinationDataStruct param value is invalid", blueprint.Name, Parent.Name));
              }
              else
                Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component not contains ObjectCombinationDataStruct param value", blueprint.Name, Parent.Name));
            }
            else
              Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component not contains ObjectCombinationDataStruct param", blueprint.Name, Parent.Name));
          }
          else
            Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} not contains Combination component", blueprint.Name, Parent.Name));
        }
        else
          Logger.AddError(string.Format("Combination object with guid={0} for adding items to storage {1} not found", combinationObject.Name, Parent.Name));
        if (!((VMEntity) Parent).IsCrowdItem)
          return;
        GlobalStorageManager.AssyncStorageGroupCommandMode = false;
      }
    }

    protected static CrowdStorageCommandProcessor CrowdStorageCommandProcessor { get; private set; }
  }
}
