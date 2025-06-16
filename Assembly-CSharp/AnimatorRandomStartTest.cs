using Engine.Behaviours.Components;
using UnityEngine;

public class AnimatorRandomStartTest : MonoBehaviour {
	private void Start() {
		var component = GetComponent<Pivot>();
		if (!(bool)(Object)component)
			return;
		var animator = component.GetAnimator();
		if ((bool)(Object)animator)
			animator.Play(0, 0, Random.Range(0, 1));
	}

	private void Update() { }
}