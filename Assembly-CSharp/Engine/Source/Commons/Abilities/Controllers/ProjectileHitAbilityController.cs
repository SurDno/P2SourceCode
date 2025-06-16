using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using System;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ProjectileHitAbilityController : IAbilityController
  {
    private AbilityItem abilityItem;
    private ProjectileObject projectile;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      IEntityView owner = (IEntityView) abilityItem.Ability.Owner;
      owner.OnGameObjectChangedEvent -= new Action(this.OnGOChanged);
      owner.OnGameObjectChangedEvent += new Action(this.OnGOChanged);
      this.AddEventListener();
    }

    private void OnGOChanged() => this.AddEventListener();

    private void AddEventListener()
    {
      this.projectile = ((IEntityView) this.abilityItem.Ability.Owner).GameObject?.GetComponent<ProjectileObject>();
      if (!((UnityEngine.Object) this.projectile != (UnityEngine.Object) null))
        return;
      this.projectile.OnProjectileHit -= new Action<Transform>(this.OnProjectileHitEvent);
      this.projectile.OnProjectileHit += new Action<Transform>(this.OnProjectileHitEvent);
    }

    public void Shutdown()
    {
      ((IEntityView) this.abilityItem.Ability.Owner).OnGameObjectChangedEvent -= new Action(this.OnGOChanged);
      if (!((UnityEngine.Object) this.projectile != (UnityEngine.Object) null))
        return;
      this.projectile.OnProjectileHit -= new Action<Transform>(this.OnProjectileHitEvent);
    }

    private void OnProjectileHitEvent(Transform transform)
    {
      this.abilityItem.Active = true;
      this.abilityItem.Active = false;
    }
  }
}
