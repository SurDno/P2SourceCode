public class FightAnimator_WalkEscapeBegin : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.GetAnimatorState(animator).Condition = FightAnimatorBehavior.AnimatorState.NPCCondition.EscapeBegin;
  }
}
