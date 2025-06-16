using Engine.Behaviours.Components;
using Engine.Common.Components.AttackerPlayer;
using UnityEngine;

public class DiseasedEnemy : NPCEnemy {
	public override void Prepunch(ReactionType reactionType, WeaponEnum weapon, EnemyBase enemy) { }

	public override void PrepunchUppercut(
		ReactionType reactionType,
		WeaponEnum weapon,
		EnemyBase enemy) { }

	public override void Punch(
		PunchTypeEnum punchType,
		ReactionType reactionType,
		WeaponEnum weapon,
		EnemyBase enemy) {
		if (IsDead || IsFaint)
			return;
		pivot.PlaySound(Pivot.SoundEnum.HittedVocal);
		pivot.PlaySound(Pivot.SoundEnum.BlockHitted);
		Push(Vector3.zero, enemy);
	}

	public override void Push(Vector3 velocity, EnemyBase enemy) {
		if (IsDead || IsFaint)
			return;
		var vector3 = transform.InverseTransformDirection(-enemy.transform.forward);
		var num = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
		if (!(animator != null))
			return;
		animator.SetTrigger("Fight.Triggers/Push");
		animator.SetFloat("Fight.PushAngle", num);
	}

	public override void PushMove(Vector3 direction) { }
}