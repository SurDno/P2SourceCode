// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.InventoryComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Engine.Source.Inventory;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components
{
  [Required(typeof (ParametersComponent))]
  [Factory(typeof (IInventoryComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class InventoryComponent : 
    EngineComponent,
    IInventoryComponent,
    IComponent,
    IChangeParameterListener
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<InventoryContainerResource> containerResource;
    private InventoryContainerResource container;
    private ParametersComponent parametersComponent;

    private ParametersComponent ParametersComponent
    {
      get
      {
        if (this.parametersComponent == null)
          this.parametersComponent = this.Owner.GetComponent<ParametersComponent>();
        return this.parametersComponent;
      }
    }

    [Inspected]
    public IParameter<bool> Enabled
    {
      get => this.ParametersComponent.GetOrCreateByName<bool>(ParameterNameEnum.Enabled);
    }

    [Inspected]
    public IParameter<bool> Available
    {
      get => this.ParametersComponent.GetOrCreateByName<bool>(ParameterNameEnum.Available);
    }

    [Inspected]
    public IParameter<float> Disease
    {
      get => this.ParametersComponent.GetOrCreateByName<float>(ParameterNameEnum.Disease);
    }

    [Inspected]
    public IParameter<ContainerOpenStateEnum> OpenState
    {
      get
      {
        return this.ParametersComponent.GetOrCreateByName<ContainerOpenStateEnum>(ParameterNameEnum.OpenState);
      }
    }

    public IStorageComponent Storage { get; private set; }

    public Typed<IInventoryGridBase> TypedGrid => this.container.grid;

    public Sprite ImageBackground => this.container.imageBackground.Value;

    public Sprite ImageForeground => this.container.imageForeground.Value;

    public Sprite ImageInstrument => this.container.imageInstrument.Value;

    public Sprite ImageLock => this.container.imageLock.Value;

    public Sprite ImageNotAvailable => this.container.imageNotAvailable.Value;

    public IInventoryGridBase Grid => this.container.grid.Value;

    public ContainerCellKind Kind => this.container.kind;

    public Position Position => this.container.position;

    public Position Pivot => this.container.pivot;

    public Position Anchor => this.container.anchor;

    public SlotKind SlotKind => this.container.slotKind;

    public IEnumerable<StorableGroup> Limitations
    {
      get => (IEnumerable<StorableGroup>) this.container.limitations;
    }

    public IEnumerable<StorableGroup> Except => (IEnumerable<StorableGroup>) this.container.except;

    public InventoryGroup Group => this.container.group;

    public StorableGroup Instrument => this.container.instrument;

    public List<InventoryContainerOpenResource> OpenResources => this.container.openResources;

    public float Difficulty => this.container.difficulty;

    public float InstrumentDamage => this.container.instrumentDamage;

    public float OpenTime => this.container.openTime;

    public AudioClip OpenCompleteAudio => this.container.openCompleteAudio.Value;

    public AudioClip OpenProgressAudio => this.container.openProgressAudio.Value;

    public AudioClip OpenStartAudio => this.container.openStartAudio.Value;

    public AudioClip OpenCancelAudio => this.container.openCancelAudio.Value;

    public override void OnAdded()
    {
      base.OnAdded();
      this.container = this.containerResource.Value;
      this.Storage = this.Owner.Parent.GetComponent<IStorageComponent>();
      ((StorageComponent) this.Storage).AddContainer((IInventoryComponent) this);
      this.Enabled?.AddListener((IChangeParameterListener) this);
      this.Available?.AddListener((IChangeParameterListener) this);
      this.OpenState?.AddListener((IChangeParameterListener) this);
      this.Disease?.AddListener((IChangeParameterListener) this);
    }

    public override void OnRemoved()
    {
      this.Disease?.RemoveListener((IChangeParameterListener) this);
      this.OpenState?.RemoveListener((IChangeParameterListener) this);
      this.Available?.RemoveListener((IChangeParameterListener) this);
      this.Enabled?.RemoveListener((IChangeParameterListener) this);
      base.OnRemoved();
      ((StorageComponent) this.Storage).RemoveContainer((IInventoryComponent) this);
      this.Storage = (IStorageComponent) null;
    }

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded() => this.OnInvalidate();

    private void OnInvalidate()
    {
      if (this.Storage == null)
        return;
      ((StorageComponent) this.Storage).FireChangeInventoryEvent((IInventoryComponent) this);
    }

    public void OnParameterChanged(IParameter parameter) => this.OnInvalidate();
  }
}
