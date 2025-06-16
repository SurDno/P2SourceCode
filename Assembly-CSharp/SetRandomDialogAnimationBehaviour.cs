using Engine.Behaviours.Components;
using UnityEngine;

public class SetRandomDialogAnimationBehaviour : StateMachineBehaviour {
	private int lastUsedAnimation;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		var component = animator.gameObject.GetComponent<Pivot>();
		if (component == null) {
			component = animator.gameObject.transform.parent.GetComponent<Pivot>();
			if (component == null)
				return;
		}

		var num1 = lastUsedAnimation == 0 ? 1 : 0;
		var num2 = Random.Range(0, component.DialogIdleAnimationCount + 1);
		lastUsedAnimation = num2;
		animator.SetInteger("Dialog.AnimationControl", num2);
	}
}