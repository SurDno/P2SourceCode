using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MoveProjectileEffect : IEffect
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy(Name = "Power")]
    [DataWriteProxy(Name = "Power")]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float throwPower = 20f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnityAsset<GameObject> projectilePrefab;
    private EnemyBase self;
    private EnemyBase enemy;
    private IEntityView bomb;

    public string Name => GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => queue;

    public bool Prepare(float currentRealTime, float currentGameTime) => true;

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      ParentComponent component1 = AbilityItem.Self.GetComponent<ParentComponent>();
      self = null;
      if (component1 != null)
      {
        if (component1.GetRootParent() == null)
          return true;
        self = ((IEntityView) component1.GetRootParent()).GameObject?.GetComponent<EnemyBase>();
      }
      enemy = self?.Enemy;
      NPCWeaponService component2 = self?.GetComponent<NPCWeaponService>();
      if (self == null || enemy == null || component2 == null)
        return true;
      bomb = (IEntityView) AbilityItem.Self;
      if (bomb != null)
      {
        if (bomb.GameObject != null)
          Throw();
        else
          bomb.OnGameObjectChangedEvent += OnGameObjectChanged;
      }
      return false;
    }

    private void OnGameObjectChanged()
    {
      if (!(bomb.GameObject != null))
        return;
      bomb.OnGameObjectChangedEvent -= OnGameObjectChanged;
      Throw();
    }

    private void Throw()
    {
      Vector3 vector3 = self.LastThrowV * ((enemy.transform.position - self.transform.position).normalized * Mathf.Cos(self.LastThrowAngle) + Vector3.up * Mathf.Sin(self.LastThrowAngle));
      GameObject gameObject = bomb.GameObject;
      Rigidbody component = gameObject.GetComponent<Rigidbody>();
      component.velocity = vector3;
      component.angularVelocity = Random.insideUnitSphere * throwPower;
      gameObject.GetComponent<ProjectileObject>().SetOwner(self);
    }

    public void Cleanup()
    {
    }
  }
}
