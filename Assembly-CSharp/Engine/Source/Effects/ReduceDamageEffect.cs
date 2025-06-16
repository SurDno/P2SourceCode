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
  public class ReduceDamageEffect : IEffect
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
    protected ParameterNameEnum damageParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum adsorbtionMaxParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float maxArmor = 0.1f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float durabilityReduceByHit = 0.01f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float durabilityReduceKoeficient = 0.1f;
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
      if ((double) num - (double) this.lastTime >= (double) this.interval)
      {
        this.lastTime = num;
        this.ComputeEffect();
      }
      return this.durationType != DurationTypeEnum.None && this.durationType != DurationTypeEnum.Once;
    }

    public void Cleanup()
    {
    }

    private void ComputeEffect()
    {
      if (!this.enable)
        return;
      ParametersComponent component = this.Target?.GetComponent<ParametersComponent>();
      IParameter<float> byName1 = component?.GetByName<float>(this.damageParameterName);
      if (byName1 == null)
        return;
      IParameter<float> byName2 = component?.GetByName<float>(this.adsorbtionMaxParameterName);
      if (byName2 == null)
        return;
      float num1 = byName1.Value;
      if ((double) num1 <= 0.0)
        return;
      float num2 = InstanceByRequest<DifficultySettings>.Instance.GetValue("Clothes_Durability");
      IParameter<float> byName3 = this.AbilityItem?.Item?.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
      float num3 = byName3 == null ? 1f : byName3.Value;
      float num4 = this.maxArmor * num3;
      float num5 = num1 * num4;
      float num6 = (this.durabilityReduceByHit + num5 * this.durabilityReduceKoeficient) * num2;
      float num7 = byName2.Value + num5;
      byName2.Value = num7;
      if (byName3 == null)
        return;
      byName3.Value = Mathf.Max(num3 - num6, 0.0f);
    }
  }
}
