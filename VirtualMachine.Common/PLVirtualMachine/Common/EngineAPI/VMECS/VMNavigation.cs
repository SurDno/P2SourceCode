using System;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Movable;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("Position", typeof(INavigationComponent))]
public class VMNavigation : VMEngineComponent<INavigationComponent> {
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
	public IEntity GetRegion() {
		return Component.Region == null ? null : Component.Region.Owner;
	}

	[Method("GetBuilding", "", "Building")]
	public IEntity GetBuilding() {
		return Component.Building == null ? null : Component.Building.Owner;
	}

	[Method("GetAreaType", "", "")]
	public AreaEnum GetAreaType() {
		return Component.Area;
	}

	public void OnLeaveRegion(
		ref EventArgument<IEntity, IRegionComponent> eventArgs) {
		var target = eventArgs.Target;
		if (target == null || target.IsDisposed)
			return;
		LeaveRegionEvent(target.Owner);
	}

	public void OnEnterRegion(
		ref EventArgument<IEntity, IRegionComponent> eventArgs) {
		if (ArrivedRegionEvent == null)
			return;
		ArrivedRegionEvent(eventArgs.Target?.Owner);
	}

	public void OnLeaveArea(
		ref EventArgument<IEntity, AreaEnum> EventArguments) {
		if (LeaveAreaEvent == null)
			return;
		LeaveAreaEvent(EventArguments.Target);
	}

	public void OnEnterArea(
		ref EventArgument<IEntity, AreaEnum> EventArguments) {
		if (ArrivedAreaEvent == null)
			return;
		ArrivedAreaEvent(EventArguments.Target);
	}

	private void EnterBuildingEvent(
		ref EventArgument<IEntity, IBuildingComponent> eventArguments) {
		if (ArrivedBuildingEvent == null)
			return;
		ArrivedBuildingEvent(eventArguments.Target?.Owner);
	}

	private void ExitBuildingEvent(
		ref EventArgument<IEntity, IBuildingComponent> eventArguments) {
		var target = eventArguments.Target;
		if (target == null || target.IsDisposed)
			return;
		LeaveBuildingEvent(target.Owner);
	}

	[Method("Teleport to", "Target,Area", "")]
	public void TeleportToArea(IEntity target, AreaEnum NOT_USED_area) {
		if (Parent != null) {
			if (Parent.Instance != null) {
				if (!Parent.IsWorldEntity)
					return;
				Component.TeleportTo(target);
			} else
				Logger.AddError("Position parent engine entity not defined !");
		} else
			Logger.AddError("Position parent entity not defined !");
	}

	[Method("Teleport to", "Target", "")]
	public void TeleportTo(IEntity target) {
		if (Parent != null) {
			if (Parent.Instance != null) {
				if (!Parent.IsWorldEntity)
					return;
				if (target == null)
					Logger.AddError(string.Format("Teleport position for entity {0} is null at {1}!", Parent.Name,
						EngineAPIManager.Instance.CurrentFSMStateInfo));
				else
					Component.TeleportTo(target);
			} else
				Logger.AddError("Position parent engine entity not defined !");
		} else
			Logger.AddError("Position parent entity not defined !");
	}

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.ExitRegionEvent -= OnLeaveRegion;
		Component.EnterRegionEvent -= OnEnterRegion;
		Component.ExitAreaEvent -= OnLeaveArea;
		Component.EnterAreaEvent -= OnEnterArea;
		Component.EnterBuildingEvent -= EnterBuildingEvent;
		Component.ExitBuildingEvent -= ExitBuildingEvent;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.ExitRegionEvent += OnLeaveRegion;
		Component.EnterRegionEvent += OnEnterRegion;
		Component.ExitAreaEvent += OnLeaveArea;
		Component.EnterAreaEvent += OnEnterArea;
		Component.EnterBuildingEvent += EnterBuildingEvent;
		Component.ExitBuildingEvent += ExitBuildingEvent;
	}
}