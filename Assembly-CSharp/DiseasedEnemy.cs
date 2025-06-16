// Decompiled with JetBrains decompiler
// Type: DiseasedEnemy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common.Components.AttackerPlayer;
using UnityEngine;

#nullable disable
public class DiseasedEnemy : NPCEnemy
{
  public override void Prepunch(ReactionType reactionType, WeaponEnum weapon, EnemyBase enemy)
  {
  }

  public override void PrepunchUppercut(
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
  }

  public override void Punch(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (this.IsDead || this.IsFaint)
      return;
    this.pivot.PlaySound(Pivot.SoundEnum.HittedVocal);
    this.pivot.PlaySound(Pivot.SoundEnum.BlockHitted);
    this.Push(Vector3.zero, enemy);
  }

  public override void Push(Vector3 velocity, EnemyBase enemy)
  {
    if (this.IsDead || this.IsFaint)
      return;
    Vector3 vector3 = this.transform.InverseTransformDirection(-enemy.transform.forward);
    float num = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
    if (!((Object) this.animator != (Object) null))
      return;
    this.animator.SetTrigger("Fight.Triggers/Push");
    this.animator.SetFloat("Fight.PushAngle", num);
  }

  public override void PushMove(Vector3 direction)
  {
  }
}
