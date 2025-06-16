// Decompiled with JetBrains decompiler
// Type: FightAnimator_ReactionReset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class FightAnimator_ReactionReset : StateMachineBehaviour
{
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.AnimatorState animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
    animatorState.ReactionEmptyCount = 0;
    animatorState.ReactionHandsCount = 0;
    animatorState.ReactionKnifeCount = 0;
    animatorState.ReactionBombCount = 0;
    animatorState.StaggerCount = 0;
  }
}
