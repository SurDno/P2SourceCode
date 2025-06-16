using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Abilities.Projectiles;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Difficulties;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AddBulletDamageEffect : IEffect
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
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected DurationTypeEnum durationType = DurationTypeEnum.Once;
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
    protected ParameterNameEnum damageParameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected string difficultyMultiplierParameterName = "";
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float bodyDamage = 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float armDamage = 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float legDamage = 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float headDamage = 0.0f;
    private float lastTime;
    private float startTime;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public string Name => this.GetType().Name;

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      if (this.durationType == DurationTypeEnum.Once)
        this.Target.GetComponent<EffectsComponent>().ForceComputeUpdate();
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      ParametersComponent component = this.Target?.GetComponent<ParametersComponent>();
      float num = InstanceByRequest<DifficultySettings>.Instance.GetValue(this.difficultyMultiplierParameterName);
      IParameter<float> byName = component?.GetByName<float>(this.damageParameterName);
      if (byName == null)
        return false;
      IAbilityProjectile projectile = this.AbilityItem.Projectile;
      if (this.AbilityItem.Projectile is RaycastAbilityProjectile)
      {
        ShotTargetBodyPartEnum nextTargetBodyPart = (this.AbilityItem.Projectile as RaycastAbilityProjectile).GetNextTargetBodyPart();
        if (nextTargetBodyPart == ShotTargetBodyPartEnum.Body)
          byName.Value += this.bodyDamage * num;
        if (nextTargetBodyPart == ShotTargetBodyPartEnum.Arm)
          byName.Value += this.armDamage * num;
        if (nextTargetBodyPart == ShotTargetBodyPartEnum.Leg)
          byName.Value += this.legDamage * num;
        if (nextTargetBodyPart == ShotTargetBodyPartEnum.Head)
          byName.Value += this.headDamage * num;
      }
      else
        Debug.LogError((object) ("projectile for " + (object) typeof (AddBulletDamageEffect) + " must be " + (object) typeof (RaycastAbilityProjectile)));
      return false;
    }

    public void Cleanup()
    {
    }
  }
}
