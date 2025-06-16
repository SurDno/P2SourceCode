using System;
using System.Collections.Generic;
using Engine.Impl.Tasks;
using Inspectors;
using UnityEngine;

namespace Engine.Behaviours.Unity.Mecanim;

public class AnimatorState45 {
	private static int idleTagHash = Animator.StringToHash("Idle");
	private static int rotateTagHash = Animator.StringToHash("Rotate");
	private static int moveTagHash = Animator.StringToHash("Move");
	private static int moveStartTagHash = Animator.StringToHash("MoveStart");
	private static int moveStopTagHash = Animator.StringToHash("MoveStop");
	private static int poiTagHash = Animator.StringToHash("POI");
	private static int poiExitTagHash = Animator.StringToHash("POIExit");
	private static Dictionary<Animator, AnimatorState45> animatorStates = new(64);
	private HashSet<string> triggers = new();
	public Animator Animator;
	public float PrimaryIdleProbability = 0.5f;

	public event Action StopDoneEvent;

	[Inspected] public bool IsRotate => Animator.GetCurrentAnimatorStateInfo(0).tagHash == rotateTagHash;

	[Inspected]
	public bool IsMove {
		get {
			if (Animator == null)
				return false;
			var tagHash = Animator.GetCurrentAnimatorStateInfo(0).tagHash;
			return tagHash == moveTagHash || tagHash == moveStartTagHash || tagHash == moveStopTagHash;
		}
	}

	[Inspected] public bool IsMovementStart => Animator.GetCurrentAnimatorStateInfo(0).tagHash == moveStartTagHash;

	[Inspected] public bool IsMovementStop => Animator.GetCurrentAnimatorStateInfo(0).tagHash == moveStopTagHash;

	[Inspected]
	public bool IsPOI {
		get {
			var tagHash = Animator.GetCurrentAnimatorStateInfo(0).tagHash;
			return tagHash == poiTagHash || tagHash == poiExitTagHash;
		}
	}

	[Inspected] public bool IsPOIExit => Animator.GetCurrentAnimatorStateInfo(0).tagHash == poiExitTagHash;

	public bool MovableStop {
		set => Animator.SetBool("Movable.Stop", value);
	}

	[Inspected]
	public bool NextMoveIsLeft {
		get => Animator.GetBool("Movable.Next.Is.Left");
		set => Animator.SetBool("Movable.Next.Is.Left", value);
	}

	public void SetTrigger(string name) {
		Animator.SetTrigger(name);
		triggers.Add(name);
	}

	public void ResetTrigger(string name) {
		Animator.ResetTrigger(name);
		triggers.Remove(name);
	}

	public void ResetAllTriggers() {
		foreach (var trigger in triggers)
			Animator.ResetTrigger(trigger);
		triggers.Clear();
	}

	public void FireStopDoneEvent() {
		var stopDoneEvent = StopDoneEvent;
		if (stopDoneEvent == null)
			return;
		stopDoneEvent();
	}

	public MovableState45 ControlMovableState {
		set {
			if (!Animator.gameObject.activeSelf)
				return;
			Animator.ResetTrigger("Triggers/Base/Idle");
			Animator.ResetTrigger("Triggers/Base/Move");
			Animator.ResetTrigger("Triggers/Base/POI");
			Animator.ResetTrigger("Triggers/Base/Rotate");
			Animator.ResetTrigger("Triggers/Base/IdlePreset");
			switch (value) {
				case MovableState45.Idle:
					Animator.SetTrigger("Triggers/Base/Idle");
					Animator.SetInteger("Movable.State.Control", 1);
					break;
				case MovableState45.Rotate:
					Animator.SetTrigger("Triggers/Base/Rotate");
					Animator.SetInteger("Movable.State.Control", 2);
					break;
				case MovableState45.Move:
					Animator.SetTrigger("Triggers/Base/Move");
					Animator.SetInteger("Movable.State.Control", 3);
					break;
				case MovableState45.POI:
					Animator.SetTrigger("Triggers/Base/POI");
					Animator.SetInteger("Movable.State.Control", 6);
					break;
				case MovableState45.IdlePreset:
					Animator.SetTrigger("Triggers/Base/IdlePreset");
					Animator.SetInteger("Movable.State.Control", 6);
					break;
			}
		}
		get => (MovableState45)Animator.GetInteger("Movable.State.Control");
	}

	public MecanimKinds.MovablePOIStateKind ControlPOIState {
		set => Animator.SetInteger("Movable.POI.Control", (int)value);
	}

	public int ControlPOIMiddleAnimationsCount {
		set => Animator.SetInteger("Movable.POI.MiddleAnimationsCount", value);
	}

	public int ControlPOIAnimationIndex {
		set => Animator.SetInteger("Movable.POI.AnimationIndex", value);
	}

	public bool ControlPOIStartFromMiddle {
		set => Animator.SetBool("Movable.POI.StartFromMiddle", value);
	}

	public float StateLength => Animator.GetCurrentAnimatorStateInfo(0).length;

	public void ResetMovable() {
		Animator.SetTrigger("Movable.Cancel");
	}

	[Inspected] public float RemainingDistance { get; set; }

	[Inspected] public float VelocityScale { get; set; }

	public float MovableSpeed {
		get => Animator.GetFloat("Movable.Velocity.Z");
		set => Animator.SetFloat("Movable.Velocity.Z", value);
	}

	public float MovableAngleStart {
		set => Animator.SetFloat("Movable.Angle.Start", value);
		get => Animator.GetFloat("Movable.Angle.Start");
	}

	public static AnimatorState45 GetAnimatorState(Animator animator) {
		if (animator == null)
			return new AnimatorState45();
		AnimatorState45 animatorState;
		if (!animatorStates.TryGetValue(animator, out animatorState)) {
			animatorState = new AnimatorState45();
			animatorState.Animator = animator;
			animatorStates[animator] = animatorState;
		}

		return animatorState;
	}

	public enum MovableState45 {
		Unknown,
		Idle,
		Rotate,
		Move,
		POI,
		IdlePreset
	}
}