public static class MovementControllerUtility
{
  public static void SetRandomAnimation(
    Animator animator,
    int secondaryIdleAnimationCount,
    int secondaryLowIdleAnimationCount)
  {
    int num1 = 5;
    int num2 = 3;
    int num3 = Random.Range(0, secondaryIdleAnimationCount * num2 + secondaryLowIdleAnimationCount);
    int num4 = num3 >= secondaryIdleAnimationCount * num2 ? num1 + (num3 - secondaryIdleAnimationCount * num2) : num3 / num2;
    animator.SetInteger("Movable.Idle.AnimationControl", num4);
  }
}
