using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Locations;
using Engine.Common.Components.Movable;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components.Regions;
using Engine.Source.Components.Saves;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Components
{
  [Required(typeof (LocationItemComponent))]
  [Factory(typeof (INavigationComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class NavigationComponent : 
    EngineComponent,
    INavigationComponent,
    IComponent,
    IUpdatable,
    INeedSave
  {
    [StateSaveProxy]
    [StateLoadProxy]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool isEnabled = true;
    private AreaEnum areaKind;
    private IRegionComponent region;
    private IBuildingComponent building;
    [StateSaveProxy(MemberEnum.CustomReference)]
    [StateLoadProxy(MemberEnum.CustomReference)]
    protected IEntity setupPoint;
    private TeleportData storedTeleportData;
    [FromThis]
    private LocationItemComponent locationItem;
    [FromLocator]
    private ISimulation simulation;
    [Inspected]
    private bool forceInvalidate;
    [Inspected]
    private Vector3 storedPosition;
    [Inspected]
    private Vector3 targetPosition;
    [Inspected]
    private Quaternion targetRotation;

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

    [Inspected]
    public bool HasPrevTeleport { get; private set; }

    public event AreaHandler EnterAreaEvent;

    public event AreaHandler ExitAreaEvent;

    public event BuildingHandler EnterBuildingEvent;

    public event BuildingHandler ExitBuildingEvent;

    public event RegionHandler EnterRegionEvent;

    public event RegionHandler ExitRegionEvent;

    public event Action<INavigationComponent, IEntity> OnPreTeleport;

    public event Action<INavigationComponent, IEntity> OnTeleport;

    [Inspected]
    public IEntity SetupPoint => setupPoint;

    [Inspected]
    private IEntity TargetTeleport { get; set; }

    [Inspected]
    public bool WaitTeleport { get; private set; }

    [Inspected]
    public AreaEnum Area
    {
      get => areaKind;
      private set
      {
        if (areaKind == value)
          return;
        EventArgument<IEntity, AreaEnum> eventArguments = new EventArgument<IEntity, AreaEnum> {
          Actor = Owner
        } with
        {
          Target = areaKind
        };
        AreaHandler exitAreaEvent = ExitAreaEvent;
        if (exitAreaEvent != null)
          exitAreaEvent(ref eventArguments);
        areaKind = value;
        eventArguments = new EventArgument<IEntity, AreaEnum> {
          Actor = Owner
        } with
        {
          Target = value
        };
        AreaHandler enterAreaEvent = EnterAreaEvent;
        if (enterAreaEvent == null)
          return;
        enterAreaEvent(ref eventArguments);
      }
    }

    [Inspected]
    public IRegionComponent Region
    {
      get => region;
      private set
      {
        if (region == value)
          return;
        if (value == null)
        {
          Debug.LogError((object) "Region position == null");
        }
        else
        {
          ILocationComponent component = value.GetComponent<ILocationComponent>();
          if (component.IsHibernation)
            return;
          if (region != null)
          {
            EventArgument<IEntity, IRegionComponent> eventArguments = new EventArgument<IEntity, IRegionComponent> {
              Actor = Owner
            } with
            {
              Target = region
            };
            RegionHandler exitRegionEvent = ExitRegionEvent;
            if (exitRegionEvent != null)
              exitRegionEvent(ref eventArguments);
          }
          region = value;
          EventArgument<IEntity, IRegionComponent> eventArguments1 = new EventArgument<IEntity, IRegionComponent> {
            Actor = Owner
          } with
          {
            Target = value
          };
          RegionHandler enterRegionEvent = EnterRegionEvent;
          if (enterRegionEvent != null)
            enterRegionEvent(ref eventArguments1);
          if (WaitTeleport)
            return;
          ILocationComponent logicLocation = locationItem.LogicLocation;
          if (logicLocation == null || ((LocationComponent) logicLocation).LocationType == LocationType.Region)
            locationItem.Location = component;
        }
      }
    }

    [Inspected]
    public IBuildingComponent Building
    {
      get => building;
      private set
      {
        if (building == value)
          return;
        if (building != null)
        {
          EventArgument<IEntity, IBuildingComponent> eventArguments = new EventArgument<IEntity, IBuildingComponent> {
            Actor = Owner
          } with
          {
            Target = building
          };
          BuildingHandler exitBuildingEvent = ExitBuildingEvent;
          if (exitBuildingEvent != null)
            exitBuildingEvent(ref eventArguments);
        }
        building = value;
        if (building == null)
          return;
        EventArgument<IEntity, IBuildingComponent> eventArguments1 = new EventArgument<IEntity, IBuildingComponent> {
          Actor = Owner
        } with
        {
          Target = value
        };
        BuildingHandler enterBuildingEvent = EnterBuildingEvent;
        if (enterBuildingEvent != null)
          enterBuildingEvent(ref eventArguments1);
      }
    }

    public void TeleportTo(IEntity targetEntity)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  teleport to entity, owner : ").GetInfo(Owner).Append(" , target : ").GetInfo(targetEntity));
      if (targetEntity == null)
        throw new Exception("targetEntity == null : " + Owner.GetInfo());
      if (IsDisposed)
        throw new Exception("IsDisposed : " + Owner.GetInfo());
      if (!((Entity) Owner).IsAdded)
        throw new Exception("!((Entity)Owner).IsAdded : " + Owner.GetInfo());
      setupPoint = targetEntity;
      forceInvalidate = true;
      TargetTeleport = targetEntity;
      targetPosition = ((IEntityView) targetEntity).Position;
      targetRotation = ((IEntityView) targetEntity).Rotation;
      WaitTeleport = true;
      locationItem.Location = LocationItemUtility.GetLocation(targetEntity);
      LocationItem_OnChangeHibernation(locationItem);
    }

    public void TeleportTo(ILocationComponent location, Vector3 position, Quaternion rotation)
    {
      if (location == null)
        throw new Exception("ILocationComponent is null " + Owner.GetInfo());
      if (IsDisposed)
        throw new Exception("IsDisposed : " + Owner.GetInfo());
      if (!((Entity) Owner).IsAdded)
        throw new Exception("!((Entity)Owner).IsAdded : " + Owner.GetInfo());
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  teleport to location, owner : ").GetInfo(Owner).Append(" , location : ").GetInfo(location.Owner).Append(" , position : ").Append((object) position));
      setupPoint = location.Owner;
      forceInvalidate = true;
      TargetTeleport = null;
      targetPosition = position;
      targetRotation = rotation;
      WaitTeleport = true;
      locationItem.Location = location;
      LocationItem_OnChangeHibernation(locationItem);
    }

    private void CalculateArea()
    {
      NavMeshHit hit;
      if (NavMesh.SamplePosition(((IEntityView) Owner).Position, out hit, 2f, -1))
        Area = AreaEnumUtility.ToArea(hit.mask);
      else
        Area = AreaEnum.Unknown;
    }

    private void CalculateRegion()
    {
      Region = RegionUtility.GetRegionByPosition(((IEntityView) Owner).Position);
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || locationItem.IsHibernation || !Owner.IsEnabledInHierarchy || !IsEnabled)
        return;
      ComputePosition();
    }

    private void ComputePosition()
    {
      Vector3 position = ((IEntityView) Owner).Position;
      if (position == storedPosition && !forceInvalidate)
        return;
      forceInvalidate = false;
      storedPosition = position;
      CalculateArea();
      CalculateRegion();
    }

    public override void OnAdded()
    {
      base.OnAdded();
      InstanceByRequest<UpdateService>.Instance.NavigationUpdater.AddUpdatable(this);
      forceInvalidate = true;
      locationItem.OnHibernationChanged += LocationItem_OnChangeHibernation;
      locationItem.OnChangeLocation += LocationItem_OnChangeLocation;
    }

    public override void OnRemoved()
    {
      locationItem.OnChangeLocation -= LocationItem_OnChangeLocation;
      locationItem.OnHibernationChanged -= LocationItem_OnChangeHibernation;
      locationItem = null;
      InstanceByRequest<UpdateService>.Instance.NavigationUpdater.RemoveUpdatable(this);
      base.OnRemoved();
    }

    private void LocationItem_OnChangeHibernation(ILocationItemComponent sender)
    {
      if (locationItem.IsHibernation)
        return;
      if (WaitTeleport)
      {
        if (TargetTeleport != null)
        {
          IEntity targetTeleport = TargetTeleport;
          TargetTeleport = null;
          IEntityView entityView = (IEntityView) targetTeleport;
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  teleport owner: ").GetInfo(Owner).Append(" , teleport point : ").GetInfo(targetTeleport).Append(" , position : ").Append(entityView));
          Action<INavigationComponent, IEntity> onPreTeleport = OnPreTeleport;
          if (onPreTeleport != null)
            onPreTeleport(this, targetTeleport);
          EntityViewUtility.SetTransformAndData(Owner, entityView.Position, entityView.Rotation, ((Entity) Owner).IsPlayer);
          targetPosition = entityView.Position;
          targetRotation = entityView.Rotation;
          Action<INavigationComponent, IEntity> onTeleport = OnTeleport;
          if (onTeleport != null)
            onTeleport(this, targetTeleport);
        }
        else
        {
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  teleport owner: ").GetInfo(Owner).Append(" , position : ").Append((object) targetPosition));
          Action<INavigationComponent, IEntity> onPreTeleport = OnPreTeleport;
          if (onPreTeleport != null)
            onPreTeleport(this, null);
          EntityViewUtility.SetTransformAndData(Owner, targetPosition, targetRotation, ((Entity) Owner).IsPlayer);
          Action<INavigationComponent, IEntity> onTeleport = OnTeleport;
          if (onTeleport != null)
            onTeleport(this, null);
        }
        WaitTeleport = false;
        HasPrevTeleport = true;
      }
      forceInvalidate = true;
      ComputePosition();
    }

    private void LocationItem_OnChangeLocation(
      ILocationItemComponent sender,
      ILocationComponent location)
    {
      ILocationComponent logicLocation = locationItem.LogicLocation;
      if (logicLocation != null)
      {
        IBuildingComponent component = logicLocation.GetComponent<IBuildingComponent>();
        if (component != null)
        {
          Building = component;
          return;
        }
      }
      Building = null;
    }

    [StateSaveProxy]
    [StateLoadProxy]
    protected TeleportData TeleportData
    {
      get
      {
        if (TargetTeleport != null)
        {
          TeleportData teleportData = ProxyFactory.Create<TeleportData>();
          teleportData.Location = null;
          teleportData.Target = TargetTeleport;
          teleportData.Position = targetPosition;
          teleportData.Rotation = targetRotation;
          return teleportData;
        }
        if (locationItem.Location != null)
        {
          Vector3 vector3 = WaitTeleport ? targetPosition : ((IEntityView) Owner).Position;
          Quaternion quaternion = WaitTeleport ? targetRotation : ((IEntityView) Owner).Rotation;
          if (vector3 == Vector3.zero)
            Debug.LogError((object) ("Position is zero, owner : " + Owner.GetInfo()));
          TeleportData teleportData = ProxyFactory.Create<TeleportData>();
          teleportData.Location = locationItem.Location;
          teleportData.Target = null;
          teleportData.Position = vector3;
          teleportData.Rotation = quaternion;
          return teleportData;
        }
        Debug.LogError((object) ("Target and Location not found, owner : " + Owner.GetInfo()));
        return ProxyFactory.Create<TeleportData>();
      }
      set => storedTeleportData = value;
    }

    [OnLoaded]
    protected void OnLoaded()
    {
      if (IsDisposed)
        return;
      IEntity setupPoint = this.setupPoint;
      TeleportData storedTeleportData = this.storedTeleportData;
      if (storedTeleportData != null)
      {
        if (storedTeleportData.Target != null)
          TeleportTo(storedTeleportData.Target);
        else if (storedTeleportData.Location != null)
          TeleportTo(storedTeleportData.Location, storedTeleportData.Position, storedTeleportData.Rotation);
        else
          Debug.LogError((object) ("Target and Location not found , owner" + Owner.GetInfo()));
      }
      else
        Debug.LogError((object) ("StoredTeleportData == null , owner" + Owner.GetInfo()));
      this.setupPoint = setupPoint;
    }

    public bool NeedSave => true;
  }
}
