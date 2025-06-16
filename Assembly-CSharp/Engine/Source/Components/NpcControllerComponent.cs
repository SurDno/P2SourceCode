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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Engine.Source.Components
{
  [Factory(typeof (INpcControllerComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NpcControllerComponent : 
    EngineComponent,
    INpcControllerComponent,
    IComponent,
    ICrowdContextComponent,
    IEntityEventsListener
  {
    [FromThis]
    private DetectorComponent detector;
    [FromThis]
    private ParametersComponent parameters;
    private IEntity lastAttacker;
    [Inspected]
    private NpcControllerComponent target;

    public event Action<ActionEnum> ActionEvent;

    public event Action<CombatActionEnum, IEntity> CombatActionEvent;

    public NpcControllerComponent Target => this.target;

    [Inspected]
    public IParameterValue<bool> IsDead { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsImmortal { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsAway { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> CanAutopsy { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> CanTrade { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> ForceTrade { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> CanHeal { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<float> Health { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> Infection { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> PreInfection { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> Pain { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> Immunity { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<Engine.Common.Commons.StammKind> StammKind { get; } = (IParameterValue<Engine.Common.Commons.StammKind>) new ParameterValue<Engine.Common.Commons.StammKind>();

    [Inspected]
    public IParameterValue<FractionEnum> Fraction { get; } = (IParameterValue<FractionEnum>) new ParameterValue<FractionEnum>();

    [Inspected]
    public IParameterValue<CombatStyleEnum> CombatStyle { get; } = (IParameterValue<CombatStyleEnum>) new ParameterValue<CombatStyleEnum>();

    [Inspected]
    public IParameterValue<BoundHealthStateEnum> BoundHealthState { get; } = (IParameterValue<BoundHealthStateEnum>) new ParameterValue<BoundHealthStateEnum>();

    [Inspected]
    public IParameterValue<bool> HealingAttempted { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> ImmuneBoostAttempted { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsCombatIgnored { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> SayReplicsInIdle { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    public void FireAction(ActionEnum action)
    {
      if (this.ActionEvent != null)
        this.ActionEvent(action);
      this.Owner.GetComponent<BehaviorComponent>()?.SendEvent("Player_" + action.ToString());
    }

    public void FireAction<T>(ActionEnum action, T arg1)
    {
      if (this.ActionEvent != null)
        this.ActionEvent(action);
      this.Owner.GetComponent<BehaviorComponent>()?.SendEvent<T>("Npc_" + action.ToString(), arg1);
    }

    public void FireCombatAction(CombatActionEnum action, IEntity target)
    {
      if (this.CombatActionEvent == null)
        return;
      this.CombatActionEvent(action, target);
    }

    public IEntity LastAttacker
    {
      get => this.lastAttacker;
      set => this.lastAttacker = value;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      if (this.detector != null)
      {
        this.detector.OnSee += new Action<IDetectableComponent>(this.OnSee);
        this.detector.OnStopSee += new Action<IDetectableComponent>(this.OnStopSee);
        this.detector.OnHear += new Action<IDetectableComponent>(this.OnHear);
        this.detector.OnStopHear += new Action<IDetectableComponent>(this.OnStopHear);
      }
      this.IsDead.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Dead));
      this.IsImmortal.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Immortal));
      this.IsAway.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Away));
      this.CanAutopsy.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.CanAutopsy));
      this.CanTrade.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.CanTrade));
      this.ForceTrade.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.ForceTrade));
      this.CanHeal.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.CanHeal));
      this.Health.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Health));
      this.Infection.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Infection));
      this.PreInfection.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.PreInfection));
      this.Pain.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Pain));
      this.Immunity.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Immunity));
      this.StammKind.Set<Engine.Common.Commons.StammKind>(this.parameters.GetByName<Engine.Common.Commons.StammKind>(ParameterNameEnum.StammKind));
      this.Fraction.Set<FractionEnum>(this.parameters.GetByName<FractionEnum>(ParameterNameEnum.Fraction));
      this.CombatStyle.Set<CombatStyleEnum>(this.parameters.GetByName<CombatStyleEnum>(ParameterNameEnum.CombatStyle));
      this.BoundHealthState.Set<BoundHealthStateEnum>(this.parameters.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState));
      this.HealingAttempted.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.HealingAttempted));
      this.ImmuneBoostAttempted.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.ImmuneBoostAttempted));
      this.IsCombatIgnored.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.IsCombatIgnored));
      this.SayReplicsInIdle.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.SayReplicsInIdle));
    }

    public override void OnRemoved()
    {
      if (this.detector != null)
      {
        this.detector.OnSee -= new Action<IDetectableComponent>(this.OnSee);
        this.detector.OnStopSee -= new Action<IDetectableComponent>(this.OnStopSee);
        this.detector.OnHear -= new Action<IDetectableComponent>(this.OnHear);
        this.detector.OnStopHear -= new Action<IDetectableComponent>(this.OnStopHear);
      }
      this.IsDead.Set<bool>((IParameter<bool>) null);
      this.IsImmortal.Set<bool>((IParameter<bool>) null);
      this.IsAway.Set<bool>((IParameter<bool>) null);
      this.CanAutopsy.Set<bool>((IParameter<bool>) null);
      this.CanTrade.Set<bool>((IParameter<bool>) null);
      this.ForceTrade.Set<bool>((IParameter<bool>) null);
      this.CanHeal.Set<bool>((IParameter<bool>) null);
      this.Health.Set<float>((IParameter<float>) null);
      this.Infection.Set<float>((IParameter<float>) null);
      this.PreInfection.Set<float>((IParameter<float>) null);
      this.Pain.Set<float>((IParameter<float>) null);
      this.StammKind.Set<Engine.Common.Commons.StammKind>((IParameter<Engine.Common.Commons.StammKind>) null);
      this.Fraction.Set<FractionEnum>((IParameter<FractionEnum>) null);
      this.CombatStyle.Set<CombatStyleEnum>((IParameter<CombatStyleEnum>) null);
      this.IsCombatIgnored.Set<bool>((IParameter<bool>) null);
      this.HealingAttempted.Set<bool>((IParameter<bool>) null);
      this.ImmuneBoostAttempted.Set<bool>((IParameter<bool>) null);
      this.SayReplicsInIdle.Set<bool>((IParameter<bool>) null);
      this.SetTarget((NpcControllerComponent) null);
      base.OnRemoved();
    }

    private void OnSee(IDetectableComponent target)
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
      {
        Debug.LogError((object) ("Player not found : " + this.Owner.GetInfo()));
      }
      else
      {
        if (target.Owner == null || target.Owner.Id != player.Id)
          return;
        target.GetComponent<PlayerControllerComponent>()?.AddVisible(this.Owner);
      }
    }

    private void OnStopSee(IDetectableComponent obj)
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null || obj.Owner == null || obj.Owner.Id != player.Id)
        return;
      obj.GetComponent<PlayerControllerComponent>()?.RemoveVisible(this.Owner);
    }

    private void OnHear(IDetectableComponent target)
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
      {
        Debug.LogError((object) ("Player not found : " + this.Owner.GetInfo()));
      }
      else
      {
        if (target.Owner == null || target.Owner.Id != player.Id)
          return;
        target.GetComponent<PlayerControllerComponent>()?.AddHearing(this.Owner);
      }
    }

    private void OnStopHear(IDetectableComponent obj)
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
      {
        Debug.LogError((object) ("Player not found : " + this.Owner.GetInfo()));
      }
      else
      {
        if (obj.Owner == null || obj.Owner.Id != player.Id)
          return;
        obj.GetComponent<PlayerControllerComponent>()?.RemoveHearing(this.Owner);
      }
    }

    public void StoreState(List<IParameter> states, bool indoor)
    {
      if (!indoor)
        return;
      CrowdContextUtility.Store(this.parameters, states, ParameterNameEnum.Dead, ParameterNameEnum.Health, ParameterNameEnum.Customization, ParameterNameEnum.Model);
    }

    public void RestoreState(List<IParameter> states, bool indoor)
    {
      this.RestoreOrGenerateParameters(states);
      if (!indoor)
        return;
      CrowdContextUtility.Restore(this.parameters, states, ParameterNameEnum.Dead, ParameterNameEnum.Health, ParameterNameEnum.Customization, ParameterNameEnum.Model);
    }

    private void RestoreOrGenerateParameters(List<IParameter> states)
    {
      IParameter<int> byName1 = this.parameters.GetByName<int>(ParameterNameEnum.Customization);
      if (byName1 != null)
      {
        IParameter parameter = states.FirstOrDefault<IParameter>((Func<IParameter, bool>) (o => o.Name == ParameterNameEnum.Customization));
        byName1.Value = parameter != null ? ((IParameter<int>) parameter).Value : UnityEngine.Random.Range(byName1.MinValue, byName1.MaxValue + 1);
      }
      IParameter<int> byName2 = this.parameters.GetByName<int>(ParameterNameEnum.Model);
      if (byName2 == null)
        return;
      IParameter parameter1 = states.FirstOrDefault<IParameter>((Func<IParameter, bool>) (o => o.Name == ParameterNameEnum.Model));
      byName2.Value = parameter1 != null ? ((IParameter<int>) parameter1).Value : UnityEngine.Random.Range(byName2.MinValue, byName2.MaxValue + 1);
    }

    public void ComputeShoot(NpcControllerComponent target)
    {
      this.SetTarget(target);
      this.ComputeEventsToSingleTarget(ActionEnum.ShootNpc, target.Owner);
      ServiceLocator.GetService<CombatService>()?.HitNpc(this.Owner, target.Owner);
      target.LastAttacker = this.Owner;
    }

    public void ComputeHit(NpcControllerComponent target)
    {
      this.SetTarget(target);
      this.ComputeEventsToSingleTarget(ActionEnum.HitNpc, target.Owner);
      ServiceLocator.GetService<CombatService>()?.HitNpc(this.Owner, target.Owner);
      target.LastAttacker = this.Owner;
    }

    private void ComputeEventsToSingleTarget(ActionEnum action, IEntity target)
    {
      if (target == null)
        return;
      Transform transform = ((IEntityView) this.Owner).GameObject.transform;
      target.GetComponent<NpcControllerComponent>()?.FireAction<Transform>(action, transform);
    }

    private void SetTarget(NpcControllerComponent target)
    {
      if (this.target == target)
        return;
      if (this.target != null)
      {
        ((Entity) this.target.Owner).RemoveListener((IEntityEventsListener) this);
        this.target.IsDead.ChangeValueEvent -= new Action<bool>(this.OnChangeDeadTarget);
        this.target = (NpcControllerComponent) null;
      }
      this.target = target;
      if (this.target == null)
        return;
      ((Entity) this.target.Owner).AddListener((IEntityEventsListener) this);
      this.target.IsDead.ChangeValueEvent += new Action<bool>(this.OnChangeDeadTarget);
    }

    private void OnChangeDeadTarget(bool value)
    {
      if (!value)
        return;
      this.SetTarget((NpcControllerComponent) null);
    }

    private void OnDisposeEvent() => this.SetTarget((NpcControllerComponent) null);

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.DisposeEvent)
        return;
      this.OnDisposeEvent();
    }
  }
}
