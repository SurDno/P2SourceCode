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

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AddBulletDamageEffect : IEffect
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
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected DurationTypeEnum durationType = DurationTypeEnum.Once;
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
    protected ParameterNameEnum damageParameterName;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected string difficultyMultiplierParameterName = "";
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float bodyDamage = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float armDamage = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float legDamage = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float headDamage = 0.0f;
    private float lastTime;
    private float startTime;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public string Name => GetType().Name;

    public ParameterEffectQueueEnum Queue => queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      if (durationType == DurationTypeEnum.Once)
        Target.GetComponent<EffectsComponent>().ForceComputeUpdate();
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      ParametersComponent component = Target?.GetComponent<ParametersComponent>();
      float num = InstanceByRequest<DifficultySettings>.Instance.GetValue(difficultyMultiplierParameterName);
      IParameter<float> byName = component?.GetByName<float>(damageParameterName);
      if (byName == null)
        return false;
      IAbilityProjectile projectile = AbilityItem.Projectile;
      if (AbilityItem.Projectile is RaycastAbilityProjectile)
      {
        ShotTargetBodyPartEnum nextTargetBodyPart = (AbilityItem.Projectile as RaycastAbilityProjectile).GetNextTargetBodyPart();
        if (nextTargetBodyPart == ShotTargetBodyPartEnum.Body)
          byName.Value += bodyDamage * num;
        if (nextTargetBodyPart == ShotTargetBodyPartEnum.Arm)
          byName.Value += armDamage * num;
        if (nextTargetBodyPart == ShotTargetBodyPartEnum.Leg)
          byName.Value += legDamage * num;
        if (nextTargetBodyPart == ShotTargetBodyPartEnum.Head)
          byName.Value += headDamage * num;
      }
      else
        Debug.LogError((object) ("projectile for " + typeof (AddBulletDamageEffect) + " must be " + typeof (RaycastAbilityProjectile)));
      return false;
    }

    public void Cleanup()
    {
    }
  }
}
