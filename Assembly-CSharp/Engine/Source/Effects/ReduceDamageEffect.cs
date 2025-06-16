using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Difficulties;
using Inspectors;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ReduceDamageEffect : IEffect
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
    protected ParameterNameEnum damageParameterName;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum adsorbtionMaxParameterName;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float maxArmor = 0.1f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float durabilityReduceByHit = 0.01f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float durabilityReduceKoeficient = 0.1f;
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
      IParameter<float> byName1 = component?.GetByName<float>(damageParameterName);
      if (byName1 == null)
        return;
      IParameter<float> byName2 = component?.GetByName<float>(adsorbtionMaxParameterName);
      if (byName2 == null)
        return;
      float num1 = byName1.Value;
      if (num1 <= 0.0)
        return;
      float num2 = InstanceByRequest<DifficultySettings>.Instance.GetValue("Clothes_Durability");
      IParameter<float> byName3 = AbilityItem?.Item?.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
      float num3 = byName3 == null ? 1f : byName3.Value;
      float num4 = maxArmor * num3;
      float num5 = num1 * num4;
      float num6 = (durabilityReduceByHit + num5 * durabilityReduceKoeficient) * num2;
      float num7 = byName2.Value + num5;
      byName2.Value = num7;
      if (byName3 == null)
        return;
      byName3.Value = Mathf.Max(num3 - num6, 0.0f);
    }
  }
}
