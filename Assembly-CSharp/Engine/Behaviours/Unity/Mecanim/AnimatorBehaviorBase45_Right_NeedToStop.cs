using UnityEngine;

namespace Engine.Behaviours.Unity.Mecanim;

public class AnimatorBehaviorBase45_Right_NeedToStop : StateMachineBehaviour {
	public override void OnStateEnter(
		Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex) {
		var animatorState = AnimatorState45.GetAnimatorState(animator);
		float scale;
		animatorState.MovableStop = AnimatorBehaviorBase45Utility.NeedToStopEnterRightLeg(
			animator.gameObject.GetComponent<Rootmotion45>(), animatorState.MovableSpeed,
			animatorState.RemainingDistance, out scale);
		animatorState.VelocityScale = scale;
	}
}