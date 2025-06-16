using System;
using UnityEngine;

public class ProjectileObject : MonoBehaviour
{
  private bool wasCollision;
  private EnemyBase owner;

  public event Action<Transform> OnProjectileHit;

  public void SetOwner(EnemyBase owner) => this.owner = owner;

  private void OnCollisionEnter(Collision collision)
  {
    if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null || collision.transform.IsChildOf(this.owner.transform) || this.wasCollision)
      return;
    this.wasCollision = true;
    Action<Transform> onProjectileHit = this.OnProjectileHit;
    if (onProjectileHit == null)
      return;
    onProjectileHit(this.transform);
  }
}
