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
  public class ChangeItemParameterEffect : IEffect
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
    protected ParameterNameEnum itemParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float itemParameterChange = 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected string difficultyMultiplierParameterName = "";
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
      IParameter<float> byName = this.AbilityItem?.Item?.GetComponent<ParametersComponent>()?.GetByName<float>(this.itemParameterName);
      float num = string.IsNullOrEmpty(this.difficultyMultiplierParameterName) ? 1f : InstanceByRequest<DifficultySettings>.Instance.GetValue(this.difficultyMultiplierParameterName);
      if (byName == null)
        return;
      byName.Value += this.itemParameterChange * num;
    }
  }
}
