using System;
using System.Collections.Generic;
using Cofe.Serializations.Data;
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
using UnityEngine;

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
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected Typed<IMapPlaceholder> placeholder;
    private bool added;
    private bool hudMarkerAdded;
    [StateSaveProxy(MemberEnum.CustomListReference)]
    [StateLoadProxy(MemberEnum.CustomListReference)]
    [Inspected]
    protected List<IMMNode> nodes = [];
    [StateSaveProxy(MemberEnum.CustomReference)]
    [StateLoadProxy(MemberEnum.CustomReference)]
    [Inspected]
    protected IMapTooltipResource tooltipResource;
    [StateSaveProxy]
    [StateLoadProxy]
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
    [StateSaveProxy]
    [StateLoadProxy()]
    [Inspected]
    protected LocalizedText title = LocalizedText.Empty;

    public static HashSet<MapItemComponent> Items { get; private set; } = [];

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

    [StateSaveProxy(MemberEnum.CustomReference)]
    [StateLoadProxy(MemberEnum.CustomReference)]
    [Inspected]
    public IEntity BoundCharacter { get; set; }

    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected]
    public LocalizedText Text { get; set; }

    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected]
    public LocalizedText TooltipText { get; set; }

    [Inspected]
    public IParameterValue<BoundHealthStateEnum> BoundHealthState { get; } = new ParameterValue<BoundHealthStateEnum>();

    [Inspected]
    public IParameterValue<bool> SavePointIcon { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> SleepIcon { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> CraftIcon { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> StorageIcon { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> MerchantIcon { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<FastTravelPointEnum> FastTravelPoint { get; } = new ParameterValue<FastTravelPointEnum>();

    public event Action DiscoveredChangeEvent;

    [Inspected(Mutable = true)]
    public bool Discovered
    {
      get => discovered;
      set
      {
        if (discovered == value || discovered && title != LocalizedText.Empty && region == null)
          return;
        discovered = value;
        Action discoveredChangeEvent = DiscoveredChangeEvent;
        if (discoveredChangeEvent == null)
          return;
        discoveredChangeEvent();
      }
    }

    public LocalizedText Title
    {
      get => title;
      set
      {
        title = value;
        if (!(title != LocalizedText.Empty))
          return;
        Discovered = true;
      }
    }

    public IMapPlaceholder Resource
    {
      get => placeholder.Value;
      set
      {
        placeholder.Value = value;
        if (value == null)
          return;
        if (InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
          notificationService.AddNotify(NotificationEnum.Map, []);
        Discovered = true;
      }
    }

    public IMapTooltipResource TooltipResource
    {
      get => tooltipResource;
      set
      {
        tooltipResource = value;
        if (value != null)
        {
          if (InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
            notificationService.AddNotify(NotificationEnum.Map, []);
          Discovered = true;
        }
        UpdateHUDMarker();
      }
    }

    MapPlaceholder IMapItem.Resource => (MapPlaceholder) placeholder.Value;

    public Vector2 WorldPosition
    {
      get
      {
        Vector3 position = ((IEntityView) Owner).Position;
        return new Vector2(position.x, position.z);
      }
    }

    public float Rotation => ((IEntityView) Owner).Rotation.eulerAngles.y;

    public IRegionComponent Region => region;

    public float Reputation => region != null ? region.Reputation.Value : 0.0f;

    public int Disease => region != null ? region.DiseaseLevel.Value : 0;

    public IEnumerable<IMMNode> Nodes => nodes;

    public bool NeedSave => true;

    public void AddNode(IMMNode node)
    {
      if (node == null)
      {
        Debug.LogError("Trying to add a null node to map item : " + Owner.GetInfo());
      }
      else
      {
        nodes.Remove(node);
        nodes.Add(node);
        Discovered = true;
        UpdateHUDMarker();
      }
    }

    public void RemoveNode(IMMNode node)
    {
      nodes.Remove(node);
      UpdateHUDMarker();
    }

    public void ClearNodes()
    {
      nodes.Clear();
      UpdateHUDMarker();
    }

    public override void OnAdded()
    {
      base.OnAdded();
      BoundHealthState.Set(parameters?.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState));
      SavePointIcon.Set(parameters?.GetByName<bool>(ParameterNameEnum.SavePointIcon));
      SleepIcon.Set(parameters?.GetByName<bool>(ParameterNameEnum.SleepIcon));
      CraftIcon.Set(parameters?.GetByName<bool>(ParameterNameEnum.CraftIcon));
      StorageIcon.Set(parameters?.GetByName<bool>(ParameterNameEnum.StorageIcon));
      MerchantIcon.Set(parameters?.GetByName<bool>(ParameterNameEnum.MerchantIcon));
      FastTravelPoint.Set(parameters?.GetByName<FastTravelPointEnum>(ParameterNameEnum.FastTravelPointIndex));
      ((Entity) Owner).AddListener(this);
      OnEnableChangedEvent();
      Items.Add(this);
    }

    public override void OnRemoved()
    {
      Items.Remove(this);
      ((Entity) Owner).RemoveListener(this);
      RemoveFromMap();
      parameters = null;
      BoundHealthState.Set(null);
      SavePointIcon.Set(null);
      SleepIcon.Set(null);
      CraftIcon.Set(null);
      StorageIcon.Set(null);
      MerchantIcon.Set(null);
      FastTravelPoint.Set(null);
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
      RemoveFromMap();
      if (added)
        return;
      added = true;
      mapService.AddMapItem(this);
      UpdateHUDMarker();
    }

    private void RemoveFromMap()
    {
      if (!added)
        return;
      added = false;
      mapService.RemoveMapItem(this);
      UpdateHUDMarker();
    }

    private void UpdateHUDMarker()
    {
      bool flag = added && (nodes.Count > 0 || tooltipResource != null);
      if (flag == hudMarkerAdded)
        return;
      hudMarkerAdded = flag;
      if (hudMarkerAdded)
        mapService.AddHUDItem(this);
      else
        mapService.RemoveHUDItem(this);
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      OnEnableChangedEvent();
    }

    [OnLoaded]
    private void OnLoaded() => OnEnableChangedEvent();

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.EnableChangedEvent)
        return;
      OnEnableChangedEvent();
    }
  }
}
