using UnityEngine;

public class FightAnimator_SuperPunch : StateMachineBehaviour {
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		FightAnimatorBehavior.GetAnimatorState(animator).IsSuperPunching = true;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		FightAnimatorBehavior.GetAnimatorState(animator).IsSuperPunching = false;
	}
}