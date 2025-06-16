using UnityEngine;

public class FightAnimator_Stagger : StateMachineBehaviour {
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		++FightAnimatorBehavior.GetAnimatorState(animator).StaggerCount;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		--FightAnimatorBehavior.GetAnimatorState(animator).StaggerCount;
	}
}