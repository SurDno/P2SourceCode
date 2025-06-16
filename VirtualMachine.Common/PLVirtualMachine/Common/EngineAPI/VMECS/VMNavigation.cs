using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Movable;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Position", typeof (INavigationComponent))]
  public class VMNavigation : VMEngineComponent<INavigationComponent>
  {
    public const string ComponentName = "Position";

    [Event("Leave region event", "Регион:Region")]
    public event Action<IEntity> LeaveRegionEvent;

    [Event("Arrive region event", "Регион:Region")]
    public event Action<IEntity> ArrivedRegionEvent;

    [Event("Leave area event", "Тип области")]
    public event Action<AreaEnum> LeaveAreaEvent;

    [Event("Arrive area event", "Тип области")]
    public event Action<AreaEnum> ArrivedAreaEvent;

    [Event("Leave building event", "Building")]
    public event Action<IEntity> LeaveBuildingEvent;

    [Event("Arrive building event", "Building")]
    public event Action<IEntity> ArrivedBuildingEvent;

    [Method("GetRegion", "", "Region")]
    public IEntity GetRegion()
    {
      return this.Component.Region == null ? (IEntity) null : this.Component.Region.Owner;
    }

    [Method("GetBuilding", "", "Building")]
    public IEntity GetBuilding()
    {
      return this.Component.Building == null ? (IEntity) null : this.Component.Building.Owner;
    }

    [Method("GetAreaType", "", "")]
    public AreaEnum GetAreaType() => this.Component.Area;

    public void OnLeaveRegion(
      ref EventArgument<IEntity, IRegionComponent> eventArgs)
    {
      IRegionComponent target = eventArgs.Target;
      if (target == null || target.IsDisposed)
        return;
      this.LeaveRegionEvent(target.Owner);
    }

    public void OnEnterRegion(
      ref EventArgument<IEntity, IRegionComponent> eventArgs)
    {
      if (this.ArrivedRegionEvent == null)
        return;
      this.ArrivedRegionEvent(eventArgs.Target?.Owner);
    }

    public void OnLeaveArea(
      ref EventArgument<IEntity, AreaEnum> EventArguments)
    {
      if (this.LeaveAreaEvent == null)
        return;
      this.LeaveAreaEvent(EventArguments.Target);
    }

    public void OnEnterArea(
      ref EventArgument<IEntity, AreaEnum> EventArguments)
    {
      if (this.ArrivedAreaEvent == null)
        return;
      this.ArrivedAreaEvent(EventArguments.Target);
    }

    private void EnterBuildingEvent(
      ref EventArgument<IEntity, IBuildingComponent> eventArguments)
    {
      if (this.ArrivedBuildingEvent == null)
        return;
      this.ArrivedBuildingEvent(eventArguments.Target?.Owner);
    }

    private void ExitBuildingEvent(
      ref EventArgument<IEntity, IBuildingComponent> eventArguments)
    {
      IBuildingComponent target = eventArguments.Target;
      if (target == null || target.IsDisposed)
        return;
      this.LeaveBuildingEvent(target.Owner);
    }

    [Method("Teleport to", "Target,Area", "")]
    public void TeleportToArea(IEntity target, AreaEnum NOT_USED_area)
    {
      if (this.Parent != null)
      {
        if (this.Parent.Instance != null)
        {
          if (!this.Parent.IsWorldEntity)
            return;
          this.Component.TeleportTo(target);
        }
        else
          Logger.AddError(string.Format("Position parent engine entity not defined !"));
      }
      else
        Logger.AddError(string.Format("Position parent entity not defined !"));
    }

    [Method("Teleport to", "Target", "")]
    public void TeleportTo(IEntity target)
    {
      if (this.Parent != null)
      {
        if (this.Parent.Instance != null)
        {
          if (!this.Parent.IsWorldEntity)
            return;
          if (target == null)
            Logger.AddError(string.Format("Teleport position for entity {0} is null at {1}!", (object) this.Parent.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          else
            this.Component.TeleportTo(target);
        }
        else
          Logger.AddError(string.Format("Position parent engine entity not defined !"));
      }
      else
        Logger.AddError(string.Format("Position parent entity not defined !"));
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.ExitRegionEvent -= new RegionHandler(this.OnLeaveRegion);
      this.Component.EnterRegionEvent -= new RegionHandler(this.OnEnterRegion);
      this.Component.ExitAreaEvent -= new AreaHandler(this.OnLeaveArea);
      this.Component.EnterAreaEvent -= new AreaHandler(this.OnEnterArea);
      this.Component.EnterBuildingEvent -= new BuildingHandler(this.EnterBuildingEvent);
      this.Component.ExitBuildingEvent -= new BuildingHandler(this.ExitBuildingEvent);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.ExitRegionEvent += new RegionHandler(this.OnLeaveRegion);
      this.Component.EnterRegionEvent += new RegionHandler(this.OnEnterRegion);
      this.Component.ExitAreaEvent += new AreaHandler(this.OnLeaveArea);
      this.Component.EnterAreaEvent += new AreaHandler(this.OnEnterArea);
      this.Component.EnterBuildingEvent += new BuildingHandler(this.EnterBuildingEvent);
      this.Component.ExitBuildingEvent += new BuildingHandler(this.ExitBuildingEvent);
    }
  }
}
