// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Unity.Mecanim.AnimatorBehaviorBase45_PrimaryIdle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Unity.Mecanim
{
  public class AnimatorBehaviorBase45_PrimaryIdle : StateMachineBehaviour
  {
    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      bool flag = (double) Random.value < (double) AnimatorState45.GetAnimatorState(animator).PrimaryIdleProbability;
      animator.SetBool("Movable.Idle.PrimaryIdle", flag);
    }
  }
}
