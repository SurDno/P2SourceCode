using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Inspectors;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NpcSetWeaponEffect : IEffect
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

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      if (((IEntityView) Target).GameObject == null || ((IEntityView) Target).GameObject.GetComponent<EnemyBase>() == null)
      {
        ((IEntityView) Target).OnGameObjectChangedEvent -= SetWeapon;
        ((IEntityView) Target).OnGameObjectChangedEvent += SetWeapon;
      }
      else
        SetWeapon();
      return true;
    }

    private void SetWeapon()
    {
      if (!(((IEntityView) Target).GameObject != null) || !(((IEntityView) Target).GameObject.GetComponent<EnemyBase>() != null))
        return;
      ((IEntityView) Target).OnGameObjectChangedEvent -= SetWeapon;
      ((IEntityView) Target).GameObject.GetComponent<WeaponServiceBase>().Weapon = weapon;
    }

    public bool Compute(float currentRealTime, float currentGameTime) => false;

    public void Cleanup()
    {
    }
  }
}
