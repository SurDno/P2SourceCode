using Engine.Behaviours.Components;
using Engine.Common.Components.AttackerPlayer;

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
    if (IsDead || IsFaint)
      return;
    pivot.PlaySound(Pivot.SoundEnum.HittedVocal);
    pivot.PlaySound(Pivot.SoundEnum.BlockHitted);
    Push(Vector3.zero, enemy);
  }

  public override void Push(Vector3 velocity, EnemyBase enemy)
  {
    if (IsDead || IsFaint)
      return;
    Vector3 vector3 = this.transform.InverseTransformDirection(-enemy.transform.forward);
    float num = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
    if (!((Object) animator != (Object) null))
      return;
    animator.SetTrigger("Fight.Triggers/Push");
    animator.SetFloat("Fight.PushAngle", num);
  }

  public override void PushMove(Vector3 direction)
  {
  }
}
