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
  public class ApplyDamageToHealthEffect : IEffect
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
    protected ParameterNameEnum healthParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum isCombatIgnoredParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum immortalParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum ballisticDamageParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum fireDamageParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum meleeDamageParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum fallDamageParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum fistsDamageParameterName;
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
      IParameter<bool> byName1 = component?.GetByName<bool>(this.isCombatIgnoredParameterName);
      if (byName1 != null && byName1.Value)
        return;
      IParameter<bool> byName2 = component?.GetByName<bool>(this.immortalParameterName);
      if (byName2 != null && byName2.Value)
        return;
      IParameter<float> byName3 = component?.GetByName<float>(this.healthParameterName);
      if (byName3 == null || (double) byName3.Value < 0.0)
        return;
      float num1 = 0.0f;
      IParameter<float> byName4 = component?.GetByName<float>(this.ballisticDamageParameterName);
      if (byName4 != null)
        num1 = byName4.Value;
      float num2 = 0.0f;
      IParameter<float> byName5 = component?.GetByName<float>(this.fireDamageParameterName);
      if (byName5 != null)
        num2 = byName5.Value;
      float num3 = 0.0f;
      IParameter<float> byName6 = component?.GetByName<float>(this.meleeDamageParameterName);
      if (byName6 != null)
        num3 = byName6.Value;
      float num4 = 0.0f;
      IParameter<float> byName7 = component?.GetByName<float>(this.fallDamageParameterName);
      if (byName7 != null)
        num4 = byName7.Value;
      float num5 = 0.0f;
      IParameter<float> byName8 = component?.GetByName<float>(this.fistsDamageParameterName);
      if (byName8 != null)
        num5 = byName8.Value;
      byName3.Value -= num1 + num2 + num3 + num4 + num5;
    }

    public void Cleanup()
    {
    }
  }
}
