using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Storage", typeof (IStorageComponent))]
  public class VMStorage : VMEngineComponent<IStorageComponent>
  {
    public const string ComponentName = "Storage";
    protected List<string> innerContainerTags = new List<string>();
    protected List<IInventoryComponent> innerContainersList = new List<IInventoryComponent>();
    private static StringBuilder DataStringBuilder = new StringBuilder();
    public static bool CREATE_VIRTUAL_CONTAINERS_MODE = false;
    public static bool TestSimplePickUpMode = false;

    [Event("Item adding event", "item:Storable,Container template:Inventory")]
    public event VMStorage.ItemInventoryActionEventType AddItemEvent;

    [Event("Item removing event", "item:Storable,Container template:Inventory")]
    public event VMStorage.ItemInventoryActionEventType RemoveItemEvent;

    [Event("Item change event", "item:Storable,Container template:Inventory")]
    public event VMStorage.ItemInventoryActionEventType ChangeItemEvent;

    [Method("Add items combination", "Item cobination:Combination", "")]
    public virtual void PickUpCombination(IBlueprintRef combinationObject)
    {
    }

    [Method("Add items combination", "Item cobination:Combination,dropIfBusy", "")]
    public virtual void PickUpCombinationWithDrop(IBlueprintRef combinationObject, bool dropIfBusy)
    {
    }

    [Method("Add items combination to inventory", "Item template:Storable,Container template:Inventory", "")]
    public virtual void PickUpCombinationToInentoryByTemplate(
      IBlueprintRef combinationObject,
      IBlueprintRef containerTemplate)
    {
    }

    [Method("Add items combination to inventory", "Item template:Storable,Container template:Inventory,dropIfBusy", "")]
    public virtual void PickUpCombinationToInentoryByTemplateWithDrop(
      IBlueprintRef combinationObject,
      IBlueprintRef containerTemplate,
      bool dropIfBusy)
    {
    }

    [Method("Add item", "Item template:Storable", "")]
    public void PickUpByTemplate([Template] IEntity template)
    {
      if (template == null)
      {
        Logger.AddError(string.Format("PickUp thing entity is null !"));
      }
      else
      {
        IStorableComponent storableComponent = VMStorable.MakeStorableByTemplate(this.Parent, template);
        if (storableComponent == null)
        {
          Logger.AddError(string.Format("Cannot crteate storable entity by template {0} !", (object) template.Name));
        }
        else
        {
          IStorableComponent component = storableComponent.Owner.GetComponent<IStorableComponent>();
          if (!VMStorage.DoAddItemToStorage(this.Component, component, dropIfBusy: true))
            component.Owner.Dispose();
          this.OnModify();
        }
      }
    }

    [Method("Add items", "Item template:Storable, Items count", "")]
    public void PickUpByTemplate_v1([Template] IEntity template, int thingsCount)
    {
      try
      {
        if (template == null)
        {
          Logger.AddError(string.Format("PickUp thing entity is null !"));
        }
        else
        {
          template.GetComponent<IStorableComponent>();
          int num1 = 0;
          IStorableComponent component;
          do
          {
            component = VMStorable.MakeStorableByTemplate(this.Parent, template).Owner.GetComponent<IStorableComponent>();
            int num2 = component.Max;
            if (num2 > thingsCount - num1)
              num2 = thingsCount - num1;
            component.Count = num2;
            if (VMStorage.DoAddItemToStorage(this.Component, component, dropIfBusy: true))
              num1 += num2;
            else
              goto label_7;
          }
          while (num1 < thingsCount);
          goto label_8;
label_7:
          component.Owner.Dispose();
label_8:
          this.OnModify();
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cannot pick up items {0} to {1} storage: {2}", (object) template.Name, (object) this.Parent.Name, (object) ex));
      }
    }

    [Method("Add item or drop", "Item template:Storable,Container template:Inventory", "")]
    public void AddItemOrDropByTemplate([Template] IEntity template, [Template] IEntity containerTemplate)
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
          IInventoryComponent containerByTemplate = this.GetContainerByTemplate(containerTemplate);
          if (containerByTemplate == null)
            Logger.AddError(string.Format("PickUp target container name={0} id ={1} not found !", (object) containerTemplate.Name, (object) containerTemplate.Id));
          else
            this.Component.AddItemOrDrop(VMStorable.MakeStorableByTemplate(this.Parent, template), containerByTemplate);
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cannot pick up items {0} to {1} storage: {2}", (object) template.Name, (object) this.Parent.Name, (object) ex));
      }
    }

    [Method("Add items to inventory", "Item template:Storable,Container template:Inventory,Items count", "")]
    public virtual void PickUpToInentoryByTemplate(
      [Template] IEntity template,
      [Template] IEntity containerTemplate,
      int thingsCount)
    {
    }

    [Method("Remove item", "Item template:Storable, Items count", "")]
    public void RemoveThingByTemplate([Template] IEntity template, int thingsCount)
    {
      if (template == null)
      {
        Logger.AddError(string.Format("Remove thing entity is null !"));
      }
      else
      {
        List<IEntity> entityList = new List<IEntity>();
        foreach (IStorableComponent storableComponent in this.Component.Items)
        {
          if (storableComponent != null && !storableComponent.IsDisposed && storableComponent.Owner.Name == template.Name)
          {
            if (storableComponent.Count <= thingsCount)
            {
              entityList.Add(storableComponent.Owner);
              thingsCount -= storableComponent.Count;
              if (thingsCount <= 0)
                break;
            }
            else
            {
              storableComponent.Count -= thingsCount;
              break;
            }
          }
        }
        this.OnModify();
        for (int index = 0; index < entityList.Count; ++index)
          entityList[index].Dispose();
      }
    }

    [Method("Move item", "Item template:Storable, New place:Storage", "")]
    public void ReceiveByTemplate([Template] IEntity template, IObjRef newPlace)
    {
      if (template == null)
      {
        Logger.AddError(string.Format("Receive thing entity is null !"));
      }
      else
      {
        IStorableComponent storableByTemplate = this.GetStorableByTemplate((Engine.Common.IObject) template);
        if (storableByTemplate == null)
          return;
        if (newPlace == null)
        {
          Logger.AddError(string.Format("New place for storable receiving in {0} not defined !", (object) this.Parent.Name));
        }
        else
        {
          IEntity entity = ServiceCache.Simulation.Get(newPlace.EngineGuid);
          if (entity == null)
          {
            Logger.AddError(string.Format("New place entity guid={0} for storable receiving in {1} not found !", (object) newPlace.EngineGuid, (object) this.Parent.Name));
          }
          else
          {
            IStorageComponent component = entity.GetComponent<IStorageComponent>();
            if (component == null)
            {
              Logger.AddError(string.Format("New place entity {0} for storable receiving in {1} hasn't storage component !", (object) entity.Name, (object) this.Parent.Name));
            }
            else
            {
              this.OnModify();
              this.Component.MoveItem(storableByTemplate, component, (IInventoryComponent) null);
            }
          }
        }
      }
    }

    [Method("Move random items", "New place:Storage,Items count", "")]
    public void ReceiveItems(IObjRef newPlace, int itemsCount)
    {
      if (newPlace == null)
      {
        Logger.AddError(string.Format("New place for storable receiving in {0} not defined !", (object) this.Parent.Name));
      }
      else
      {
        IEntity entity = ServiceCache.Simulation.Get(newPlace.EngineGuid);
        if (entity == null)
        {
          Logger.AddError(string.Format("New place entity guid={0} for storable receiving in {1} not found !", (object) newPlace.EngineGuid, (object) this.Parent.Name));
        }
        else
        {
          IStorageComponent component = entity.GetComponent<IStorageComponent>();
          if (component == null)
          {
            Logger.AddError(string.Format("New place entity {0} for storable receiving in {1} hasn't storage component !", (object) entity.Name, (object) this.Parent.Name));
          }
          else
          {
            List<IStorableComponent> storableComponentList = new List<IStorableComponent>();
            if (itemsCount > this.Component.Items.Count<IStorableComponent>())
              itemsCount = this.Component.Items.Count<IStorableComponent>();
            int num = (int) Math.Floor((double) this.Component.Items.Count<IStorableComponent>() * VMMath.GetRandomDouble());
            for (int index1 = 0; index1 < itemsCount; ++index1)
            {
              int index2 = index1 + num;
              if (index2 >= this.Component.Items.Count<IStorableComponent>())
                index2 -= this.Component.Items.Count<IStorableComponent>();
              IStorableComponent storableComponent = this.Component.Items.ElementAt<IStorableComponent>(index2);
              if (storableComponent != null && !storableComponent.IsDisposed)
                storableComponentList.Add(storableComponent);
            }
            this.OnModify();
            for (int index = 0; index < storableComponentList.Count; ++index)
              this.Component.MoveItem(storableComponentList[index], component, (IInventoryComponent) null);
          }
        }
      }
    }

    [Method("Move all items", "New place:Storage", "")]
    public void ReceiveAllItems(IObjRef newPlace)
    {
      if (newPlace == null)
      {
        Logger.AddError(string.Format("New place for storable receiving in {0} not defined !", (object) this.Parent.Name));
      }
      else
      {
        IEntity entity = ServiceCache.Simulation.Get(newPlace.EngineGuid);
        if (entity == null)
        {
          Logger.AddError(string.Format("New place entity guid={0} for storable receiving in {1} not found !", (object) newPlace.EngineGuid, (object) this.Parent.Name));
        }
        else
        {
          IStorageComponent component = entity.GetComponent<IStorageComponent>();
          if (component == null)
          {
            Logger.AddError(string.Format("New place entity {0} for storable receiving in {1} hasn't storage component !", (object) entity.Name, (object) this.Parent.Name));
          }
          else
          {
            List<IStorableComponent> storableComponentList = new List<IStorableComponent>();
            foreach (IStorableComponent storableComponent in this.Component.Items)
            {
              if (storableComponent != null && !storableComponent.IsDisposed)
                storableComponentList.Add(storableComponent);
            }
            for (int index = 0; index < storableComponentList.Count; ++index)
              this.Component.MoveItem(storableComponentList[index], component, (IInventoryComponent) null);
            this.OnModify();
          }
        }
      }
    }

    [Property("Is Free", "", false, false, false)]
    public bool IsFree
    {
      get => this.Component.IsFree.Value;
      set => this.Component.IsFree.Value = value;
    }

    [Method("Get items count", "", "")]
    public int GetItemsCount()
    {
      return this.Component.Items.Where<IStorableComponent>((Func<IStorableComponent, bool>) (o => o != null && !o.IsDisposed)).Count<IStorableComponent>();
    }

    [Method("Get items count by template", "Item template:Storable", "")]
    [SpecialFunction(ESpecialFunctionName.SFN_GET_ITEMS_COUNT_BY_TEMPLATE)]
    public int GetItemsCountByTemplate([Template] IEntity template)
    {
      if (template == null)
      {
        Logger.AddError(string.Format("Get storable entity is null !"));
        return 0;
      }
      int itemsCountByTemplate = 0;
      foreach (IStorableComponent storableComponent in this.Component.Items)
      {
        if (storableComponent.Owner.TemplateId == template.Id)
          itemsCountByTemplate += storableComponent.Count;
      }
      return itemsCountByTemplate;
    }

    [Method("Check exist combination element", "Combination template:Combination", "")]
    public virtual bool CheckExistCombinationElement(IBlueprintRef combinationTemplate) => false;

    [Method("Check item in place", "Item template:Storable, Container template:Inventory", "")]
    public bool CheckItemInPlace([Template] IEntity template, [Template] IEntity containerTemplate)
    {
      if (template == null)
      {
        Logger.AddError(string.Format("Storable entity for item checking not defined !"));
        return false;
      }
      if (containerTemplate == null)
      {
        Logger.AddError(string.Format("Storage place for item checking not defined !"));
        return false;
      }
      foreach (IStorableComponent storableComponent in this.Component.Items)
      {
        if (storableComponent != null && !storableComponent.IsDisposed && storableComponent.Container.Owner.TemplateId == containerTemplate.Id && storableComponent.Owner.TemplateId == template.Id)
          return true;
      }
      return false;
    }

    [Method("Set storables title", "Item template:Storable, text", "")]
    public void SetStorablesTitle([Template] IEntity template, ITextRef text)
    {
      if (template == null)
        Logger.AddError(string.Format("Storable entity for title setting not defined !"));
      LocalizedText engineTextInstance = EngineAPIManager.CreateEngineTextInstance(text);
      foreach (IStorableComponent storableComponent in this.Component.Items)
      {
        if (storableComponent != null && !storableComponent.IsDisposed && storableComponent.Owner.TemplateId == template.Id)
          VMStorable.DoSetStorableTextField(storableComponent, "title", engineTextInstance);
      }
    }

    [Method("Set storables description", "Item template:Storable, text", "")]
    public void SetStorablesDescription([Template] IEntity template, ITextRef text)
    {
      if (template == null)
        Logger.AddError(string.Format("Storable entity for description setting not defined !"));
      LocalizedText engineTextInstance = EngineAPIManager.CreateEngineTextInstance(text);
      foreach (IStorableComponent storableComponent in this.Component.Items)
      {
        if (storableComponent != null && !storableComponent.IsDisposed && storableComponent.Owner.TemplateId == template.Id)
          VMStorable.DoSetStorableTextField(storableComponent, "description", engineTextInstance);
      }
    }

    [Method("Set storables description", "Item template:Storable, text", "")]
    public void SetStorablesTooltip([Template] IEntity template, ITextRef text)
    {
      if (template == null)
        Logger.AddError(string.Format("Storable entity for tooltip setting not defined !"));
      LocalizedText engineTextInstance = EngineAPIManager.CreateEngineTextInstance(text);
      foreach (IStorableComponent storableComponent in this.Component.Items)
      {
        if (storableComponent != null && !storableComponent.IsDisposed && storableComponent.Owner.TemplateId == template.Id)
          VMStorable.DoSetStorableTextField(storableComponent, "tooltip", engineTextInstance);
      }
    }

    public IStorableComponent GetStorableByTemplate(Engine.Common.IObject template)
    {
      if (template == null)
      {
        Logger.AddError(string.Format("Get storable entity is null !"));
        return (IStorableComponent) null;
      }
      foreach (IStorableComponent storableByTemplate in this.Component.Items)
      {
        if (storableByTemplate != null && !storableByTemplate.IsDisposed && storableByTemplate.Owner.Name == template.Name)
          return storableByTemplate;
      }
      return (IStorableComponent) null;
    }

    [Method("Clear storage", "", "")]
    public void Free()
    {
      try
      {
        this.Component.ClearItems();
      }
      catch (Exception ex)
      {
        string str = "Not defined";
        if (this.Component != null && this.Component.Owner != null)
          str = this.Component.Owner.Name;
        Logger.AddError(string.Format("Storage {0} clear error: {1)", (object) str, (object) ex.Message));
      }
      this.OnModify();
    }

    [Method("Clear Container", "Container template:Inventory", "")]
    public void FreeContainer([Template] IEntity containerTemplate)
    {
      try
      {
        if (containerTemplate == null)
        {
          Logger.AddError(string.Format("Storage container for claring not defined !"));
          return;
        }
        IInventoryComponent component = containerTemplate.GetComponent<IInventoryComponent>();
        if (component != null)
          this.Component.ClearItems(component);
        else
          Logger.AddError(string.Format("Container {0} not found in storage {1} at {2} !", (object) containerTemplate.Name, (object) this.Parent.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
      catch (Exception ex)
      {
        string str1 = "Not defined";
        string str2 = "Not defined";
        if (this.Component != null && this.Component.Owner != null)
          str1 = this.Component.Owner.Name;
        if (containerTemplate != null)
          str2 = containerTemplate.Name;
        Logger.AddError(string.Format("Storage {0} container {1} clear error: {2)", (object) str1, (object) str2, (object) ex.Message));
      }
      this.OnModify();
    }

    [Method("Get Open", "Container template:Inventory", "")]
    public ContainerOpenStateEnum GetOpen([Template] IEntity containerTemplate)
    {
      IInventoryComponent containerByTemplate = this.GetContainerByTemplate(containerTemplate);
      if (containerByTemplate != null)
        return containerByTemplate.OpenState.Value;
      Logger.AddWarning(string.Format("Container {0} not found in {1}", (object) containerTemplate.Name, (object) this.Parent.Name));
      return ContainerOpenStateEnum.Closed;
    }

    [Method("Set Open", "Container template:Inventory, value:bool", "")]
    public void SetOpen([Template] IEntity containerTemplate, ContainerOpenStateEnum value)
    {
      IInventoryComponent containerByTemplate = this.GetContainerByTemplate(containerTemplate);
      if (containerByTemplate != null)
        containerByTemplate.OpenState.Value = value;
      else
        Logger.AddWarning(string.Format("Container {0} not found in {1}", (object) containerTemplate.Name, (object) this.Parent.Name));
      this.OnModify();
    }

    [Method("Set All Open", "value:bool", "")]
    public void SetAllOpen(ContainerOpenStateEnum value)
    {
      foreach (IInventoryComponent container in this.Component.Containers)
      {
        if (container != null)
          container.OpenState.Value = value;
      }
      this.OnModify();
    }

    [Method("Get Available", "Container template:Inventory", "")]
    public bool GetAvailable([Template] IEntity containerTemplate)
    {
      IInventoryComponent containerByTemplate = this.GetContainerByTemplate(containerTemplate);
      if (containerByTemplate != null)
        return containerByTemplate.Available.Value;
      Logger.AddWarning(string.Format("Container {0} not found in {1}", (object) containerTemplate.Name, (object) this.Parent.Name));
      return false;
    }

    [Method("Set Available", "Container template:Inventory, value:bool", "")]
    public void SetAvailable([Template] IEntity containerTemplate, bool value)
    {
      IInventoryComponent containerByTemplate = this.GetContainerByTemplate(containerTemplate);
      if (containerByTemplate != null)
        containerByTemplate.Available.Value = value;
      else
        Logger.AddWarning(string.Format("Container {0} not found in {1}", (object) containerTemplate.Name, (object) this.Parent.Name));
    }

    [Method("Set All Available", "value:bool", "")]
    public void SetAllAvailable(bool value)
    {
      foreach (IInventoryComponent container in this.Component.Containers)
      {
        if (container != null)
          container.Available.Value = value;
      }
    }

    [Method("Get Enabled", "Container template:Inventory", "")]
    public bool GetEnabled([Template] IEntity containerTemplate)
    {
      IInventoryComponent containerByTemplate = this.GetContainerByTemplate(containerTemplate);
      if (containerByTemplate != null)
        return containerByTemplate.Enabled.Value;
      Logger.AddWarning(string.Format("Container {0} not found in {1}", (object) containerTemplate.Name, (object) this.Parent.Name));
      return false;
    }

    [Method("Set Enabled", "Container template:Inventory, value:bool", "")]
    public void SetEnabled([Template] IEntity containerTemplate, bool value)
    {
      IInventoryComponent containerByTemplate = this.GetContainerByTemplate(containerTemplate);
      if (containerByTemplate != null)
        containerByTemplate.Enabled.Value = value;
      else
        Logger.AddWarning(string.Format("Container {0} not found in {1}", (object) containerTemplate.Name, (object) this.Parent.Name));
      this.OnModify();
    }

    [Method("Set All Enabled", "value:bool", "")]
    public void SetAllEnabled(bool value)
    {
      foreach (IInventoryComponent container in this.Component.Containers)
      {
        if (container != null)
          container.Enabled.Value = value;
      }
      this.OnModify();
    }

    [Method("Set disease", "Container template:Inventory, value:float", "")]
    public void SetDisease([Template] IEntity containerTemplate, float value)
    {
      IInventoryComponent containerByTemplate = this.GetContainerByTemplate(containerTemplate);
      if (containerByTemplate != null)
        containerByTemplate.Disease.Value = value;
      else
        Logger.AddWarning(string.Format("Container {0} not found in {1}", (object) containerTemplate.Name, (object) this.Parent.Name));
      this.OnModify();
    }

    protected IInventoryComponent GetContainerByTemplate(IEntity containerTemplate)
    {
      return this.Component.Containers.FirstOrDefault<IInventoryComponent>((Func<IInventoryComponent, bool>) (o =>
      {
        if (o.Owner.TemplateId == containerTemplate.Id)
          return true;
        return o.Owner.Template != null && o.Owner.Template.TemplateId == containerTemplate.Id;
      }));
    }

    public int InnerContainersCount => this.innerContainersList.Count;

    public IInventoryComponent GetInnerContainer(int containerIndex)
    {
      if (containerIndex >= 0 && containerIndex < this.innerContainersList.Count)
        return this.innerContainersList[containerIndex];
      Logger.AddError(string.Format("Invalid inner container index {0} int {1} storage !", (object) containerIndex, (object) this.Parent.Name));
      return (IInventoryComponent) null;
    }

    public List<IInventoryComponent> GetInnerContainersByTagsList(OperationMultiTagsInfo tagsList)
    {
      if (tagsList == null)
        return this.innerContainersList;
      List<IInventoryComponent> containersByTagsList = new List<IInventoryComponent>();
      for (int index = 0; index < this.innerContainersList.Count; ++index)
      {
        if (index < this.innerContainerTags.Count && tagsList.CheckTag(this.innerContainerTags[index]))
          containersByTagsList.Add(this.innerContainersList[index]);
      }
      return containersByTagsList;
    }

    public string GetInnerContainerTag(int containerIndex)
    {
      if (containerIndex >= 0 && containerIndex < this.innerContainerTags.Count)
        return this.innerContainerTags[containerIndex];
      Logger.AddError(string.Format("Invalid inner container index {0} in {1} storage !", (object) containerIndex, (object) this.Parent.Name));
      return "";
    }

    public void SetInnerContainerTag(int containerIndex, string sTag)
    {
      if (containerIndex >= 0 && containerIndex < this.innerContainerTags.Count)
        this.innerContainerTags[containerIndex] = sTag;
      else
        Logger.AddError(string.Format("Invalid inner container index {0} in {1} storage !", (object) containerIndex, (object) this.Parent.Name));
      this.OnModify();
    }

    public void ResetInnerContainersTags()
    {
      for (int index = 0; index < this.innerContainerTags.Count; ++index)
        this.innerContainerTags[index] = VMCommon.defaultTag;
      this.OnModify();
    }

    public override void OnCreate()
    {
    }

    public override void AfterCreate()
    {
      if (this.Component == null)
      {
        Logger.AddError(string.Format("Component {0} instance not created in {1} !", (object) this.Name, (object) this.Parent.Name));
      }
      else
      {
        try
        {
          if (this.IsTemplate)
            return;
          this.innerContainersList.Clear();
          this.innerContainerTags.Clear();
          this.innerContainersList.Capacity = this.Component.InventoryTemplates.Count<IEntity>();
          this.innerContainerTags.Capacity = this.Component.InventoryTemplates.Count<IEntity>();
          List<VMBaseEntity> childEntities = this.Parent.GetChildEntities();
          if (childEntities != null)
          {
            for (int index = 0; index < childEntities.Count; ++index)
            {
              if (childEntities[index].Instance != null)
              {
                IInventoryComponent component = childEntities[index].Instance.GetComponent<IInventoryComponent>();
                if (component != null)
                {
                  this.innerContainersList.Add(component);
                  this.innerContainerTags.Add("");
                }
              }
              else
                Logger.AddError(string.Format("Invalid inventory container creation: Storage {0} inner inventory {1} engine instance not defined !", (object) this.Parent.Name, (object) childEntities[index].Name));
            }
          }
          if (this.innerContainersList.Count != 0)
            return;
          foreach (IEntity inventoryTemplate in this.Component.InventoryTemplates)
          {
            IInventoryComponent inventoryComponent = VMStorage.MakeStorageInventoryByTemplate(this.Parent, inventoryTemplate);
            if (inventoryComponent != null)
            {
              this.innerContainersList.Add(inventoryComponent);
              this.innerContainerTags.Add("");
            }
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Entity {0} storage component aftercreation error: {1} !", (object) this.Parent.Name, (object) ex.ToString()));
        }
      }
    }

    public void OnLoadInventory(IEntity inventoryEntity)
    {
      this.innerContainersList.Add(inventoryEntity.GetComponent<IInventoryComponent>());
    }

    public void CheckInventoryTagsConsistensy()
    {
      if (this.innerContainersList.Count == this.innerContainerTags.Count)
        return;
      if (this.innerContainerTags.Count < this.innerContainersList.Count)
      {
        for (int index = 0; index < this.innerContainersList.Count - this.innerContainerTags.Count; ++index)
          this.innerContainerTags.Add("");
      }
      else
        this.innerContainerTags.RemoveRange(this.innerContainersList.Count, this.innerContainerTags.Count - this.innerContainersList.Count);
    }

    public static IInventoryComponent MakeStorageInventoryByTemplate(
      VMBaseEntity storageEntity,
      IEntity template)
    {
      if (template.GetComponent<IInventoryComponent>() == null)
      {
        Logger.AddError(string.Format("Invalid inventory object template {0}: there is no inventory component!", (object) template.Name));
        return (IInventoryComponent) null;
      }
      try
      {
        VMBaseEntity dynamicChildInstance = EngineAPIManager.Instance.CreateWorldTemplateDynamicChildInstance(template, storageEntity, storageEntity.Instance);
        if (dynamicChildInstance != null)
          return dynamicChildInstance.Instance.GetComponent<IInventoryComponent>();
        Logger.AddError(string.Format("Cannot create storage inventory entity in {0}, probably editor template with engine id = {1} not found", (object) storageEntity.Name, (object) template.Id));
        return (IInventoryComponent) null;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Making inventory instance in storage {0} by template {1}  error: {2}", (object) storageEntity.Name, (object) template.Name, (object) ex));
      }
      return (IInventoryComponent) null;
    }

    public static bool DoAddItemToStorage(
      IStorageComponent storage,
      IStorableComponent storable,
      IInventoryComponent container = null,
      bool dropIfBusy = false)
    {
      if (storage == null)
      {
        Logger.AddError(string.Format("Cannot add item to storage, storage component not defined"));
        return false;
      }
      if (storable == null)
      {
        Logger.AddError(string.Format("Cannot add item to storage, storable component not defined"));
        return false;
      }
      if (storable.Count <= 0)
      {
        Logger.AddWarning(string.Format("Add item to storage warning: adding items '{0}' count is 0", (object) storable.Owner.Name));
        return false;
      }
      try
      {
        bool storage1;
        if (dropIfBusy)
        {
          IEntity owner = storable.Owner;
          if (storable.Owner == null)
          {
            Logger.AddError(string.Format("Invalid storable adding to {0} at", (object) storage.Owner.Name));
            storage1 = false;
          }
          else
          {
            storage.AddItemOrDrop(storable, container);
            storage1 = true;
          }
        }
        else
          storage1 = storage.AddItem(storable, container);
        return storage1;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cannot add item {0} to storage {1}, error: {2}", (object) storage.Owner, (object) storable.Owner, (object) ex.ToString()));
      }
      return false;
    }

    public static bool AssyncStorageCommandMode { get; set; } = true;

    private void RaiseAddItemEvent(IStorableComponent item, IInventoryComponent container)
    {
      try
      {
        Guid id = item.Owner.Id;
        if (EngineAPIManager.Instance.GetEditorTemplateByEngineGuid(id) == null)
        {
          IEntity template = (IEntity) item.Owner.Template ?? VMStorable.GetEngTemplateByStorableName(item.Owner.Name);
          if (template == null)
            Logger.AddError(string.Format("New added to {0} storable entity engine template not defined !", (object) this.Parent.Name));
          else
            VMStorable.MakeStorableByInstance(this, item, template);
          if (EngineAPIManager.Instance.GetEditorTemplateByEngineGuid(id) == null)
          {
            Logger.AddWarning(string.Format("New added to {0} storable entity engine template not defined !", (object) this.Parent.Name));
            return;
          }
        }
        if (this.AddItemEvent == null)
          return;
        IEntity template1 = (IEntity) null;
        if (container != null && container.Owner != null)
          template1 = (IEntity) container.Owner.Template;
        this.AddItemEvent(item.Owner, template1);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("RaiseAddItemEvent error: {0} in {1} !", (object) ex.ToString(), (object) this.Parent.Name));
      }
    }

    private void RaiseRemoveItemEvent(IStorableComponent item, IInventoryComponent container)
    {
      try
      {
        if (item.Owner == null)
          return;
        Guid id = item.Owner.Id;
        if (EngineAPIManager.Instance.GetEditorTemplateByEngineGuid(id) == null)
        {
          IEntity template = (IEntity) item.Owner.Template ?? VMStorable.GetEngTemplateByStorableName(item.Owner.Name);
          if (template == null)
          {
            Logger.AddError(string.Format("Removing from {0} storable entity engine template not defined !", (object) this.Parent.Name));
            return;
          }
          VMStorable.MakeStorableByInstance(this, item, template);
          if (EngineAPIManager.Instance.GetEditorTemplateByEngineGuid(id) == null)
          {
            Logger.AddWarning(string.Format("Removing from  {0} storable entity engine template not defined !", (object) this.Parent.Name));
            return;
          }
        }
        if (this.RemoveItemEvent == null)
          return;
        this.RemoveItemEvent(item.Owner, (IEntity) container.Owner.Template);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Removing item at {0} storage event raising error: {1}", (object) this.Parent.Name, (object) ex.Message));
      }
    }

    private void RaiseChangeItemEvent(IStorableComponent item, IInventoryComponent container)
    {
      Guid id = item.Owner.Id;
      if (EngineAPIManager.Instance.GetEditorTemplateByEngineGuid(id) == null)
      {
        IEntity template = (IEntity) item.Owner.Template ?? VMStorable.GetEngTemplateByStorableName(item.Owner.Name);
        if (template == null)
        {
          Logger.AddError(string.Format("Change from {0} storable entity engine template not defined !", (object) this.Parent.Name));
          return;
        }
        VMStorable.MakeStorableByInstance(this, item, template);
        if (EngineAPIManager.Instance.GetEditorTemplateByEngineGuid(id) == null)
        {
          Logger.AddWarning(string.Format("Change from  {0} storable entity engine template not defined !", (object) this.Parent.Name));
          return;
        }
      }
      if (this.ChangeItemEvent == null)
        return;
      this.ChangeItemEvent(item.Owner, (IEntity) container.Owner.Template);
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.OnAddItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.RaiseAddItemEvent);
      this.Component.OnRemoveItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.RaiseRemoveItemEvent);
      this.Component.OnChangeItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.RaiseChangeItemEvent);
      this.innerContainersList.Clear();
      base.Clear();
    }

    protected override void Init()
    {
      this.SetEngineData(this.TemplateComponent.Tag);
      if (this.IsTemplate)
        return;
      this.Component.OnAddItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.RaiseAddItemEvent);
      this.Component.OnRemoveItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.RaiseRemoveItemEvent);
      this.Component.OnChangeItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.RaiseChangeItemEvent);
    }

    public delegate void ItemInventoryActionEventType(IEntity entity, [Template] IEntity template);
  }
}
