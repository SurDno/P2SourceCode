public class FightAnimator_ReactionEmpty : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    ++FightAnimatorBehavior.GetAnimatorState(animator).ReactionEmptyCount;
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.AnimatorState animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
    --animatorState.ReactionEmptyCount;
    if (animatorState.ReactionEmptyCount >= 0)
      return;
    animatorState.ReactionEmptyCount = 0;
  }
}
