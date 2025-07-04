﻿using UnityEngine;

namespace Engine.Behaviours.Unity.Mecanim
{
  public class AnimatorBehaviorBase45_Right_NeedToStop : StateMachineBehaviour
  {
    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(animator);
      animatorState.MovableStop = AnimatorBehaviorBase45Utility.NeedToStopEnterRightLeg(animator.gameObject.GetComponent<Rootmotion45>(), animatorState.MovableSpeed, animatorState.RemainingDistance, out float scale);
      animatorState.VelocityScale = scale;
    }
  }
}
