// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.ProjectileHitAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using System;
using UnityEngine;

#nullable disable
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
