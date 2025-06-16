using UnityEngine;

public static class MovementControllerUtility {
	public static void SetRandomAnimation(
		Animator animator,
		int secondaryIdleAnimationCount,
		int secondaryLowIdleAnimationCount) {
		var num1 = 5;
		var num2 = 3;
		var num3 = Random.Range(0, secondaryIdleAnimationCount * num2 + secondaryLowIdleAnimationCount);
		var num4 = num3 >= secondaryIdleAnimationCount * num2
			? num1 + (num3 - secondaryIdleAnimationCount * num2)
			: num3 / num2;
		animator.SetInteger("Movable.Idle.AnimationControl", num4);
	}
}