public class FightAnimator_ReactionHands : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    ++FightAnimatorBehavior.GetAnimatorState(animator).ReactionHandsCount;
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.AnimatorState animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
    --animatorState.ReactionHandsCount;
    if (animatorState.ReactionHandsCount >= 0)
      return;
    animatorState.ReactionHandsCount = 0;
  }
}
