using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Abilities.Controllers;
using Engine.Source.Commons.Effects;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NpcPunchHitEffect : IEffect
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected WeaponEnum weapon = WeaponEnum.Hands;

    public string Name => GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => queue;

    public void Cleanup()
    {
    }

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      if (!(AbilityItem.AbilityController is CloseCombatAbilityController))
      {
        Debug.LogError(typeof (NpcPunchEffect).Name + " requires " + typeof (CloseCombatAbilityController).Name);
        return false;
      }
      ((IEntityView) AbilityItem.Self).GameObject.GetComponent<EnemyBase>()?.FirePunchHitEvent(weapon);
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime) => false;
  }
}
