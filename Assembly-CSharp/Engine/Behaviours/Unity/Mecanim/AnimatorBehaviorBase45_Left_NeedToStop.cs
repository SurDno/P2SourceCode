// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Unity.Mecanim.AnimatorBehaviorBase45_Left_NeedToStop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Unity.Mecanim
{
  public class AnimatorBehaviorBase45_Left_NeedToStop : StateMachineBehaviour
  {
    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(animator);
      float scale;
      animatorState.MovableStop = AnimatorBehaviorBase45Utility.NeedToStopEnterLeftLeg(animator.gameObject.GetComponent<Rootmotion45>(), animatorState.MovableSpeed, animatorState.RemainingDistance, out scale);
      animatorState.VelocityScale = scale;
    }
  }
}
