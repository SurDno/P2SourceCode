using System.Collections.Generic;
using UnityEngine;

namespace Engine.Behaviours.Unity.Mecanim
{
  public class DiseasedAnimatorState
  {
    private static Dictionary<Animator, DiseasedAnimatorState> diseasedAnimatorStates = new Dictionary<Animator, DiseasedAnimatorState>();
    public Animator Animator;

    public static DiseasedAnimatorState GetAnimatorState(Animator animator)
    {
      DiseasedAnimatorState animatorState;
      if (!DiseasedAnimatorState.diseasedAnimatorStates.TryGetValue(animator, out animatorState))
      {
        animatorState = new DiseasedAnimatorState();
        animatorState.Animator = animator;
        DiseasedAnimatorState.diseasedAnimatorStates[animator] = animatorState;
      }
      return animatorState;
    }

    public void TriggerPlayerPush()
    {
      this.Animator.SetTrigger("AttackerDiseased.Player.Push.Trigger");
    }

    public void TriggerPlayerFendOff()
    {
      this.Animator.SetTrigger("AttackerDiseased.Player.FendOff.Trigger");
    }

    public float PlayerPushAngle
    {
      set => this.Animator.SetFloat("AttackerDiseased.Player.Push.Angle", value);
      get => this.Animator.GetFloat("AttackerDiseased.Player.Push.Angle");
    }
  }
}
