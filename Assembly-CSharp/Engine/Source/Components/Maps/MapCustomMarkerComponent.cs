// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Maps.MapCustomMarkerComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components.Maps
{
  [Factory(typeof (MapCustomMarkerComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class MapCustomMarkerComponent : EngineComponent, INeedSave, IEntityEventsListener
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool isEnabled = true;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
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
      get => this.isEnabled;
      set
      {
        this.isEnabled = value;
        this.OnChangeEnabled();
      }
    }

    public Vector2 Position
    {
      get => this.position;
      set
      {
        this.position = value;
        this.ApplyPosition();
      }
    }

    public bool NeedSave => true;

    private void ApplyPosition()
    {
      ((IEntityView) this.Owner).Position = new Vector3(this.position.x, 0.0f, this.position.y);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      ((Entity) this.Owner).AddListener((IEntityEventsListener) this);
      this.OnEnableChangedEvent();
    }

    public override void OnRemoved()
    {
      ((Entity) this.Owner).RemoveListener((IEntityEventsListener) this);
      this.RemoveFromMap();
      base.OnRemoved();
    }

    private void OnEnableChangedEvent()
    {
      if (this.Owner == null)
        return;
      if (this.Owner.IsEnabledInHierarchy && this.IsEnabled)
        this.AddToMap();
      else
        this.RemoveFromMap();
    }

    private void AddToMap()
    {
      if (this.added)
        return;
      this.added = true;
      if (this.mapService.CustomMarker != null)
        return;
      this.mapService.CustomMarker = this.Owner;
    }

    private void RemoveFromMap()
    {
      if (!this.added)
        return;
      this.added = false;
      if (this.mapService.CustomMarker != this.Owner)
        return;
      this.mapService.CustomMarker = (IEntity) null;
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      this.OnEnableChangedEvent();
    }

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded()
    {
      this.ApplyPosition();
      this.OnEnableChangedEvent();
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.EnableChangedEvent)
        return;
      this.OnEnableChangedEvent();
    }
  }
}
