using UnityEngine;

public class FightAnimator_StepPunch : StateMachineBehaviour {
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		FightAnimatorBehavior.GetAnimatorState(animator).IsStepPunching = true;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		FightAnimatorBehavior.GetAnimatorState(animator).IsStepPunching = false;
	}
}