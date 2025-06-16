public class FightAnimator_ReactionReset : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.AnimatorState animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
    animatorState.ReactionEmptyCount = 0;
    animatorState.ReactionHandsCount = 0;
    animatorState.ReactionKnifeCount = 0;
    animatorState.ReactionBombCount = 0;
    animatorState.StaggerCount = 0;
  }
}
