using System.Collections.Generic;
using Cofe.Serializations.Data;
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
using UnityEngine;

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
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<InventoryContainerResource> containerResource;
    private InventoryContainerResource container;
    private ParametersComponent parametersComponent;

    private ParametersComponent ParametersComponent
    {
      get
      {
        if (parametersComponent == null)
          parametersComponent = Owner.GetComponent<ParametersComponent>();
        return parametersComponent;
      }
    }

    [Inspected]
    public IParameter<bool> Enabled
    {
      get => ParametersComponent.GetOrCreateByName<bool>(ParameterNameEnum.Enabled);
    }

    [Inspected]
    public IParameter<bool> Available
    {
      get => ParametersComponent.GetOrCreateByName<bool>(ParameterNameEnum.Available);
    }

    [Inspected]
    public IParameter<float> Disease
    {
      get => ParametersComponent.GetOrCreateByName<float>(ParameterNameEnum.Disease);
    }

    [Inspected]
    public IParameter<ContainerOpenStateEnum> OpenState
    {
      get
      {
        return ParametersComponent.GetOrCreateByName<ContainerOpenStateEnum>(ParameterNameEnum.OpenState);
      }
    }

    public IStorageComponent Storage { get; private set; }

    public Typed<IInventoryGridBase> TypedGrid => container.grid;

    public Sprite ImageBackground => container.imageBackground.Value;

    public Sprite ImageForeground => container.imageForeground.Value;

    public Sprite ImageInstrument => container.imageInstrument.Value;

    public Sprite ImageLock => container.imageLock.Value;

    public Sprite ImageNotAvailable => container.imageNotAvailable.Value;

    public IInventoryGridBase Grid => container.grid.Value;

    public ContainerCellKind Kind => container.kind;

    public Position Position => container.position;

    public Position Pivot => container.pivot;

    public Position Anchor => container.anchor;

    public SlotKind SlotKind => container.slotKind;

    public IEnumerable<StorableGroup> Limitations
    {
      get => container.limitations;
    }

    public IEnumerable<StorableGroup> Except => container.except;

    public InventoryGroup Group => container.group;

    public StorableGroup Instrument => container.instrument;

    public List<InventoryContainerOpenResource> OpenResources => container.openResources;

    public float Difficulty => container.difficulty;

    public float InstrumentDamage => container.instrumentDamage;

    public float OpenTime => container.openTime;

    public AudioClip OpenCompleteAudio => container.openCompleteAudio.Value;

    public AudioClip OpenProgressAudio => container.openProgressAudio.Value;

    public AudioClip OpenStartAudio => container.openStartAudio.Value;

    public AudioClip OpenCancelAudio => container.openCancelAudio.Value;

    public override void OnAdded()
    {
      base.OnAdded();
      container = containerResource.Value;
      Storage = Owner.Parent.GetComponent<IStorageComponent>();
      ((StorageComponent) Storage).AddContainer(this);
      Enabled?.AddListener(this);
      Available?.AddListener(this);
      OpenState?.AddListener(this);
      Disease?.AddListener(this);
    }

    public override void OnRemoved()
    {
      Disease?.RemoveListener(this);
      OpenState?.RemoveListener(this);
      Available?.RemoveListener(this);
      Enabled?.RemoveListener(this);
      base.OnRemoved();
      ((StorageComponent) Storage).RemoveContainer(this);
      Storage = null;
    }

    [OnLoaded]
    private void OnLoaded() => OnInvalidate();

    private void OnInvalidate()
    {
      if (Storage == null)
        return;
      ((StorageComponent) Storage).FireChangeInventoryEvent(this);
    }

    public void OnParameterChanged(IParameter parameter) => OnInvalidate();
  }
}
