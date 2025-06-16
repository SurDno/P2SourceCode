using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components.Utilities;
using Engine.Source.Inventory;
using Engine.Source.Services;
using Engine.Source.Services.Templates;
using Inspectors;

namespace Engine.Source.Components
{
  [Required(typeof (ParametersComponent))]
  [Factory(typeof (IStorageComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class StorageComponent : EngineComponent, IStorageComponent, IComponent, INeedSave
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected string tag;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<TemplateInfo> inventoryTemplates = new List<TemplateInfo>();
    [StateSaveProxy(MemberEnum.CustomListReference)]
    [StateLoadProxy(MemberEnum.CustomListReference)]
    [CopyableProxy()]
    [Inspected]
    protected List<IStorableComponent> items = new List<IStorableComponent>();
    [Inspected]
    private HashSet<IInventoryComponent> containers = new HashSet<IInventoryComponent>();
    [FromThis]
    private ParametersComponent parameters;

    [Inspected]
    public IParameterValue<bool> IsFree { get; } = new ParameterValue<bool>();

    public string Tag => tag;

    public IEnumerable<IStorableComponent> Items => items;

    public IEnumerable<IInventoryComponent> Containers
    {
      get => containers;
    }

    public IEnumerable<IEntity> InventoryTemplates
    {
      get
      {
        ITemplateService templateService = ServiceLocator.GetService<ITemplateService>();
        foreach (TemplateInfo inventoryTemplate in inventoryTemplates)
        {
          IEntity template = templateService.GetTemplate<IEntity>(inventoryTemplate.Id);
          if (template != null)
            yield return template;
          template = null;
        }
      }
    }

    public IEnumerable<TemplateInfo> InventoryTemplateInfos
    {
      get => inventoryTemplates;
    }

    public bool NeedSave => items.Count != 0;

    public event Action<IStorableComponent, IInventoryComponent> OnAddItemEvent;

    public event Action<IStorableComponent, IInventoryComponent> OnRemoveItemEvent;

    public event Action<IStorableComponent, IInventoryComponent> OnChangeItemEvent;

    public event Action<IStorageComponent, IInventoryComponent> ChangeInventoryEvent;

    public void ClearItems()
    {
      foreach (IStorableComponent storableComponent in items.ToList())
      {
        if (RemoveItem(storableComponent))
          storableComponent.Owner.Dispose();
      }
    }

    public void ClearItems(IInventoryComponent inventory)
    {
      foreach (IStorableComponent storableComponent in items.ToList())
      {
        IInventoryComponent container = ((StorableComponent) storableComponent).Container;
        if ((inventory.Owner.Id == container.Owner.TemplateId || inventory.Owner.Template != null && inventory.Owner.Template.TemplateId == container.Owner.TemplateId || container.Owner.Template != null && inventory.Owner.Id == container.Owner.Template.TemplateId) && RemoveItem(storableComponent))
          storableComponent.Owner.Dispose();
      }
    }

    public bool AddItem(IStorableComponent item, IInventoryComponent container)
    {
      return AddItem(item, container, null);
    }

    public bool AddItem(IStorableComponent item, IInventoryComponent container, Cell cellTo)
    {
      if (item.Storage != null)
      {
        Debug.LogError((object) ("Item already added : " + item.Owner.GetInfo()));
        return false;
      }
      if (item.IsDisposed)
      {
        Debug.LogError((object) ("Item disposed : " + item.Owner.GetInfo()));
        return false;
      }
      if (!((Entity) item.Owner).IsAdded)
      {
        Debug.LogError((object) ("Item is not added in simulation : " + item.Owner.GetInfo()));
        return false;
      }
      if (item.Count <= 0)
      {
        Debug.LogError((object) ("Item count error, count : " + item.Count + " , owner : " + item.Owner.GetInfo()));
        return false;
      }
      Intersect intersect = StorageUtility.GetIntersect(this, container, (StorableComponent) item, cellTo);
      if (item.IsDisposed)
        return true;
      if (!intersect.IsAllowed)
        return false;
      ((StorableComponent) item).Container = intersect.Container;
      ((StorableComponent) item).Cell = intersect.Cell.To();
      ((StorableComponent) item).Storage = intersect.Storage;
      StorableComponent storableComponent = intersect.Storables.FirstOrDefault();
      if (storableComponent == null || storableComponent.IsDisposed)
      {
        items.Add(item);
        Action<IStorableComponent, IInventoryComponent> onAddItemEvent = OnAddItemEvent;
        if (onAddItemEvent != null)
          onAddItemEvent(intersect.Storable, intersect.Container);
      }
      else
      {
        storableComponent.Count += item.Count;
        Action<IStorableComponent, IInventoryComponent> onChangeItemEvent = OnChangeItemEvent;
        if (onChangeItemEvent != null)
          onChangeItemEvent(intersect.Storable, intersect.Container);
        item.Owner.Dispose();
      }
      return true;
    }

    public bool RemoveItem(IStorableComponent item)
    {
      if (item.Storage != this)
      {
        Debug.LogError((object) ("Wrong item storage : " + item.Owner.GetInfo()));
        return false;
      }
      if (item.IsDisposed)
      {
        Debug.LogError((object) ("Item disposed : " + item.Owner.GetInfo()));
        return false;
      }
      if (!((Entity) item.Owner).IsAdded)
      {
        Debug.LogError((object) ("Item is not added in simulation : " + item.Owner.GetInfo()));
        return false;
      }
      IInventoryComponent container = item.Container;
      ((StorableComponent) item).Container = null;
      ((StorableComponent) item).Cell = null;
      ((StorableComponent) item).Storage = null;
      items.Remove(item);
      Action<IStorableComponent, IInventoryComponent> onRemoveItemEvent = OnRemoveItemEvent;
      if (onRemoveItemEvent != null)
        onRemoveItemEvent(item, container);
      return true;
    }

    public bool MoveItem(
      IStorableComponent item,
      IStorageComponent storage,
      IInventoryComponent container)
    {
      return MoveItem(item, storage, container, null);
    }

    public bool MoveItem(
      IStorableComponent item,
      IStorageComponent storage,
      IInventoryComponent container,
      Cell toCell)
    {
      if (item.Storage != this)
      {
        Debug.LogError((object) ("Wrong item storage : " + item.Owner.GetInfo()));
        return false;
      }
      if (item.IsDisposed)
      {
        Debug.LogError((object) ("Item disposed : " + item.Owner.GetInfo()));
        return false;
      }
      if (!((Entity) item.Owner).IsAdded)
      {
        Debug.LogError((object) ("Item is not added in simulation : " + item.Owner.GetInfo()));
        return false;
      }
      if (item.Count <= 0)
      {
        Debug.LogError((object) ("Item count error, count : " + item.Count + " , owner : " + item.Owner.GetInfo()));
        return false;
      }
      if (!StorageUtility.GetIntersect(this, container, (StorableComponent) item, toCell).IsAllowed)
        return false;
      if (!RemoveItem(item))
      {
        Debug.LogError((object) ("Error remove item : " + item.Owner.GetInfo()));
        return false;
      }
      if (((StorageComponent) storage).AddItem(item, container, toCell))
        return true;
      Debug.LogError((object) ("Error add item : " + item.Owner.GetInfo()));
      return false;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      IsFree.Set(parameters.GetByName<bool>(ParameterNameEnum.IsFree));
    }

    public override void OnRemoved()
    {
      IsFree.Set(null);
      ClearItems();
      base.OnRemoved();
    }

    public void FireChangeInventoryEvent(IInventoryComponent inventory)
    {
      Action<IStorageComponent, IInventoryComponent> changeInventoryEvent = ChangeInventoryEvent;
      if (changeInventoryEvent == null)
        return;
      changeInventoryEvent(this, inventory);
    }

    [OnLoaded]
    private void OnLoaded()
    {
      foreach (IStorableComponent storableComponent in items.ToList())
      {
        Action<IStorableComponent, IInventoryComponent> onAddItemEvent = OnAddItemEvent;
        if (onAddItemEvent != null)
          onAddItemEvent(storableComponent, storableComponent.Container);
      }
    }

    public void AddContainer(IInventoryComponent inventory) => containers.Add(inventory);

    public void RemoveContainer(IInventoryComponent inventory) => containers.Remove(inventory);

    public void AddItemOrDrop(IStorableComponent item, IInventoryComponent container)
    {
      if (AddItem(item, container))
        return;
      ServiceLocator.GetService<DropBagService>().DropBag(item, Owner);
    }

    [Inspected(Mode = ExecuteMode.Edit)]
    private void RegenerateGuids()
    {
      foreach (TemplateInfo inventoryTemplate in inventoryTemplates)
        inventoryTemplate.Id = Guid.NewGuid();
      ServiceLocator.GetService<IEditorTemplateService>().SetDirty(Owner);
    }
  }
}
