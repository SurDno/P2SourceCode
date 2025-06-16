using UnityEngine;

public class SetRandomPOIAmimationBehaviour : StateMachineBehaviour {
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		var num = Random.Range(0, animator.GetInteger("Movable.POI.MiddleAnimationsCount"));
		animator.SetInteger("Movable.POI.AnimationIndex2", num);
	}
}