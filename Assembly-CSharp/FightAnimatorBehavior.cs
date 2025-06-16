using System.Collections.Generic;
using UnityEngine;

public class FightAnimatorBehavior : StateMachineBehaviour
{
  private static Dictionary<Animator, AnimatorState> animatorStates = new Dictionary<Animator, AnimatorState>();
  private static int walkBlendTreeHash = Animator.StringToHash("Walk Blend Tree");
  private static int defeatedHash = Animator.StringToHash("Defeated");
  private static int deadHash = Animator.StringToHash("Dead");

  public static AnimatorState GetAnimatorState(Animator animator)
  {
    if (animator == null)
      return new AnimatorState();
    AnimatorState animatorState;
    if (!animatorStates.TryGetValue(animator, out animatorState))
    {
      animatorState = new AnimatorState();
      animatorState.Animator = animator;
      animatorStates[animator] = animatorState;
    }
    return animatorState;
  }

  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    AnimatorState animatorState = GetAnimatorState(animator);
    if (stateInfo.shortNameHash == walkBlendTreeHash)
      animatorState.Condition = AnimatorState.NPCCondition.Walk;
    else if (stateInfo.shortNameHash == defeatedHash)
    {
      animatorState.Condition = AnimatorState.NPCCondition.Defeated;
    }
    else
    {
      if (stateInfo.shortNameHash != deadHash)
        return;
      animatorState.Condition = AnimatorState.NPCCondition.Dead;
    }
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    AnimatorState animatorState = GetAnimatorState(animator);
    if (layerIndex != animator.GetLayerIndex("Fight Reaction Layer") && layerIndex != animator.GetLayerIndex("Fight Knife Reaction Layer"))
      return;
    animatorState.IsPushing = false;
  }

  public class AnimatorState
  {
    public Animator Animator;
    public NPCCondition Condition;

    public void Reset()
    {
      StaggerCount = 0;
      ReactionEmptyCount = 0;
      ReactionHandsCount = 0;
      ReactionKnifeCount = 0;
      ReactionBombCount = 0;
      ReactionPushCount = 0;
      IsQuickBlock = false;
      IsDodge = false;
      IsCheating = false;
      IsRunPunching = false;
      IsPunching = false;
      IsStepPunching = false;
      IsTelegraphPunching = false;
      IsSuperPunching = false;
      IsThrowing = false;
      IsAiming = false;
      IsShoting = false;
      IsSurrender = false;
      IsRessurect = false;
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

    public bool IsStagger => StaggerCount > 0;

    public bool IsAttacking
    {
      get
      {
        return IsRunPunching || IsPunching || IsStepPunching || IsTelegraphPunching || IsSuperPunching || IsCheating || IsThrowing || IsAiming || IsShoting;
      }
    }

    public bool IsPushing { get; set; }

    public bool IsReaction
    {
      get
      {
        return ReactionEmptyCount > 0 || ReactionHandsCount > 0 || ReactionKnifeCount > 0 || ReactionBombCount > 0 || ReactionPushCount > 0;
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
