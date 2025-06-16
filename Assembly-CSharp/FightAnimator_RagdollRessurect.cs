public class FightAnimator_RagdollRessurect : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.GetAnimatorState(animator).IsRessurect = true;
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.GetAnimatorState(animator).IsRessurect = false;
  }
}
