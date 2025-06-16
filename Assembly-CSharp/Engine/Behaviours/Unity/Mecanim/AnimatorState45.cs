// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Unity.Mecanim.AnimatorState45
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.Tasks;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Unity.Mecanim
{
  public class AnimatorState45
  {
    private static int idleTagHash = Animator.StringToHash("Idle");
    private static int rotateTagHash = Animator.StringToHash("Rotate");
    private static int moveTagHash = Animator.StringToHash("Move");
    private static int moveStartTagHash = Animator.StringToHash("MoveStart");
    private static int moveStopTagHash = Animator.StringToHash("MoveStop");
    private static int poiTagHash = Animator.StringToHash("POI");
    private static int poiExitTagHash = Animator.StringToHash("POIExit");
    private static Dictionary<Animator, AnimatorState45> animatorStates = new Dictionary<Animator, AnimatorState45>(64);
    private HashSet<string> triggers = new HashSet<string>();
    public Animator Animator;
    public float PrimaryIdleProbability = 0.5f;

    public event Action StopDoneEvent;

    [Inspected]
    public bool IsRotate
    {
      get => this.Animator.GetCurrentAnimatorStateInfo(0).tagHash == AnimatorState45.rotateTagHash;
    }

    [Inspected]
    public bool IsMove
    {
      get
      {
        if ((UnityEngine.Object) this.Animator == (UnityEngine.Object) null)
          return false;
        int tagHash = this.Animator.GetCurrentAnimatorStateInfo(0).tagHash;
        return tagHash == AnimatorState45.moveTagHash || tagHash == AnimatorState45.moveStartTagHash || tagHash == AnimatorState45.moveStopTagHash;
      }
    }

    [Inspected]
    public bool IsMovementStart
    {
      get
      {
        return this.Animator.GetCurrentAnimatorStateInfo(0).tagHash == AnimatorState45.moveStartTagHash;
      }
    }

    [Inspected]
    public bool IsMovementStop
    {
      get
      {
        return this.Animator.GetCurrentAnimatorStateInfo(0).tagHash == AnimatorState45.moveStopTagHash;
      }
    }

    [Inspected]
    public bool IsPOI
    {
      get
      {
        int tagHash = this.Animator.GetCurrentAnimatorStateInfo(0).tagHash;
        return tagHash == AnimatorState45.poiTagHash || tagHash == AnimatorState45.poiExitTagHash;
      }
    }

    [Inspected]
    public bool IsPOIExit
    {
      get => this.Animator.GetCurrentAnimatorStateInfo(0).tagHash == AnimatorState45.poiExitTagHash;
    }

    public bool MovableStop
    {
      set => this.Animator.SetBool("Movable.Stop", value);
    }

    [Inspected]
    public bool NextMoveIsLeft
    {
      get => this.Animator.GetBool("Movable.Next.Is.Left");
      set => this.Animator.SetBool("Movable.Next.Is.Left", value);
    }

    public void SetTrigger(string name)
    {
      this.Animator.SetTrigger(name);
      this.triggers.Add(name);
    }

    public void ResetTrigger(string name)
    {
      this.Animator.ResetTrigger(name);
      this.triggers.Remove(name);
    }

    public void ResetAllTriggers()
    {
      foreach (string trigger in this.triggers)
        this.Animator.ResetTrigger(trigger);
      this.triggers.Clear();
    }

    public void FireStopDoneEvent()
    {
      Action stopDoneEvent = this.StopDoneEvent;
      if (stopDoneEvent == null)
        return;
      stopDoneEvent();
    }

    public AnimatorState45.MovableState45 ControlMovableState
    {
      set
      {
        if (!this.Animator.gameObject.activeSelf)
          return;
        this.Animator.ResetTrigger("Triggers/Base/Idle");
        this.Animator.ResetTrigger("Triggers/Base/Move");
        this.Animator.ResetTrigger("Triggers/Base/POI");
        this.Animator.ResetTrigger("Triggers/Base/Rotate");
        this.Animator.ResetTrigger("Triggers/Base/IdlePreset");
        switch (value)
        {
          case AnimatorState45.MovableState45.Idle:
            this.Animator.SetTrigger("Triggers/Base/Idle");
            this.Animator.SetInteger("Movable.State.Control", 1);
            break;
          case AnimatorState45.MovableState45.Rotate:
            this.Animator.SetTrigger("Triggers/Base/Rotate");
            this.Animator.SetInteger("Movable.State.Control", 2);
            break;
          case AnimatorState45.MovableState45.Move:
            this.Animator.SetTrigger("Triggers/Base/Move");
            this.Animator.SetInteger("Movable.State.Control", 3);
            break;
          case AnimatorState45.MovableState45.POI:
            this.Animator.SetTrigger("Triggers/Base/POI");
            this.Animator.SetInteger("Movable.State.Control", 6);
            break;
          case AnimatorState45.MovableState45.IdlePreset:
            this.Animator.SetTrigger("Triggers/Base/IdlePreset");
            this.Animator.SetInteger("Movable.State.Control", 6);
            break;
        }
      }
      get => (AnimatorState45.MovableState45) this.Animator.GetInteger("Movable.State.Control");
    }

    public MecanimKinds.MovablePOIStateKind ControlPOIState
    {
      set => this.Animator.SetInteger("Movable.POI.Control", (int) value);
    }

    public int ControlPOIMiddleAnimationsCount
    {
      set => this.Animator.SetInteger("Movable.POI.MiddleAnimationsCount", value);
    }

    public int ControlPOIAnimationIndex
    {
      set => this.Animator.SetInteger("Movable.POI.AnimationIndex", value);
    }

    public bool ControlPOIStartFromMiddle
    {
      set => this.Animator.SetBool("Movable.POI.StartFromMiddle", value);
    }

    public float StateLength => this.Animator.GetCurrentAnimatorStateInfo(0).length;

    public void ResetMovable() => this.Animator.SetTrigger("Movable.Cancel");

    [Inspected]
    public float RemainingDistance { get; set; }

    [Inspected]
    public float VelocityScale { get; set; }

    public float MovableSpeed
    {
      get => this.Animator.GetFloat("Movable.Velocity.Z");
      set => this.Animator.SetFloat("Movable.Velocity.Z", value);
    }

    public float MovableAngleStart
    {
      set => this.Animator.SetFloat("Movable.Angle.Start", value);
      get => this.Animator.GetFloat("Movable.Angle.Start");
    }

    public static AnimatorState45 GetAnimatorState(Animator animator)
    {
      if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
        return new AnimatorState45();
      AnimatorState45 animatorState;
      if (!AnimatorState45.animatorStates.TryGetValue(animator, out animatorState))
      {
        animatorState = new AnimatorState45();
        animatorState.Animator = animator;
        AnimatorState45.animatorStates[animator] = animatorState;
      }
      return animatorState;
    }

    public enum MovableState45
    {
      Unknown,
      Idle,
      Rotate,
      Move,
      POI,
      IdlePreset,
    }
  }
}
