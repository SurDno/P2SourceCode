// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Unity.Mecanim.AnimatorBehaviorBase45_FireStopDoneEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Unity.Mecanim
{
  public class AnimatorBehaviorBase45_FireStopDoneEvent : StateMachineBehaviour
  {
    private static int movableWalkStopLeftStateHash = Animator.StringToHash("Base Layer.Move.Walk Stop Left");
    private static int movableWalkStopRightStateHash = Animator.StringToHash("Base Layer.Move.Walk Stop Right");

    public override void OnStateExit(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(animator);
      Rootmotion45 component = animator.gameObject.GetComponent<Rootmotion45>();
      animatorState.FireStopDoneEvent();
      if (stateInfo.fullPathHash == AnimatorBehaviorBase45_FireStopDoneEvent.movableWalkStopLeftStateHash)
      {
        animatorState.VelocityScale = animatorState.RemainingDistance / AnimatorBehaviorBase45Utility.LeftLegStopDistance(component, animatorState.MovableSpeed);
      }
      else
      {
        if (stateInfo.fullPathHash != AnimatorBehaviorBase45_FireStopDoneEvent.movableWalkStopRightStateHash)
          return;
        animatorState.VelocityScale = animatorState.RemainingDistance / AnimatorBehaviorBase45Utility.RightLegStopDistance(component, animatorState.MovableSpeed);
      }
    }
  }
}
