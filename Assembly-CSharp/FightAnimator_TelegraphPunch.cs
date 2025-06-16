using UnityEngine;

public class FightAnimator_TelegraphPunch : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.GetAnimatorState(animator).IsTelegraphPunching = true;
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.GetAnimatorState(animator).IsTelegraphPunching = false;
  }
}
