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
  public class IncreaseThirstEffect : IEffect
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
    protected ParameterNameEnum thirstParameterName;
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
    protected ParameterNameEnum lowStaminaParameterName;
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
    protected float increaseThirstStepLowStaminaValue = 0.00015f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float increaseThirstStepMiddleStaminaValue = 0.0001f;
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
            Debug.LogError((object) ("Error compute effects, effect name : " + Name + " , target : " + Target.GetInfo()));
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
      IParameter<bool> byName1 = component?.GetByName<bool>(runParameterName);
      if (byName1 != null && byName1.Value)
        return;
      IParameter<float> byName2 = component?.GetByName<float>(staminaParameterName);
      if (byName2 == null || byName2.Value >= (double) byName2.MaxValue)
        return;
      IParameter<float> byName3 = component?.GetByName<float>(thirstParameterName);
      if (byName3 == null || byName3.Value >= (double) byName3.MaxValue)
        return;
      IParameter<bool> byName4 = component?.GetByName<bool>(lowStaminaParameterName);
      if (byName4 == null)
        return;
      float num1 = InstanceByRequest<DifficultySettings>.Instance.GetValue("Thirst_Speed");
      float num2 = byName3.Value;
      float num3 = !byName4.Value ? num2 + num1 * increaseThirstStepMiddleStaminaValue : num2 + num1 * increaseThirstStepLowStaminaValue * byName2.MaxValue;
      byName3.Value = num3;
    }

    public void Cleanup()
    {
    }
  }
}
