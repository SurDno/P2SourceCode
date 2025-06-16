using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components.Saves;
using Engine.Source.Connections;
using Engine.Source.Inventory;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory(typeof (IStorableComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class StorableComponent : EngineComponent, IStorableComponent, IComponent, INeedSave
  {
    [StateSaveProxy]
    [StateLoadProxy]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool isEnabled = true;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<IInventoryPlaceholder> placeholder;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<StorableGroup> groups = new List<StorableGroup>();
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<IStorableTooltipComponent> tooltips = new List<IStorableTooltipComponent>();
    [StateSaveProxy]
    [StateLoadProxy]
    protected int count = 1;
    [StateSaveProxy]
    [StateLoadProxy()]
    protected int max = 1;
    private IStorageComponent storage;
    [FromThis]
    private ParametersComponent parameters;

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => isEnabled;
      set
      {
        isEnabled = value;
        OnChangeEnabled();
      }
    }

    public event Action<IStorableComponent> ChangeStorageEvent;

    public event Action<IStorableComponent> UseEvent;

    [Inspected]
    public IParameterValue<float> Durability { get; } = new ParameterValue<float>();

    [Inspected]
    public Typed<IInventoryPlaceholder> TypedPlaceholder => placeholder;

    [Inspected]
    public int Count
    {
      get => count;
      set
      {
        count = value;
        CheckValue();
      }
    }

    [Inspected]
    public IEnumerable<StorableGroup> Groups => groups;

    [Inspected]
    public int Max
    {
      get => max;
      set
      {
        max = value;
        CheckValue();
      }
    }

    [Inspected]
    public IStorageComponent Storage
    {
      get => storage;
      set
      {
        storage = value;
        ((Entity) Owner).DontSave = storage != null && ((Entity) storage.Owner).DontSave;
        Action<IStorableComponent> changeStorageEvent = ChangeStorageEvent;
        if (changeStorageEvent == null)
          return;
        changeStorageEvent(this);
      }
    }

    [Inspected]
    public IInventoryComponent Container { get; set; }

    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected]
    public Cell Cell { get; set; }

    [Inspected]
    public Invoice Invoice { get; set; }

    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected]
    public LocalizedText Title { get; set; }

    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected]
    public LocalizedText Tooltip { get; set; }

    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected]
    public LocalizedText Description { get; set; }

    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected]
    public LocalizedText SpecialDescription { get; set; }

    public InventoryPlaceholder Placeholder => (InventoryPlaceholder) placeholder.Value;

    public List<IStorableTooltipComponent> Tooltips => tooltips;

    [StateSaveProxy]
    [StateLoadProxy]
    protected StorableData StorableData
    {
      get
      {
        if (Storage == null)
        {
          Debug.LogError((object) ("Storage not found, owner : " + Owner.GetInfo()));
          return null;
        }
        if (Container == null)
        {
          Debug.LogError((object) ("Container not found, owner : " + Owner.GetInfo()));
          return null;
        }
        StorableData storableData = ProxyFactory.Create<StorableData>();
        storableData.Storage = Storage;
        storableData.TemplateId = Container.Owner.TemplateId;
        return storableData;
      }
      set
      {
        if (value == null)
          Debug.LogError((object) ("StorableData is null, owner : " + Owner.GetInfo()));
        else if (value.Storage == null)
        {
          Debug.LogError((object) ("Storage not found, owner : " + Owner.GetInfo()));
        }
        else
        {
          storage = value.Storage;
          Container = value.Storage.Containers.FirstOrDefault(o => o.Owner.TemplateId == value.TemplateId);
        }
      }
    }

    public bool NeedSave => true;

    public void Use()
    {
      Action<IStorableComponent> useEvent = UseEvent;
      if (useEvent == null)
        return;
      useEvent(this);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      Durability.Set(parameters?.GetByName<float>(ParameterNameEnum.Durability));
      CheckValue();
    }

    public override void OnRemoved()
    {
      if (storage != null)
      {
        storage.RemoveItem(this);
        storage = null;
      }
      Durability.Set(null);
      base.OnRemoved();
    }

    private void CheckValue()
    {
      if (count <= max)
        return;
      Debug.LogError((object) ("Count : " + count + " , max : " + max + " , owner : " + Owner.GetInfo()));
    }

    [OnLoaded]
    private void OnLoaded()
    {
      Action<IStorableComponent> changeStorageEvent = ChangeStorageEvent;
      if (changeStorageEvent == null)
        return;
      changeStorageEvent(this);
    }
  }
}
