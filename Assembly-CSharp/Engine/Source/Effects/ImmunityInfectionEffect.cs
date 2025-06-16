using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ImmunityInfectionEffect : IEffect
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
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum infectionDamageParameterName;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum immunityParameterName;
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
      if (num - (double) lastTime >= interval)
      {
        lastTime = num;
        ComputeEffect();
      }
      return durationType != DurationTypeEnum.None && durationType != DurationTypeEnum.Once;
    }

    public void Cleanup()
    {
    }

    private void ComputeEffect()
    {
      if (!enable)
        return;
      ParametersComponent component = Target?.GetComponent<ParametersComponent>();
      IParameter<float> byName1 = component?.GetByName<float>(infectionDamageParameterName);
      if (byName1 == null)
        return;
      IParameter<float> byName2 = component?.GetByName<float>(immunityParameterName);
      if (byName2 == null)
        return;
      float num1 = byName1.Value;
      if (num1 <= 0.0)
        return;
      float num2 = byName2.Value;
      if (num2 <= 0.0)
        return;
      float num3 = Mathf.Max(num1 - num2, 0.0f);
      float num4 = Mathf.Max(num2 - num1, 0.0f);
      byName1.Value = num3;
      byName2.Value = num4;
    }
  }
}
