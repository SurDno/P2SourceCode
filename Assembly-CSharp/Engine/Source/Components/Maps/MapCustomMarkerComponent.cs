using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Components.Maps
{
  [Factory(typeof (MapCustomMarkerComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class MapCustomMarkerComponent : EngineComponent, INeedSave, IEntityEventsListener
  {
    [StateSaveProxy]
    [StateLoadProxy]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool isEnabled = true;
    [StateSaveProxy]
    [StateLoadProxy()]
    [Inspected]
    protected Vector2 position = Vector2.zero;
    private bool added;
    [FromThis]
    private PositionComponent positionComponent;
    [FromLocator]
    private MapService mapService;

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

    public Vector2 Position
    {
      get => position;
      set
      {
        position = value;
        ApplyPosition();
      }
    }

    public bool NeedSave => true;

    private void ApplyPosition()
    {
      ((IEntityView) Owner).Position = new Vector3(position.x, 0.0f, position.y);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      ((Entity) Owner).AddListener(this);
      OnEnableChangedEvent();
    }

    public override void OnRemoved()
    {
      ((Entity) Owner).RemoveListener(this);
      RemoveFromMap();
      base.OnRemoved();
    }

    private void OnEnableChangedEvent()
    {
      if (Owner == null)
        return;
      if (Owner.IsEnabledInHierarchy && IsEnabled)
        AddToMap();
      else
        RemoveFromMap();
    }

    private void AddToMap()
    {
      if (added)
        return;
      added = true;
      if (mapService.CustomMarker != null)
        return;
      mapService.CustomMarker = Owner;
    }

    private void RemoveFromMap()
    {
      if (!added)
        return;
      added = false;
      if (mapService.CustomMarker != Owner)
        return;
      mapService.CustomMarker = null;
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      OnEnableChangedEvent();
    }

    [OnLoaded]
    private void OnLoaded()
    {
      ApplyPosition();
      OnEnableChangedEvent();
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.EnableChangedEvent)
        return;
      OnEnableChangedEvent();
    }
  }
}
