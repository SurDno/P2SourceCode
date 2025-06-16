// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.NavigationComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
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
using System;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
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
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
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
      get => this.isEnabled;
      set
      {
        this.isEnabled = value;
        this.OnChangeEnabled();
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
    public IEntity SetupPoint => this.setupPoint;

    [Inspected]
    private IEntity TargetTeleport { get; set; }

    [Inspected]
    public bool WaitTeleport { get; private set; }

    [Inspected]
    public AreaEnum Area
    {
      get => this.areaKind;
      private set
      {
        if (this.areaKind == value)
          return;
        EventArgument<IEntity, AreaEnum> eventArguments = new EventArgument<IEntity, AreaEnum>()
        {
          Actor = this.Owner
        } with
        {
          Target = this.areaKind
        };
        AreaHandler exitAreaEvent = this.ExitAreaEvent;
        if (exitAreaEvent != null)
          exitAreaEvent(ref eventArguments);
        this.areaKind = value;
        eventArguments = new EventArgument<IEntity, AreaEnum>()
        {
          Actor = this.Owner
        } with
        {
          Target = value
        };
        AreaHandler enterAreaEvent = this.EnterAreaEvent;
        if (enterAreaEvent == null)
          return;
        enterAreaEvent(ref eventArguments);
      }
    }

    [Inspected]
    public IRegionComponent Region
    {
      get => this.region;
      private set
      {
        if (this.region == value)
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
          if (this.region != null)
          {
            EventArgument<IEntity, IRegionComponent> eventArguments = new EventArgument<IEntity, IRegionComponent>()
            {
              Actor = this.Owner
            } with
            {
              Target = this.region
            };
            RegionHandler exitRegionEvent = this.ExitRegionEvent;
            if (exitRegionEvent != null)
              exitRegionEvent(ref eventArguments);
          }
          this.region = value;
          EventArgument<IEntity, IRegionComponent> eventArguments1 = new EventArgument<IEntity, IRegionComponent>()
          {
            Actor = this.Owner
          } with
          {
            Target = value
          };
          RegionHandler enterRegionEvent = this.EnterRegionEvent;
          if (enterRegionEvent != null)
            enterRegionEvent(ref eventArguments1);
          if (this.WaitTeleport)
            return;
          ILocationComponent logicLocation = this.locationItem.LogicLocation;
          if (logicLocation == null || ((LocationComponent) logicLocation).LocationType == LocationType.Region)
            this.locationItem.Location = component;
        }
      }
    }

    [Inspected]
    public IBuildingComponent Building
    {
      get => this.building;
      private set
      {
        if (this.building == value)
          return;
        if (this.building != null)
        {
          EventArgument<IEntity, IBuildingComponent> eventArguments = new EventArgument<IEntity, IBuildingComponent>()
          {
            Actor = this.Owner
          } with
          {
            Target = this.building
          };
          BuildingHandler exitBuildingEvent = this.ExitBuildingEvent;
          if (exitBuildingEvent != null)
            exitBuildingEvent(ref eventArguments);
        }
        this.building = value;
        if (this.building == null)
          return;
        EventArgument<IEntity, IBuildingComponent> eventArguments1 = new EventArgument<IEntity, IBuildingComponent>()
        {
          Actor = this.Owner
        } with
        {
          Target = value
        };
        BuildingHandler enterBuildingEvent = this.EnterBuildingEvent;
        if (enterBuildingEvent != null)
          enterBuildingEvent(ref eventArguments1);
      }
    }

    public void TeleportTo(IEntity targetEntity)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  teleport to entity, owner : ").GetInfo((object) this.Owner).Append(" , target : ").GetInfo((object) targetEntity));
      if (targetEntity == null)
        throw new Exception("targetEntity == null : " + this.Owner.GetInfo());
      if (this.IsDisposed)
        throw new Exception("IsDisposed : " + this.Owner.GetInfo());
      if (!((Entity) this.Owner).IsAdded)
        throw new Exception("!((Entity)Owner).IsAdded : " + this.Owner.GetInfo());
      this.setupPoint = targetEntity;
      this.forceInvalidate = true;
      this.TargetTeleport = targetEntity;
      this.targetPosition = ((IEntityView) targetEntity).Position;
      this.targetRotation = ((IEntityView) targetEntity).Rotation;
      this.WaitTeleport = true;
      this.locationItem.Location = LocationItemUtility.GetLocation(targetEntity);
      this.LocationItem_OnChangeHibernation((ILocationItemComponent) this.locationItem);
    }

    public void TeleportTo(ILocationComponent location, Vector3 position, Quaternion rotation)
    {
      if (location == null)
        throw new Exception("ILocationComponent is null " + this.Owner.GetInfo());
      if (this.IsDisposed)
        throw new Exception("IsDisposed : " + this.Owner.GetInfo());
      if (!((Entity) this.Owner).IsAdded)
        throw new Exception("!((Entity)Owner).IsAdded : " + this.Owner.GetInfo());
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  teleport to location, owner : ").GetInfo((object) this.Owner).Append(" , location : ").GetInfo((object) location.Owner).Append(" , position : ").Append((object) position));
      this.setupPoint = location.Owner;
      this.forceInvalidate = true;
      this.TargetTeleport = (IEntity) null;
      this.targetPosition = position;
      this.targetRotation = rotation;
      this.WaitTeleport = true;
      this.locationItem.Location = location;
      this.LocationItem_OnChangeHibernation((ILocationItemComponent) this.locationItem);
    }

    private void CalculateArea()
    {
      NavMeshHit hit;
      if (NavMesh.SamplePosition(((IEntityView) this.Owner).Position, out hit, 2f, -1))
        this.Area = AreaEnumUtility.ToArea(hit.mask);
      else
        this.Area = AreaEnum.Unknown;
    }

    private void CalculateRegion()
    {
      this.Region = (IRegionComponent) RegionUtility.GetRegionByPosition(((IEntityView) this.Owner).Position);
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || this.locationItem.IsHibernation || !this.Owner.IsEnabledInHierarchy || !this.IsEnabled)
        return;
      this.ComputePosition();
    }

    private void ComputePosition()
    {
      Vector3 position = ((IEntityView) this.Owner).Position;
      if (position == this.storedPosition && !this.forceInvalidate)
        return;
      this.forceInvalidate = false;
      this.storedPosition = position;
      this.CalculateArea();
      this.CalculateRegion();
    }

    public override void OnAdded()
    {
      base.OnAdded();
      InstanceByRequest<UpdateService>.Instance.NavigationUpdater.AddUpdatable((IUpdatable) this);
      this.forceInvalidate = true;
      this.locationItem.OnHibernationChanged += new Action<ILocationItemComponent>(this.LocationItem_OnChangeHibernation);
      this.locationItem.OnChangeLocation += new Action<ILocationItemComponent, ILocationComponent>(this.LocationItem_OnChangeLocation);
    }

    public override void OnRemoved()
    {
      this.locationItem.OnChangeLocation -= new Action<ILocationItemComponent, ILocationComponent>(this.LocationItem_OnChangeLocation);
      this.locationItem.OnHibernationChanged -= new Action<ILocationItemComponent>(this.LocationItem_OnChangeHibernation);
      this.locationItem = (LocationItemComponent) null;
      InstanceByRequest<UpdateService>.Instance.NavigationUpdater.RemoveUpdatable((IUpdatable) this);
      base.OnRemoved();
    }

    private void LocationItem_OnChangeHibernation(ILocationItemComponent sender)
    {
      if (this.locationItem.IsHibernation)
        return;
      if (this.WaitTeleport)
      {
        if (this.TargetTeleport != null)
        {
          IEntity targetTeleport = this.TargetTeleport;
          this.TargetTeleport = (IEntity) null;
          IEntityView entityView = (IEntityView) targetTeleport;
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  teleport owner: ").GetInfo((object) this.Owner).Append(" , teleport point : ").GetInfo((object) targetTeleport).Append(" , position : ").Append((object) entityView));
          Action<INavigationComponent, IEntity> onPreTeleport = this.OnPreTeleport;
          if (onPreTeleport != null)
            onPreTeleport((INavigationComponent) this, targetTeleport);
          EntityViewUtility.SetTransformAndData(this.Owner, entityView.Position, entityView.Rotation, ((Entity) this.Owner).IsPlayer);
          this.targetPosition = entityView.Position;
          this.targetRotation = entityView.Rotation;
          Action<INavigationComponent, IEntity> onTeleport = this.OnTeleport;
          if (onTeleport != null)
            onTeleport((INavigationComponent) this, targetTeleport);
        }
        else
        {
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  teleport owner: ").GetInfo((object) this.Owner).Append(" , position : ").Append((object) this.targetPosition));
          Action<INavigationComponent, IEntity> onPreTeleport = this.OnPreTeleport;
          if (onPreTeleport != null)
            onPreTeleport((INavigationComponent) this, (IEntity) null);
          EntityViewUtility.SetTransformAndData(this.Owner, this.targetPosition, this.targetRotation, ((Entity) this.Owner).IsPlayer);
          Action<INavigationComponent, IEntity> onTeleport = this.OnTeleport;
          if (onTeleport != null)
            onTeleport((INavigationComponent) this, (IEntity) null);
        }
        this.WaitTeleport = false;
        this.HasPrevTeleport = true;
      }
      this.forceInvalidate = true;
      this.ComputePosition();
    }

    private void LocationItem_OnChangeLocation(
      ILocationItemComponent sender,
      ILocationComponent location)
    {
      ILocationComponent logicLocation = this.locationItem.LogicLocation;
      if (logicLocation != null)
      {
        IBuildingComponent component = logicLocation.GetComponent<IBuildingComponent>();
        if (component != null)
        {
          this.Building = component;
          return;
        }
      }
      this.Building = (IBuildingComponent) null;
    }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected TeleportData TeleportData
    {
      get
      {
        if (this.TargetTeleport != null)
        {
          TeleportData teleportData = ProxyFactory.Create<TeleportData>();
          teleportData.Location = (ILocationComponent) null;
          teleportData.Target = this.TargetTeleport;
          teleportData.Position = this.targetPosition;
          teleportData.Rotation = this.targetRotation;
          return teleportData;
        }
        if (this.locationItem.Location != null)
        {
          Vector3 vector3 = this.WaitTeleport ? this.targetPosition : ((IEntityView) this.Owner).Position;
          Quaternion quaternion = this.WaitTeleport ? this.targetRotation : ((IEntityView) this.Owner).Rotation;
          if (vector3 == Vector3.zero)
            Debug.LogError((object) ("Position is zero, owner : " + this.Owner.GetInfo()));
          TeleportData teleportData = ProxyFactory.Create<TeleportData>();
          teleportData.Location = this.locationItem.Location;
          teleportData.Target = (IEntity) null;
          teleportData.Position = vector3;
          teleportData.Rotation = quaternion;
          return teleportData;
        }
        Debug.LogError((object) ("Target and Location not found, owner : " + this.Owner.GetInfo()));
        return ProxyFactory.Create<TeleportData>();
      }
      set => this.storedTeleportData = value;
    }

    [Cofe.Serializations.Data.OnLoaded]
    protected void OnLoaded()
    {
      if (this.IsDisposed)
        return;
      IEntity setupPoint = this.setupPoint;
      TeleportData storedTeleportData = this.storedTeleportData;
      if (storedTeleportData != null)
      {
        if (storedTeleportData.Target != null)
          this.TeleportTo(storedTeleportData.Target);
        else if (storedTeleportData.Location != null)
          this.TeleportTo(storedTeleportData.Location, storedTeleportData.Position, storedTeleportData.Rotation);
        else
          Debug.LogError((object) ("Target and Location not found , owner" + this.Owner.GetInfo()));
      }
      else
        Debug.LogError((object) ("StoredTeleportData == null , owner" + this.Owner.GetInfo()));
      this.setupPoint = setupPoint;
    }

    public bool NeedSave => true;
  }
}
