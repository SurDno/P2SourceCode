using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Connections;
using Inspectors;
using System;
using UnityEngine;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MoveProjectileEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy(MemberEnum.None, Name = "Power")]
    [DataWriteProxy(MemberEnum.None, Name = "Power")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float throwPower = 20f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnityAsset<GameObject> projectilePrefab;
    private EnemyBase self;
    private EnemyBase enemy;
    private IEntityView bomb;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime) => true;

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      ParentComponent component1 = this.AbilityItem.Self.GetComponent<ParentComponent>();
      this.self = (EnemyBase) null;
      if (component1 != null)
      {
        if (component1.GetRootParent() == null)
          return true;
        this.self = ((IEntityView) component1.GetRootParent()).GameObject?.GetComponent<EnemyBase>();
      }
      this.enemy = this.self?.Enemy;
      NPCWeaponService component2 = this.self?.GetComponent<NPCWeaponService>();
      if ((UnityEngine.Object) this.self == (UnityEngine.Object) null || (UnityEngine.Object) this.enemy == (UnityEngine.Object) null || (UnityEngine.Object) component2 == (UnityEngine.Object) null)
        return true;
      this.bomb = (IEntityView) this.AbilityItem.Self;
      if (this.bomb != null)
      {
        if ((UnityEngine.Object) this.bomb.GameObject != (UnityEngine.Object) null)
          this.Throw();
        else
          this.bomb.OnGameObjectChangedEvent += new Action(this.OnGameObjectChanged);
      }
      return false;
    }

    private void OnGameObjectChanged()
    {
      if (!((UnityEngine.Object) this.bomb.GameObject != (UnityEngine.Object) null))
        return;
      this.bomb.OnGameObjectChangedEvent -= new Action(this.OnGameObjectChanged);
      this.Throw();
    }

    private void Throw()
    {
      Vector3 vector3 = this.self.LastThrowV * ((this.enemy.transform.position - this.self.transform.position).normalized * Mathf.Cos(this.self.LastThrowAngle) + Vector3.up * Mathf.Sin(this.self.LastThrowAngle));
      GameObject gameObject = this.bomb.GameObject;
      Rigidbody component = gameObject.GetComponent<Rigidbody>();
      component.velocity = vector3;
      component.angularVelocity = UnityEngine.Random.insideUnitSphere * this.throwPower;
      gameObject.GetComponent<ProjectileObject>().SetOwner(this.self);
    }

    public void Cleanup()
    {
    }
  }
}
