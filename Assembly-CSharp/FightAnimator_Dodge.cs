using UnityEngine;

public class FightAnimator_Dodge : StateMachineBehaviour {
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		FightAnimatorBehavior.GetAnimatorState(animator).IsDodge = true;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		FightAnimatorBehavior.GetAnimatorState(animator).IsDodge = false;
	}
}