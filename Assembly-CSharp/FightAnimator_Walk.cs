using UnityEngine;

public class FightAnimator_Walk : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.GetAnimatorState(animator).Condition = FightAnimatorBehavior.AnimatorState.NPCCondition.Walk;
  }
}
