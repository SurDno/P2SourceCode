using UnityEngine;

public class FightAnimator_RunPunch : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.GetAnimatorState(animator).IsRunPunching = true;
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.GetAnimatorState(animator).IsRunPunching = false;
  }
}
