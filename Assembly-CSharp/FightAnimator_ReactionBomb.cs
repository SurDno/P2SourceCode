using UnityEngine;

public class FightAnimator_ReactionBomb : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    ++FightAnimatorBehavior.GetAnimatorState(animator).ReactionBombCount;
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.AnimatorState animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
    --animatorState.ReactionBombCount;
    if (animatorState.ReactionBombCount >= 0)
      return;
    animatorState.ReactionBombCount = 0;
  }
}
