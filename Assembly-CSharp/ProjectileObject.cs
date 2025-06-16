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
    if (owner == null || collision.transform.IsChildOf(owner.transform) || wasCollision)
      return;
    wasCollision = true;
    Action<Transform> onProjectileHit = OnProjectileHit;
    if (onProjectileHit == null)
      return;
    onProjectileHit(transform);
  }
}
