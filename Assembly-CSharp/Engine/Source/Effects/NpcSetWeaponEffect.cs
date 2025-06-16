using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Inspectors;
using System;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NpcSetWeaponEffect : IEffect
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
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected WeaponEnum weapon = WeaponEnum.Hands;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      if ((UnityEngine.Object) ((IEntityView) this.Target).GameObject == (UnityEngine.Object) null || (UnityEngine.Object) ((IEntityView) this.Target).GameObject.GetComponent<EnemyBase>() == (UnityEngine.Object) null)
      {
        ((IEntityView) this.Target).OnGameObjectChangedEvent -= new Action(this.SetWeapon);
        ((IEntityView) this.Target).OnGameObjectChangedEvent += new Action(this.SetWeapon);
      }
      else
        this.SetWeapon();
      return true;
    }

    private void SetWeapon()
    {
      if (!((UnityEngine.Object) ((IEntityView) this.Target).GameObject != (UnityEngine.Object) null) || !((UnityEngine.Object) ((IEntityView) this.Target).GameObject.GetComponent<EnemyBase>() != (UnityEngine.Object) null))
        return;
      ((IEntityView) this.Target).OnGameObjectChangedEvent -= new Action(this.SetWeapon);
      ((IEntityView) this.Target).GameObject.GetComponent<WeaponServiceBase>().Weapon = this.weapon;
    }

    public bool Compute(float currentRealTime, float currentGameTime) => false;

    public void Cleanup()
    {
    }
  }
}
