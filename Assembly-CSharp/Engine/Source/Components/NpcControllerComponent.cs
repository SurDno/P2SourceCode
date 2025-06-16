using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components.Crowds;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine.Source.Components;

[Factory(typeof(INpcControllerComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class NpcControllerComponent :
	EngineComponent,
	INpcControllerComponent,
	IComponent,
	ICrowdContextComponent,
	IEntityEventsListener {
	[FromThis] private DetectorComponent detector;
	[FromThis] private ParametersComponent parameters;
	private IEntity lastAttacker;
	[Inspected] private NpcControllerComponent target;

	public event Action<ActionEnum> ActionEvent;

	public event Action<CombatActionEnum, IEntity> CombatActionEvent;

	public NpcControllerComponent Target => target;

	[Inspected] public IParameterValue<bool> IsDead { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> IsImmortal { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> IsAway { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> CanAutopsy { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> CanTrade { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> ForceTrade { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> CanHeal { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<float> Health { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> Infection { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> PreInfection { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> Pain { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> Immunity { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<StammKind> StammKind { get; } = new ParameterValue<StammKind>();

	[Inspected] public IParameterValue<FractionEnum> Fraction { get; } = new ParameterValue<FractionEnum>();

	[Inspected] public IParameterValue<CombatStyleEnum> CombatStyle { get; } = new ParameterValue<CombatStyleEnum>();

	[Inspected]
	public IParameterValue<BoundHealthStateEnum> BoundHealthState { get; } = new ParameterValue<BoundHealthStateEnum>();

	[Inspected] public IParameterValue<bool> HealingAttempted { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> ImmuneBoostAttempted { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> IsCombatIgnored { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> SayReplicsInIdle { get; } = new ParameterValue<bool>();

	public void FireAction(ActionEnum action) {
		if (ActionEvent != null)
			ActionEvent(action);
		Owner.GetComponent<BehaviorComponent>()?.SendEvent("Player_" + action);
	}

	public void FireAction<T>(ActionEnum action, T arg1) {
		if (ActionEvent != null)
			ActionEvent(action);
		Owner.GetComponent<BehaviorComponent>()?.SendEvent("Npc_" + action, arg1);
	}

	public void FireCombatAction(CombatActionEnum action, IEntity target) {
		if (CombatActionEvent == null)
			return;
		CombatActionEvent(action, target);
	}

	public IEntity LastAttacker {
		get => lastAttacker;
		set => lastAttacker = value;
	}

	public override void OnAdded() {
		base.OnAdded();
		if (detector != null) {
			detector.OnSee += OnSee;
			detector.OnStopSee += OnStopSee;
			detector.OnHear += OnHear;
			detector.OnStopHear += OnStopHear;
		}

		IsDead.Set(parameters.GetByName<bool>(ParameterNameEnum.Dead));
		IsImmortal.Set(parameters.GetByName<bool>(ParameterNameEnum.Immortal));
		IsAway.Set(parameters.GetByName<bool>(ParameterNameEnum.Away));
		CanAutopsy.Set(parameters.GetByName<bool>(ParameterNameEnum.CanAutopsy));
		CanTrade.Set(parameters.GetByName<bool>(ParameterNameEnum.CanTrade));
		ForceTrade.Set(parameters.GetByName<bool>(ParameterNameEnum.ForceTrade));
		CanHeal.Set(parameters.GetByName<bool>(ParameterNameEnum.CanHeal));
		Health.Set(parameters.GetByName<float>(ParameterNameEnum.Health));
		Infection.Set(parameters.GetByName<float>(ParameterNameEnum.Infection));
		PreInfection.Set(parameters.GetByName<float>(ParameterNameEnum.PreInfection));
		Pain.Set(parameters.GetByName<float>(ParameterNameEnum.Pain));
		Immunity.Set(parameters.GetByName<float>(ParameterNameEnum.Immunity));
		StammKind.Set(parameters.GetByName<StammKind>(ParameterNameEnum.StammKind));
		Fraction.Set(parameters.GetByName<FractionEnum>(ParameterNameEnum.Fraction));
		CombatStyle.Set(parameters.GetByName<CombatStyleEnum>(ParameterNameEnum.CombatStyle));
		BoundHealthState.Set(parameters.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState));
		HealingAttempted.Set(parameters.GetByName<bool>(ParameterNameEnum.HealingAttempted));
		ImmuneBoostAttempted.Set(parameters.GetByName<bool>(ParameterNameEnum.ImmuneBoostAttempted));
		IsCombatIgnored.Set(parameters.GetByName<bool>(ParameterNameEnum.IsCombatIgnored));
		SayReplicsInIdle.Set(parameters.GetByName<bool>(ParameterNameEnum.SayReplicsInIdle));
	}

	public override void OnRemoved() {
		if (detector != null) {
			detector.OnSee -= OnSee;
			detector.OnStopSee -= OnStopSee;
			detector.OnHear -= OnHear;
			detector.OnStopHear -= OnStopHear;
		}

		IsDead.Set(null);
		IsImmortal.Set(null);
		IsAway.Set(null);
		CanAutopsy.Set(null);
		CanTrade.Set(null);
		ForceTrade.Set(null);
		CanHeal.Set(null);
		Health.Set(null);
		Infection.Set(null);
		PreInfection.Set(null);
		Pain.Set(null);
		StammKind.Set(null);
		Fraction.Set(null);
		CombatStyle.Set(null);
		IsCombatIgnored.Set(null);
		HealingAttempted.Set(null);
		ImmuneBoostAttempted.Set(null);
		SayReplicsInIdle.Set(null);
		SetTarget(null);
		base.OnRemoved();
	}

	private void OnSee(IDetectableComponent target) {
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player == null)
			Debug.LogError("Player not found : " + Owner.GetInfo());
		else {
			if (target.Owner == null || target.Owner.Id != player.Id)
				return;
			target.GetComponent<PlayerControllerComponent>()?.AddVisible(Owner);
		}
	}

	private void OnStopSee(IDetectableComponent obj) {
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player == null || obj.Owner == null || obj.Owner.Id != player.Id)
			return;
		obj.GetComponent<PlayerControllerComponent>()?.RemoveVisible(Owner);
	}

	private void OnHear(IDetectableComponent target) {
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player == null)
			Debug.LogError("Player not found : " + Owner.GetInfo());
		else {
			if (target.Owner == null || target.Owner.Id != player.Id)
				return;
			target.GetComponent<PlayerControllerComponent>()?.AddHearing(Owner);
		}
	}

	private void OnStopHear(IDetectableComponent obj) {
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player == null)
			Debug.LogError("Player not found : " + Owner.GetInfo());
		else {
			if (obj.Owner == null || obj.Owner.Id != player.Id)
				return;
			obj.GetComponent<PlayerControllerComponent>()?.RemoveHearing(Owner);
		}
	}

	public void StoreState(List<IParameter> states, bool indoor) {
		if (!indoor)
			return;
		CrowdContextUtility.Store(parameters, states, ParameterNameEnum.Dead, ParameterNameEnum.Health,
			ParameterNameEnum.Customization, ParameterNameEnum.Model);
	}

	public void RestoreState(List<IParameter> states, bool indoor) {
		RestoreOrGenerateParameters(states);
		if (!indoor)
			return;
		CrowdContextUtility.Restore(parameters, states, ParameterNameEnum.Dead, ParameterNameEnum.Health,
			ParameterNameEnum.Customization, ParameterNameEnum.Model);
	}

	private void RestoreOrGenerateParameters(List<IParameter> states) {
		var byName1 = parameters.GetByName<int>(ParameterNameEnum.Customization);
		if (byName1 != null) {
			var parameter = states.FirstOrDefault(o => o.Name == ParameterNameEnum.Customization);
			byName1.Value = parameter != null
				? ((IParameter<int>)parameter).Value
				: Random.Range(byName1.MinValue, byName1.MaxValue + 1);
		}

		var byName2 = parameters.GetByName<int>(ParameterNameEnum.Model);
		if (byName2 == null)
			return;
		var parameter1 = states.FirstOrDefault(o => o.Name == ParameterNameEnum.Model);
		byName2.Value = parameter1 != null
			? ((IParameter<int>)parameter1).Value
			: Random.Range(byName2.MinValue, byName2.MaxValue + 1);
	}

	public void ComputeShoot(NpcControllerComponent target) {
		SetTarget(target);
		ComputeEventsToSingleTarget(ActionEnum.ShootNpc, target.Owner);
		ServiceLocator.GetService<CombatService>()?.HitNpc(Owner, target.Owner);
		target.LastAttacker = Owner;
	}

	public void ComputeHit(NpcControllerComponent target) {
		SetTarget(target);
		ComputeEventsToSingleTarget(ActionEnum.HitNpc, target.Owner);
		ServiceLocator.GetService<CombatService>()?.HitNpc(Owner, target.Owner);
		target.LastAttacker = Owner;
	}

	private void ComputeEventsToSingleTarget(ActionEnum action, IEntity target) {
		if (target == null)
			return;
		var transform = ((IEntityView)Owner).GameObject.transform;
		target.GetComponent<NpcControllerComponent>()?.FireAction(action, transform);
	}

	private void SetTarget(NpcControllerComponent target) {
		if (this.target == target)
			return;
		if (this.target != null) {
			((Entity)this.target.Owner).RemoveListener(this);
			this.target.IsDead.ChangeValueEvent -= OnChangeDeadTarget;
			this.target = null;
		}

		this.target = target;
		if (this.target == null)
			return;
		((Entity)this.target.Owner).AddListener(this);
		this.target.IsDead.ChangeValueEvent += OnChangeDeadTarget;
	}

	private void OnChangeDeadTarget(bool value) {
		if (!value)
			return;
		SetTarget(null);
	}

	private void OnDisposeEvent() {
		SetTarget(null);
	}

	public void OnEntityEvent(IEntity sender, EntityEvents kind) {
		if (kind != EntityEvents.DisposeEvent)
			return;
		OnDisposeEvent();
	}
}