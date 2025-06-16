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
using System;
using System.Xml;

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
          this.AddItemEvent += (VMStorage.ItemInventoryActionEventType) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
          break;
        case "RemoveItemEvent":
          this.RemoveItemEvent += (VMStorage.ItemInventoryActionEventType) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
          break;
        case "ChangeItemEvent":
          this.ChangeItemEvent += (VMStorage.ItemInventoryActionEventType) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
          break;
      }
    }

    public override void Initialize(VMBaseEntity parent)
    {
      if (Storage.CrowdStorageCommandProcessor == null)
      {
        Storage.CrowdStorageCommandProcessor = new CrowdStorageCommandProcessor();
        AssyncProcessManager.RegistrAssyncUpdateableObject((IAssyncUpdateable) Storage.CrowdStorageCommandProcessor);
      }
      base.Initialize(parent);
    }

    public override void Initialize(VMBaseEntity parent, IComponent engComponent)
    {
      if (Storage.CrowdStorageCommandProcessor == null)
      {
        Storage.CrowdStorageCommandProcessor = new CrowdStorageCommandProcessor();
        AssyncProcessManager.RegistrAssyncUpdateableObject((IAssyncUpdateable) Storage.CrowdStorageCommandProcessor);
      }
      base.Initialize(parent, engComponent);
    }

    public static void ResetInstance()
    {
      Storage.CrowdStorageCommandProcessor = (CrowdStorageCommandProcessor) null;
    }

    public override void PickUpCombination(IBlueprintRef combinationObject)
    {
      this.DoPickUpCombinationByTemplate(combinationObject);
    }

    public override void PickUpCombinationWithDrop(IBlueprintRef combinationObject, bool dropIfBusy)
    {
      this.DoPickUpCombinationByTemplate(combinationObject, dropIfBusy);
    }

    public override void PickUpCombinationToInentoryByTemplate(
      IBlueprintRef combinationObject,
      IBlueprintRef containerTemplate)
    {
      this.DoPickUpCombinationToInventoryByTemplate(combinationObject, containerTemplate);
    }

    public override void PickUpCombinationToInentoryByTemplateWithDrop(
      IBlueprintRef combinationObject,
      IBlueprintRef containerTemplate,
      bool dropIfBusy)
    {
      this.DoPickUpCombinationToInventoryByTemplate(combinationObject, containerTemplate, dropIfBusy);
    }

    public override void PickUpToInentoryByTemplate(
      [Template] IEntity template,
      [Template] IEntity containerTemplate,
      int thingsCount)
    {
      try
      {
        if (template == null)
          Logger.AddError(string.Format("PickUp thing entity is null !"));
        else if (containerTemplate == null)
        {
          Logger.AddError(string.Format("PickUp target container not defined !"));
        }
        else
        {
          IInventoryComponent containerByTemplate = this.GetContainerByTemplate(containerTemplate);
          if (containerByTemplate == null)
            Logger.AddError(string.Format("PickUp target container name={0} id ={1} not found !", (object) containerTemplate.Name, (object) containerTemplate.Id));
          else if (((VMEntity) this.Parent).IsCrowdItem)
          {
            CrowdStorageCommand crowdStorageCommand = new CrowdStorageCommand();
            crowdStorageCommand.Initialize(EStorageCommandType.StorageCommandTypeAddItem, (VMStorage) this, containerByTemplate.Owner, template, thingsCount);
            Storage.CrowdStorageCommandProcessor.MakeStorageCommand((IStorageCommand) crowdStorageCommand, true);
          }
          else
          {
            VMStorage.TestSimplePickUpMode = true;
            int num = 0;
            do
            {
              int iNeedCount = thingsCount - num;
              int inventoryByTemplate = this.DoAddItemsToInventoryByTemplate(containerByTemplate.Owner, template, iNeedCount);
              if (inventoryByTemplate != 0)
                num += inventoryByTemplate;
              else
                break;
            }
            while (num < thingsCount);
            VMStorage.TestSimplePickUpMode = false;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cannot pick up items {0} to {1} storage: {2}", (object) template.Name, (object) this.Parent.Name, (object) ex));
      }
    }

    public int DoAddItemsToInventoryByTemplate(
      IEntity targetContainer,
      IEntity template,
      int iNeedCount)
    {
      IStorableComponent storableComponent = VMStorable.MakeStorableByTemplate(this.Parent, template);
      IEntity owner = storableComponent.Owner;
      this.Parent.Instance.GetComponent<IStorageComponent>();
      IStorableComponent component = owner.GetComponent<IStorableComponent>();
      int inventoryByTemplate = component.Max;
      if (inventoryByTemplate > iNeedCount)
        inventoryByTemplate = iNeedCount;
      component.Count = inventoryByTemplate;
      if (VMStorage.DoAddItemToStorage(this.Component, component, targetContainer.GetComponent<IInventoryComponent>(), true))
      {
        this.OnModify();
        return inventoryByTemplate;
      }
      storableComponent.Owner.Dispose();
      return 0;
    }

    public override void StateSave(IDataWriter writer)
    {
      base.StateSave(writer);
      SaveManagerUtility.SaveList(writer, "InnerContainerTags", this.innerContainerTags);
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        if (xmlNode.ChildNodes[i].Name == "InnerContainerTags")
          VMSaveLoadManager.LoadList((XmlElement) xmlNode.ChildNodes[i], this.innerContainerTags);
      }
    }

    private void DoPickUpCombinationByTemplate(IBlueprintRef combinationObject, bool dropIfBusy = true)
    {
      if (combinationObject == null)
      {
        Logger.AddError(string.Format("Combination object for adding items to storage {0} not defined", (object) this.Parent.Name));
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
                    ((GlobalStorageManager) VMGlobalStorageManager.Instance).AddCombinationToStorage(combinationData, (VMStorage) this, (OperationTagsInfo) null, (OperationMultiTagsInfo) null, dropIfBusy);
                  this.OnModify();
                }
                else
                  Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component ObjectCombinationDataStruct param value is invalid", (object) blueprint.Name, (object) this.Parent.Name));
              }
              else
                Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component not contains ObjectCombinationDataStruct param value", (object) blueprint.Name, (object) this.Parent.Name));
            }
            else
              Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component not contains ObjectCombinationDataStruct param", (object) blueprint.Name, (object) this.Parent.Name));
          }
          else
            Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} not contains Combination component", (object) blueprint.Name, (object) this.Parent.Name));
        }
        else
          Logger.AddError(string.Format("Combination object with guid={0} for adding items to storage {1} not found", (object) combinationObject.Name, (object) this.Parent.Name));
      }
    }

    public override bool CheckExistCombinationElement(IBlueprintRef combinationTemplate)
    {
      IBlueprint blueprint = (IBlueprint) null;
      if (combinationTemplate != null && combinationTemplate.Blueprint != null)
        blueprint = combinationTemplate.Blueprint;
      if (blueprint == null)
      {
        Logger.AddError(string.Format("Combination for checking element containing in storage {0} not defined at {1} !", (object) this.Parent.Name, (object) DynamicFSM.CurrentStateInfo));
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
                foreach (IStorableComponent storableComponent in this.Component.Items)
                {
                  if (storableComponent != null && !storableComponent.IsDisposed)
                  {
                    Guid templateId = storableComponent.Owner.TemplateId;
                    IWorldBlueprint engineTemplateByGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(templateId);
                    if (engineTemplateByGuid == null)
                      Logger.AddError(string.Format("Combination element checking in storage {0} error: template {1} id = {2} not registered in vm", (object) this.Parent.Name, (object) storableComponent.Owner.Name, (object) templateId));
                    else if (combinationDataStruct.ContainsItem((IBlueprint) engineTemplateByGuid))
                      return true;
                  }
                }
              }
            }
            else
              Logger.AddError(string.Format("Combination object {0} for checking element containing in storage {1}  not contains Combination data at", (object) blueprint.Name, (object) this.Parent.Name, (object) DynamicFSM.CurrentStateInfo));
          }
          else
            Logger.AddError(string.Format("Combination object {0} for checking element containing in storage {1}  not contains Combination data at", (object) blueprint.Name, (object) this.Parent.Name, (object) DynamicFSM.CurrentStateInfo));
        }
        else
          Logger.AddError(string.Format("Combination object {0} for checking element containing in storage {1}  not contains ObjectCombinationDataStruct param", (object) blueprint.Name, (object) this.Parent.Name, (object) DynamicFSM.CurrentStateInfo));
      }
      else
        Logger.AddError(string.Format("Combination object {0} for checking element containing in storage {1}  not contains Combination component at {2}", (object) blueprint.Name, (object) this.Parent.Name, (object) DynamicFSM.CurrentStateInfo));
      return false;
    }

    private void DoPickUpCombinationToInventoryByTemplate(
      IBlueprintRef combinationObject,
      IBlueprintRef containerTemplate,
      bool dropIfBusy = true)
    {
      if (combinationObject == null)
      {
        Logger.AddError(string.Format("Combination object for adding items to storage {0} not defined", (object) this.Parent.Name));
      }
      else
      {
        if (((VMEntity) this.Parent).IsCrowdItem)
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
                    ((GlobalStorageManager) VMGlobalStorageManager.Instance).AddCombinationToStorage(combinationData, (VMStorage) this, containerTypes, (OperationMultiTagsInfo) null, dropIfBusy);
                    this.OnModify();
                  }
                }
                else
                  Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component ObjectCombinationDataStruct param value is invalid", (object) blueprint.Name, (object) this.Parent.Name));
              }
              else
                Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component not contains ObjectCombinationDataStruct param value", (object) blueprint.Name, (object) this.Parent.Name));
            }
            else
              Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} combination component not contains ObjectCombinationDataStruct param", (object) blueprint.Name, (object) this.Parent.Name));
          }
          else
            Logger.AddError(string.Format("Combination object {0} for adding items to storage {1} not contains Combination component", (object) blueprint.Name, (object) this.Parent.Name));
        }
        else
          Logger.AddError(string.Format("Combination object with guid={0} for adding items to storage {1} not found", (object) combinationObject.Name, (object) this.Parent.Name));
        if (!((VMEntity) this.Parent).IsCrowdItem)
          return;
        GlobalStorageManager.AssyncStorageGroupCommandMode = false;
      }
    }

    protected static CrowdStorageCommandProcessor CrowdStorageCommandProcessor { get; private set; }
  }
}
