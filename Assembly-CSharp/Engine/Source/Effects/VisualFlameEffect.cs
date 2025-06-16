using System.Linq;
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
using UnityEngine;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class VisualFlameEffect : IEffect
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool single = false;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool realTime = false;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float duration;
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

    public string Name => GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => queue;

    private bool TargetIgnored(IEntity target)
    {
      ParametersComponent component = target.GetComponent<ParametersComponent>();
      IParameter<bool> byName1 = component?.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
      if (byName1 != null && byName1.Value)
        return true;
      IParameter<float> byName2 = component?.GetByName<float>(ParameterNameEnum.FireArmor);
      return byName2 != null && byName2.Value >= 1.0;
    }

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      if (TargetIgnored(Target))
        return true;
      float num = realTime ? currentRealTime : currentGameTime;
      if (single && !Name.IsNullOrEmpty() && Target.GetComponent<EffectsComponent>().Effects.FirstOrDefault(o => o.Name == Name) is VisualFlameEffect visualFlameEffect)
      {
        visualFlameEffect.duration = duration;
        visualFlameEffect.startTime = num;
        return false;
      }
      startTime = num;
      effect = ((IEntityView) Target).GameObject.GetComponentInChildren<SkinnedMeshRenderer>()?.gameObject?.AddComponent<BurnedSurfaceEffect>();
      if (effect != null)
      {
        effect.SmokeLevel = 1f;
        effect.FireLevel = 1f;
      }
      ParametersComponent component = Target.GetComponent<ParametersComponent>();
      if (Target.GetComponent<NpcControllerComponent>() != null)
      {
        isNpc = true;
        deadParameter = component.GetByName<bool>(ParameterNameEnum.Dead);
      }
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      if (TargetIgnored(Target))
        return false;
      float num1 = realTime ? currentRealTime : currentGameTime;
      float num2 = num1 - previousTime;
      previousTime = num1;
      bool flag = num1 - (double) startTime > duration;
      if (effect != null)
      {
        bool needRemove = GetNeedRemove();
        float num3 = Mathf.Max(duration - (num1 - startTime), 0.0f);
        float num4 = num3 >= (double) fadeOutTime ? 1f : num3 / fadeOutTime;
        if (needRemove)
        {
          effect.FireLevel = num4;
          effect.SmokeLevel = num4;
        }
        else if (flag)
        {
          effect.FireLevel = Mathf.MoveTowards(effect.FireLevel, fireLevelOnDisabled, num2 * fireAndSmokeChangeSpeed);
          effect.SmokeLevel = Mathf.MoveTowards(effect.SmokeLevel, smokeLevelOnDisabled, num2 * fireAndSmokeChangeSpeed);
          if (!Mathf.Approximately(effect.FireLevel, fireLevelOnDisabled) || !Mathf.Approximately(effect.SmokeLevel, smokeLevelOnDisabled))
            return true;
        }
      }
      return !(duration > 0.0 & flag);
    }

    public void Cleanup()
    {
      if (GetNeedRemove())
      {
        if (!(effect != null))
          return;
        Object.Destroy(effect);
        effect = null;
      }
      else
      {
        effect.SmokeLevel = smokeLevelOnDisabled;
        effect.FireLevel = fireLevelOnDisabled;
      }
    }

    private bool GetNeedRemove()
    {
      bool needRemove = true;
      if (isNpc && deadParameter != null && deadParameter.Value)
        needRemove = false;
      return needRemove;
    }
  }
}
