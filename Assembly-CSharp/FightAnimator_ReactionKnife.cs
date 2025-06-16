using UnityEngine;

public class FightAnimator_ReactionKnife : StateMachineBehaviour {
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		++FightAnimatorBehavior.GetAnimatorState(animator).ReactionKnifeCount;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		var animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
		--animatorState.ReactionKnifeCount;
		if (animatorState.ReactionKnifeCount >= 0)
			return;
		animatorState.ReactionKnifeCount = 0;
	}
}