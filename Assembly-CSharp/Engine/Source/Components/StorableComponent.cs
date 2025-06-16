using Cofe.Proxies;
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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Engine.Source.Components
{
  [Factory(typeof (IStorableComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class StorableComponent : EngineComponent, IStorableComponent, IComponent, INeedSave
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool isEnabled = true;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<IInventoryPlaceholder> placeholder;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<StorableGroup> groups = new List<StorableGroup>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<IStorableTooltipComponent> tooltips = new List<IStorableTooltipComponent>();
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected int count = 1;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected int max = 1;
    private IStorageComponent storage;
    [FromThis]
    private ParametersComponent parameters;

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => this.isEnabled;
      set
      {
        this.isEnabled = value;
        this.OnChangeEnabled();
      }
    }

    public event Action<IStorableComponent> ChangeStorageEvent;

    public event Action<IStorableComponent> UseEvent;

    [Inspected]
    public IParameterValue<float> Durability { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public Typed<IInventoryPlaceholder> TypedPlaceholder => this.placeholder;

    [Inspected]
    public int Count
    {
      get => this.count;
      set
      {
        this.count = value;
        this.CheckValue();
      }
    }

    [Inspected]
    public IEnumerable<StorableGroup> Groups => (IEnumerable<StorableGroup>) this.groups;

    [Inspected]
    public int Max
    {
      get => this.max;
      set
      {
        this.max = value;
        this.CheckValue();
      }
    }

    [Inspected]
    public IStorageComponent Storage
    {
      get => this.storage;
      set
      {
        this.storage = value;
        ((Entity) this.Owner).DontSave = this.storage != null && ((Entity) this.storage.Owner).DontSave;
        Action<IStorableComponent> changeStorageEvent = this.ChangeStorageEvent;
        if (changeStorageEvent == null)
          return;
        changeStorageEvent((IStorableComponent) this);
      }
    }

    [Inspected]
    public IInventoryComponent Container { get; set; }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public Cell Cell { get; set; }

    [Inspected]
    public Invoice Invoice { get; set; }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public LocalizedText Title { get; set; }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public LocalizedText Tooltip { get; set; }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public LocalizedText Description { get; set; }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public LocalizedText SpecialDescription { get; set; }

    public InventoryPlaceholder Placeholder => (InventoryPlaceholder) this.placeholder.Value;

    public List<IStorableTooltipComponent> Tooltips => this.tooltips;

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected StorableData StorableData
    {
      get
      {
        if (this.Storage == null)
        {
          Debug.LogError((object) ("Storage not found, owner : " + this.Owner.GetInfo()));
          return (StorableData) null;
        }
        if (this.Container == null)
        {
          Debug.LogError((object) ("Container not found, owner : " + this.Owner.GetInfo()));
          return (StorableData) null;
        }
        StorableData storableData = ProxyFactory.Create<StorableData>();
        storableData.Storage = this.Storage;
        storableData.TemplateId = this.Container.Owner.TemplateId;
        return storableData;
      }
      set
      {
        if (value == null)
          Debug.LogError((object) ("StorableData is null, owner : " + this.Owner.GetInfo()));
        else if (value.Storage == null)
        {
          Debug.LogError((object) ("Storage not found, owner : " + this.Owner.GetInfo()));
        }
        else
        {
          this.storage = value.Storage;
          this.Container = value.Storage.Containers.FirstOrDefault<IInventoryComponent>((Func<IInventoryComponent, bool>) (o => o.Owner.TemplateId == value.TemplateId));
        }
      }
    }

    public bool NeedSave => true;

    public void Use()
    {
      Action<IStorableComponent> useEvent = this.UseEvent;
      if (useEvent == null)
        return;
      useEvent((IStorableComponent) this);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      this.Durability.Set<float>(this.parameters?.GetByName<float>(ParameterNameEnum.Durability));
      this.CheckValue();
    }

    public override void OnRemoved()
    {
      if (this.storage != null)
      {
        this.storage.RemoveItem((IStorableComponent) this);
        this.storage = (IStorageComponent) null;
      }
      this.Durability.Set<float>((IParameter<float>) null);
      base.OnRemoved();
    }

    private void CheckValue()
    {
      if (this.count <= this.max)
        return;
      Debug.LogError((object) ("Count : " + (object) this.count + " , max : " + (object) this.max + " , owner : " + this.Owner.GetInfo()));
    }

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded()
    {
      Action<IStorableComponent> changeStorageEvent = this.ChangeStorageEvent;
      if (changeStorageEvent == null)
        return;
      changeStorageEvent((IStorableComponent) this);
    }
  }
}
