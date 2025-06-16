using Cofe.Utility;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;
using System;
using System.Linq;
using UnityEngine;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class VisualFlameEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool single = false;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool realTime = false;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float duration = 0.0f;
    private float startTime;
    private IParameter<bool> burningParameter;
    private bool isNpc;
    private IParameter<bool> deadParameter;
    private BurnedSurfaceEffect effect;
    private float fadeOutTime = 2f;
    private float fireLevelOnDisabled = 0.0f;
    private float smokeLevelOnDisabled = 1f;
    private float fireAndSmokeChangeSpeed = 1f;
    private float previousTime;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    private bool TargetIgnored(IEntity target)
    {
      ParametersComponent component = target.GetComponent<ParametersComponent>();
      IParameter<bool> byName1 = component?.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
      if (byName1 != null && byName1.Value)
        return true;
      IParameter<float> byName2 = component?.GetByName<float>(ParameterNameEnum.FireArmor);
      return byName2 != null && (double) byName2.Value >= 1.0;
    }

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      if (this.TargetIgnored(this.Target))
        return true;
      float num = this.realTime ? currentRealTime : currentGameTime;
      if (this.single && !this.Name.IsNullOrEmpty() && this.Target.GetComponent<EffectsComponent>().Effects.FirstOrDefault<IEffect>((Func<IEffect, bool>) (o => o.Name == this.Name)) is VisualFlameEffect visualFlameEffect)
      {
        visualFlameEffect.duration = this.duration;
        visualFlameEffect.startTime = num;
        return false;
      }
      this.startTime = num;
      this.effect = ((IEntityView) this.Target).GameObject.GetComponentInChildren<SkinnedMeshRenderer>()?.gameObject?.AddComponent<BurnedSurfaceEffect>();
      if ((UnityEngine.Object) this.effect != (UnityEngine.Object) null)
      {
        this.effect.SmokeLevel = 1f;
        this.effect.FireLevel = 1f;
      }
      ParametersComponent component = this.Target.GetComponent<ParametersComponent>();
      if (this.Target.GetComponent<NpcControllerComponent>() != null)
      {
        this.isNpc = true;
        this.deadParameter = component.GetByName<bool>(ParameterNameEnum.Dead);
      }
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      if (this.TargetIgnored(this.Target))
        return false;
      float num1 = this.realTime ? currentRealTime : currentGameTime;
      float num2 = num1 - this.previousTime;
      this.previousTime = num1;
      bool flag = (double) num1 - (double) this.startTime > (double) this.duration;
      if ((UnityEngine.Object) this.effect != (UnityEngine.Object) null)
      {
        bool needRemove = this.GetNeedRemove();
        float num3 = Mathf.Max(this.duration - (num1 - this.startTime), 0.0f);
        float num4 = (double) num3 >= (double) this.fadeOutTime ? 1f : num3 / this.fadeOutTime;
        if (needRemove)
        {
          this.effect.FireLevel = num4;
          this.effect.SmokeLevel = num4;
        }
        else if (flag)
        {
          this.effect.FireLevel = Mathf.MoveTowards(this.effect.FireLevel, this.fireLevelOnDisabled, num2 * this.fireAndSmokeChangeSpeed);
          this.effect.SmokeLevel = Mathf.MoveTowards(this.effect.SmokeLevel, this.smokeLevelOnDisabled, num2 * this.fireAndSmokeChangeSpeed);
          if (!Mathf.Approximately(this.effect.FireLevel, this.fireLevelOnDisabled) || !Mathf.Approximately(this.effect.SmokeLevel, this.smokeLevelOnDisabled))
            return true;
        }
      }
      return !((double) this.duration > 0.0 & flag);
    }

    public void Cleanup()
    {
      if (this.GetNeedRemove())
      {
        if (!((UnityEngine.Object) this.effect != (UnityEngine.Object) null))
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.effect);
        this.effect = (BurnedSurfaceEffect) null;
      }
      else
      {
        this.effect.SmokeLevel = this.smokeLevelOnDisabled;
        this.effect.FireLevel = this.fireLevelOnDisabled;
      }
    }

    private bool GetNeedRemove()
    {
      bool needRemove = true;
      if (this.isNpc && this.deadParameter != null && this.deadParameter.Value)
        needRemove = false;
      return needRemove;
    }
  }
}
