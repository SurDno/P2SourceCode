// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Maps.MapItemComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;
using Engine.Common.Generator;
using Engine.Common.Maps;
using Engine.Common.MindMap;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Connections;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components.Maps
{
  [Required(typeof (PositionComponent))]
  [Factory(typeof (IMapItemComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class MapItemComponent : 
    EngineComponent,
    IMapItemComponent,
    IComponent,
    IMapItem,
    INeedSave,
    IEntityEventsListener
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
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<IMapPlaceholder> placeholder;
    private bool added;
    private bool hudMarkerAdded;
    [StateSaveProxy(MemberEnum.CustomListReference)]
    [StateLoadProxy(MemberEnum.CustomListReference)]
    [Inspected]
    protected List<IMMNode> nodes = new List<IMMNode>();
    [StateSaveProxy(MemberEnum.CustomReference)]
    [StateLoadProxy(MemberEnum.CustomReference)]
    [Inspected]
    protected IMapTooltipResource tooltipResource;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected bool discovered;
    [FromThis]
    private IRegionComponent region;
    [FromThis]
    private ParametersComponent parameters;
    [FromThis]
    private PositionComponent positionComponent;
    [FromLocator]
    private NotificationService notificationService;
    [FromLocator]
    private MapService mapService;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    protected LocalizedText title = LocalizedText.Empty;

    public static HashSet<MapItemComponent> Items { get; private set; } = new HashSet<MapItemComponent>();

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

    [StateSaveProxy(MemberEnum.CustomReference)]
    [StateLoadProxy(MemberEnum.CustomReference)]
    [Inspected]
    public IEntity BoundCharacter { get; set; }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public LocalizedText Text { get; set; }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public LocalizedText TooltipText { get; set; }

    [Inspected]
    public IParameterValue<BoundHealthStateEnum> BoundHealthState { get; } = (IParameterValue<BoundHealthStateEnum>) new ParameterValue<BoundHealthStateEnum>();

    [Inspected]
    public IParameterValue<bool> SavePointIcon { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> SleepIcon { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> CraftIcon { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> StorageIcon { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> MerchantIcon { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<FastTravelPointEnum> FastTravelPoint { get; } = (IParameterValue<FastTravelPointEnum>) new ParameterValue<FastTravelPointEnum>();

    public event Action DiscoveredChangeEvent;

    [Inspected(Mutable = true)]
    public bool Discovered
    {
      get => this.discovered;
      set
      {
        if (this.discovered == value || this.discovered && this.title != LocalizedText.Empty && this.region == null)
          return;
        this.discovered = value;
        Action discoveredChangeEvent = this.DiscoveredChangeEvent;
        if (discoveredChangeEvent == null)
          return;
        discoveredChangeEvent();
      }
    }

    public LocalizedText Title
    {
      get => this.title;
      set
      {
        this.title = value;
        if (!(this.title != LocalizedText.Empty))
          return;
        this.Discovered = true;
      }
    }

    public IMapPlaceholder Resource
    {
      get => this.placeholder.Value;
      set
      {
        this.placeholder.Value = value;
        if (value == null)
          return;
        if (InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
          this.notificationService.AddNotify(NotificationEnum.Map, Array.Empty<object>());
        this.Discovered = true;
      }
    }

    public IMapTooltipResource TooltipResource
    {
      get => this.tooltipResource;
      set
      {
        this.tooltipResource = value;
        if (value != null)
        {
          if (InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
            this.notificationService.AddNotify(NotificationEnum.Map, Array.Empty<object>());
          this.Discovered = true;
        }
        this.UpdateHUDMarker();
      }
    }

    MapPlaceholder IMapItem.Resource => (MapPlaceholder) this.placeholder.Value;

    public Vector2 WorldPosition
    {
      get
      {
        Vector3 position = ((IEntityView) this.Owner).Position;
        return new Vector2(position.x, position.z);
      }
    }

    public float Rotation => ((IEntityView) this.Owner).Rotation.eulerAngles.y;

    public IRegionComponent Region => this.region;

    public float Reputation => this.region != null ? this.region.Reputation.Value : 0.0f;

    public int Disease => this.region != null ? this.region.DiseaseLevel.Value : 0;

    public IEnumerable<IMMNode> Nodes => (IEnumerable<IMMNode>) this.nodes;

    public bool NeedSave => true;

    public void AddNode(IMMNode node)
    {
      if (node == null)
      {
        Debug.LogError((object) ("Trying to add a null node to map item : " + this.Owner.GetInfo()));
      }
      else
      {
        this.nodes.Remove(node);
        this.nodes.Add(node);
        this.Discovered = true;
        this.UpdateHUDMarker();
      }
    }

    public void RemoveNode(IMMNode node)
    {
      this.nodes.Remove(node);
      this.UpdateHUDMarker();
    }

    public void ClearNodes()
    {
      this.nodes.Clear();
      this.UpdateHUDMarker();
    }

    public override void OnAdded()
    {
      base.OnAdded();
      this.BoundHealthState.Set<BoundHealthStateEnum>(this.parameters?.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState));
      this.SavePointIcon.Set<bool>(this.parameters?.GetByName<bool>(ParameterNameEnum.SavePointIcon));
      this.SleepIcon.Set<bool>(this.parameters?.GetByName<bool>(ParameterNameEnum.SleepIcon));
      this.CraftIcon.Set<bool>(this.parameters?.GetByName<bool>(ParameterNameEnum.CraftIcon));
      this.StorageIcon.Set<bool>(this.parameters?.GetByName<bool>(ParameterNameEnum.StorageIcon));
      this.MerchantIcon.Set<bool>(this.parameters?.GetByName<bool>(ParameterNameEnum.MerchantIcon));
      this.FastTravelPoint.Set<FastTravelPointEnum>(this.parameters?.GetByName<FastTravelPointEnum>(ParameterNameEnum.FastTravelPointIndex));
      ((Entity) this.Owner).AddListener((IEntityEventsListener) this);
      this.OnEnableChangedEvent();
      MapItemComponent.Items.Add(this);
    }

    public override void OnRemoved()
    {
      MapItemComponent.Items.Remove(this);
      ((Entity) this.Owner).RemoveListener((IEntityEventsListener) this);
      this.RemoveFromMap();
      this.parameters = (ParametersComponent) null;
      this.BoundHealthState.Set<BoundHealthStateEnum>((IParameter<BoundHealthStateEnum>) null);
      this.SavePointIcon.Set<bool>((IParameter<bool>) null);
      this.SleepIcon.Set<bool>((IParameter<bool>) null);
      this.CraftIcon.Set<bool>((IParameter<bool>) null);
      this.StorageIcon.Set<bool>((IParameter<bool>) null);
      this.MerchantIcon.Set<bool>((IParameter<bool>) null);
      this.FastTravelPoint.Set<FastTravelPointEnum>((IParameter<FastTravelPointEnum>) null);
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
      this.RemoveFromMap();
      if (this.added)
        return;
      this.added = true;
      this.mapService.AddMapItem((IMapItem) this);
      this.UpdateHUDMarker();
    }

    private void RemoveFromMap()
    {
      if (!this.added)
        return;
      this.added = false;
      this.mapService.RemoveMapItem((IMapItem) this);
      this.UpdateHUDMarker();
    }

    private void UpdateHUDMarker()
    {
      bool flag = this.added && (this.nodes.Count > 0 || this.tooltipResource != null);
      if (flag == this.hudMarkerAdded)
        return;
      this.hudMarkerAdded = flag;
      if (this.hudMarkerAdded)
        this.mapService.AddHUDItem((IMapItem) this);
      else
        this.mapService.RemoveHUDItem((IMapItem) this);
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      this.OnEnableChangedEvent();
    }

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded() => this.OnEnableChangedEvent();

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.EnableChangedEvent)
        return;
      this.OnEnableChangedEvent();
    }
  }
}
