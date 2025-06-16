public class FightAnimator_ReactionPush : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    ++FightAnimatorBehavior.GetAnimatorState(animator).ReactionPushCount;
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    --FightAnimatorBehavior.GetAnimatorState(animator).ReactionPushCount;
  }
}
