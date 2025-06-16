// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Unity.Mecanim.DiseasedAnimatorState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
