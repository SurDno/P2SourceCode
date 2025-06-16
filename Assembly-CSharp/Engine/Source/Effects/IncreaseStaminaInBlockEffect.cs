using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class IncreaseStaminaInBlockEffect : IEffect
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
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected bool enable = true;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum staminaParameterName;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum runParameterName;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum blockTypeParameterName;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum isFightingParameterName;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected DurationTypeEnum durationType = DurationTypeEnum.None;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool realTime;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float duration;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float interval;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float increaseStaminaStepMaxValue = 0.01f;
    private float lastTime;
    private float startTime;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public string Name => GetType().Name;

    public ParameterEffectQueueEnum Queue => queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      float num = realTime ? currentRealTime : currentGameTime;
      if (durationType == DurationTypeEnum.ByAbility)
        AbilityItem.AddDependEffect(this);
      startTime = num;
      lastTime = num;
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      float num = realTime ? currentRealTime : currentGameTime;
      if (durationType == DurationTypeEnum.ByDuration && num - (double) startTime > duration || durationType == DurationTypeEnum.ByAbility && (AbilityItem == null || !AbilityItem.Active))
        return false;
      if (interval == 0.0)
      {
        lastTime = num;
        ComputeEffect();
      }
      else
      {
        while (num - (double) this.lastTime >= interval)
        {
          float lastTime = this.lastTime;
          this.lastTime += interval;
          if (lastTime == (double) this.lastTime)
          {
            Debug.LogError("Error compute effects, effect name : " + Name + " , target : " + Target.GetInfo());
            break;
          }
          ComputeEffect();
        }
      }
      return durationType != DurationTypeEnum.None && durationType != DurationTypeEnum.Once;
    }

    public void ComputeEffect()
    {
      if (!enable)
        return;
      ParametersComponent component = Target?.GetComponent<ParametersComponent>();
      IParameter<bool> byName1 = component?.GetByName<bool>(isFightingParameterName);
      if (byName1 != null && !byName1.Value)
        return;
      IParameter<BlockTypeEnum> byName2 = component?.GetByName<BlockTypeEnum>(blockTypeParameterName);
      if (byName2 != null && byName2.Value != BlockTypeEnum.Block)
        return;
      IParameter<bool> byName3 = component?.GetByName<bool>(runParameterName);
      if (byName3 != null && byName3.Value)
        return;
      IParameter<float> byName4 = component?.GetByName<float>(staminaParameterName);
      if (byName4 == null)
        return;
      float num = byName4.Value;
      if (byName4.Value >= (double) byName4.MaxValue)
        return;
      byName4.Value += increaseStaminaStepMaxValue;
    }

    public void Cleanup()
    {
    }
  }
}
