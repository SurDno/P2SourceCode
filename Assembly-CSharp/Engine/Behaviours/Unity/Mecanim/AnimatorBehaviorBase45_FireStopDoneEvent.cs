using UnityEngine;

namespace Engine.Behaviours.Unity.Mecanim;

public class AnimatorBehaviorBase45_FireStopDoneEvent : StateMachineBehaviour {
	private static int movableWalkStopLeftStateHash = Animator.StringToHash("Base Layer.Move.Walk Stop Left");
	private static int movableWalkStopRightStateHash = Animator.StringToHash("Base Layer.Move.Walk Stop Right");

	public override void OnStateExit(
		Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex) {
		var animatorState = AnimatorState45.GetAnimatorState(animator);
		var component = animator.gameObject.GetComponent<Rootmotion45>();
		animatorState.FireStopDoneEvent();
		if (stateInfo.fullPathHash == movableWalkStopLeftStateHash)
			animatorState.VelocityScale = animatorState.RemainingDistance /
			                              AnimatorBehaviorBase45Utility.LeftLegStopDistance(component,
				                              animatorState.MovableSpeed);
		else {
			if (stateInfo.fullPathHash != movableWalkStopRightStateHash)
				return;
			animatorState.VelocityScale = animatorState.RemainingDistance /
			                              AnimatorBehaviorBase45Utility.RightLegStopDistance(component,
				                              animatorState.MovableSpeed);
		}
	}
}