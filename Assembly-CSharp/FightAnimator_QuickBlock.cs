using UnityEngine;

public class FightAnimator_QuickBlock : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.GetAnimatorState(animator).IsQuickBlock = true;
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.GetAnimatorState(animator).IsQuickBlock = false;
  }
}
