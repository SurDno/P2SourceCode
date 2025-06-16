using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Inspectors;
using UnityEngine;

public class AnimatorStateChecker : MonoBehaviour {
	[Inspected] private AnimatorState45 animatorState;

	private void Awake() {
		animatorState = AnimatorState45.GetAnimatorState(GetComponent<Pivot>().GetAnimator());
	}
}