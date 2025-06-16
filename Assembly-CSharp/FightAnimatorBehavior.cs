// Decompiled with JetBrains decompiler
// Type: FightAnimatorBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class FightAnimatorBehavior : StateMachineBehaviour
{
  private static Dictionary<Animator, FightAnimatorBehavior.AnimatorState> animatorStates = new Dictionary<Animator, FightAnimatorBehavior.AnimatorState>();
  private static int walkBlendTreeHash = Animator.StringToHash("Walk Blend Tree");
  private static int defeatedHash = Animator.StringToHash("Defeated");
  private static int deadHash = Animator.StringToHash("Dead");

  public static FightAnimatorBehavior.AnimatorState GetAnimatorState(Animator animator)
  {
    if ((Object) animator == (Object) null)
      return new FightAnimatorBehavior.AnimatorState();
    FightAnimatorBehavior.AnimatorState animatorState;
    if (!FightAnimatorBehavior.animatorStates.TryGetValue(animator, out animatorState))
    {
      animatorState = new FightAnimatorBehavior.AnimatorState();
      animatorState.Animator = animator;
      FightAnimatorBehavior.animatorStates[animator] = animatorState;
    }
    return animatorState;
  }

  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.AnimatorState animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
    if (stateInfo.shortNameHash == FightAnimatorBehavior.walkBlendTreeHash)
      animatorState.Condition = FightAnimatorBehavior.AnimatorState.NPCCondition.Walk;
    else if (stateInfo.shortNameHash == FightAnimatorBehavior.defeatedHash)
    {
      animatorState.Condition = FightAnimatorBehavior.AnimatorState.NPCCondition.Defeated;
    }
    else
    {
      if (stateInfo.shortNameHash != FightAnimatorBehavior.deadHash)
        return;
      animatorState.Condition = FightAnimatorBehavior.AnimatorState.NPCCondition.Dead;
    }
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    FightAnimatorBehavior.AnimatorState animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
    if (layerIndex != animator.GetLayerIndex("Fight Reaction Layer") && layerIndex != animator.GetLayerIndex("Fight Knife Reaction Layer"))
      return;
    animatorState.IsPushing = false;
  }

  public class AnimatorState
  {
    public Animator Animator;
    public FightAnimatorBehavior.AnimatorState.NPCCondition Condition;

    public void Reset()
    {
      this.StaggerCount = 0;
      this.ReactionEmptyCount = 0;
      this.ReactionHandsCount = 0;
      this.ReactionKnifeCount = 0;
      this.ReactionBombCount = 0;
      this.ReactionPushCount = 0;
      this.IsQuickBlock = false;
      this.IsDodge = false;
      this.IsCheating = false;
      this.IsRunPunching = false;
      this.IsPunching = false;
      this.IsStepPunching = false;
      this.IsTelegraphPunching = false;
      this.IsSuperPunching = false;
      this.IsThrowing = false;
      this.IsAiming = false;
      this.IsShoting = false;
      this.IsSurrender = false;
      this.IsRessurect = false;
    }

    public bool IsQuickBlock { get; set; }

    public bool IsDodge { get; set; }

    public bool IsCheating { get; set; }

    public bool IsRunPunching { get; set; }

    public bool IsPunching { get; set; }

    public bool IsStepPunching { get; set; }

    public bool IsTelegraphPunching { get; set; }

    public bool IsSuperPunching { get; set; }

    public bool IsThrowing { get; set; }

    public bool IsAiming { get; set; }

    public bool IsShoting { get; set; }

    public bool IsSurrender { get; set; }

    public bool IsRessurect { get; set; }

    public int ReactionEmptyCount { get; set; }

    public int ReactionHandsCount { get; set; }

    public int ReactionKnifeCount { get; set; }

    public int ReactionBombCount { get; set; }

    public int ReactionPushCount { get; set; }

    public int StaggerCount { get; set; }

    public bool IsStagger => this.StaggerCount > 0;

    public bool IsAttacking
    {
      get
      {
        return this.IsRunPunching || this.IsPunching || this.IsStepPunching || this.IsTelegraphPunching || this.IsSuperPunching || this.IsCheating || this.IsThrowing || this.IsAiming || this.IsShoting;
      }
    }

    public bool IsPushing { get; set; }

    public bool IsReaction
    {
      get
      {
        return this.ReactionEmptyCount > 0 || this.ReactionHandsCount > 0 || this.ReactionKnifeCount > 0 || this.ReactionBombCount > 0 || this.ReactionPushCount > 0;
      }
    }

    public enum NPCCondition
    {
      Walk,
      Defeated,
      Dead,
      EscapeBegin,
      Escape,
      Loot,
    }
  }
}
