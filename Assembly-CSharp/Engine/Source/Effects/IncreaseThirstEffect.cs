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
using UnityEngine;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class IncreaseThirstEffect : IEffect
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
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected bool enable = true;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum staminaParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum thirstParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum runParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum lowStaminaParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected DurationTypeEnum durationType = DurationTypeEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool realTime;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float duration;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float interval;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float increaseThirstStepLowStaminaValue = 0.00015f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float increaseThirstStepMiddleStaminaValue = 0.0001f;
    private float lastTime;
    private float startTime;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public string Name => this.GetType().Name;

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      float num = this.realTime ? currentRealTime : currentGameTime;
      if (this.durationType == DurationTypeEnum.ByAbility)
        this.AbilityItem.AddDependEffect((IEffect) this);
      this.startTime = num;
      this.lastTime = num;
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      float num = this.realTime ? currentRealTime : currentGameTime;
      if (this.durationType == DurationTypeEnum.ByDuration && (double) num - (double) this.startTime > (double) this.duration || this.durationType == DurationTypeEnum.ByAbility && (this.AbilityItem == null || !this.AbilityItem.Active))
        return false;
      if ((double) this.interval == 0.0)
      {
        this.lastTime = num;
        this.ComputeEffect();
      }
      else
      {
        while ((double) num - (double) this.lastTime >= (double) this.interval)
        {
          float lastTime = this.lastTime;
          this.lastTime += this.interval;
          if ((double) lastTime == (double) this.lastTime)
          {
            Debug.LogError((object) ("Error compute effects, effect name : " + this.Name + " , target : " + this.Target.GetInfo()));
            break;
          }
          this.ComputeEffect();
        }
      }
      return this.durationType != DurationTypeEnum.None && this.durationType != DurationTypeEnum.Once;
    }

    public void ComputeEffect()
    {
      if (!this.enable)
        return;
      ParametersComponent component = this.Target?.GetComponent<ParametersComponent>();
      IParameter<bool> byName1 = component?.GetByName<bool>(this.runParameterName);
      if (byName1 != null && byName1.Value)
        return;
      IParameter<float> byName2 = component?.GetByName<float>(this.staminaParameterName);
      if (byName2 == null || (double) byName2.Value >= (double) byName2.MaxValue)
        return;
      IParameter<float> byName3 = component?.GetByName<float>(this.thirstParameterName);
      if (byName3 == null || (double) byName3.Value >= (double) byName3.MaxValue)
        return;
      IParameter<bool> byName4 = component?.GetByName<bool>(this.lowStaminaParameterName);
      if (byName4 == null)
        return;
      float num1 = InstanceByRequest<DifficultySettings>.Instance.GetValue("Thirst_Speed");
      float num2 = byName3.Value;
      float num3 = !byName4.Value ? num2 + num1 * this.increaseThirstStepMiddleStaminaValue : num2 + num1 * this.increaseThirstStepLowStaminaValue * byName2.MaxValue;
      byName3.Value = num3;
    }

    public void Cleanup()
    {
    }
  }
}
