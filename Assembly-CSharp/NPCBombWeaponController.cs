// Decompiled with JetBrains decompiler
// Type: NPCBombWeaponController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class NPCBombWeaponController : NPCWeaponControllerBase
{
  private GameObject bomb;
  private ProjectileObject projectile;
  private NPCEnemy npcEnemy;
  private float throwPower = 20f;

  public override void Initialise(NPCWeaponService service)
  {
    this.npcEnemy = service.GetComponent<NPCEnemy>();
    if ((UnityEngine.Object) service.BombParent != (UnityEngine.Object) null)
    {
      this.bomb = UnityEngine.Object.Instantiate<GameObject>(service.BombPrefab, service.BombParent);
      this.bomb?.SetActive(false);
    }
    base.Initialise(service);
  }

  protected override void ShowWeapon(bool show)
  {
    base.ShowWeapon(show);
    if (show)
      return;
    this.bomb?.SetActive(show);
  }

  protected override void GetLayersIndices()
  {
    if (!((UnityEngine.Object) this.animator != (UnityEngine.Object) null))
      return;
    this.walkLayerIndex = this.animator.GetLayerIndex("Fight Bomb Walk Layer");
    this.attackLayerIndex = this.animator.GetLayerIndex("Fight Bomb Attack Layer");
    this.reactionLayerIndex = this.animator.GetLayerIndex("Fight Bomb Reaction Layer");
  }

  public override void OnAnimatorEvent(string data)
  {
    if (data.StartsWith("Bomb.Show"))
    {
      if ((UnityEngine.Object) this.bomb == (UnityEngine.Object) null && (UnityEngine.Object) this.service.BombParent != (UnityEngine.Object) null)
        this.bomb = UnityEngine.Object.Instantiate<GameObject>(this.service.BombPrefab, this.service.BombParent);
      this.ShowWeapon(true);
      this.bomb?.SetActive(true);
    }
    else if (data.StartsWith("Bomb.Throw"))
    {
      this.bomb?.SetActive(false);
      this.CreateBomb();
    }
    base.OnAnimatorEvent(data);
  }

  private void CreateBomb()
  {
    EnemyBase enemy = this.npcEnemy.Enemy;
    if ((UnityEngine.Object) enemy == (UnityEngine.Object) null)
      return;
    Vector3 vector3 = this.npcEnemy.LastThrowV * ((enemy.transform.position - this.npcEnemy.transform.position).normalized * Mathf.Cos(this.npcEnemy.LastThrowAngle) + Vector3.up * Mathf.Sin(this.npcEnemy.LastThrowAngle));
    Transform transform = this.service.BombParent.transform;
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ScriptableObjectInstance<ResourceFromCodeData>.Instance.BottleBomb, transform.position, transform.rotation);
    Rigidbody component = gameObject.GetComponent<Rigidbody>();
    component.velocity = vector3;
    component.angularVelocity = UnityEngine.Random.insideUnitSphere * this.throwPower;
    component.useGravity = true;
    this.projectile = gameObject.GetComponent<ProjectileObject>();
    this.projectile.SetOwner((EnemyBase) this.npcEnemy);
    this.projectile.OnProjectileHit -= new Action<Transform>(this.OnProjectileHit);
    this.projectile.OnProjectileHit += new Action<Transform>(this.OnProjectileHit);
  }

  private void OnProjectileHit(Transform projectileTransform)
  {
    this.service.ProjectileHitPosition = projectileTransform.position;
    this.service.ProjectileHitRotation = projectileTransform.rotation;
    this.BombHit();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.projectile.gameObject);
  }
}
