// Decompiled with JetBrains decompiler
// Type: ProjectileObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
