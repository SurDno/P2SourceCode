using UnityEngine;

public class FightAnimator_Throw : StateMachineBehaviour {
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		FightAnimatorBehavior.GetAnimatorState(animator).IsThrowing = true;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		FightAnimatorBehavior.GetAnimatorState(animator).IsThrowing = false;
	}
}