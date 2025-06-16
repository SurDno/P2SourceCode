// Decompiled with JetBrains decompiler
// Type: FightAnimator_ReactionBomb
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
