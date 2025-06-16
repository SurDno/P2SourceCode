using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
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
      owner.OnGameObjectChangedEvent -= OnGOChanged;
      owner.OnGameObjectChangedEvent += OnGOChanged;
      AddEventListener();
    }

    private void OnGOChanged() => AddEventListener();

    private void AddEventListener()
    {
      projectile = ((IEntityView) abilityItem.Ability.Owner).GameObject?.GetComponent<ProjectileObject>();
      if (!(projectile != null))
        return;
      projectile.OnProjectileHit -= OnProjectileHitEvent;
      projectile.OnProjectileHit += OnProjectileHitEvent;
    }

    public void Shutdown()
    {
      ((IEntityView) abilityItem.Ability.Owner).OnGameObjectChangedEvent -= OnGOChanged;
      if (!(projectile != null))
        return;
      projectile.OnProjectileHit -= OnProjectileHitEvent;
    }

    private void OnProjectileHitEvent(Transform transform)
    {
      abilityItem.Active = true;
      abilityItem.Active = false;
    }
  }
}
