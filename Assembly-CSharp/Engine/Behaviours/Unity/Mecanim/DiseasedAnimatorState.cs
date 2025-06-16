using System.Collections.Generic;
using UnityEngine;

namespace Engine.Behaviours.Unity.Mecanim;

public class DiseasedAnimatorState {
	private static Dictionary<Animator, DiseasedAnimatorState> diseasedAnimatorStates = new();
	public Animator Animator;

	public static DiseasedAnimatorState GetAnimatorState(Animator animator) {
		DiseasedAnimatorState animatorState;
		if (!diseasedAnimatorStates.TryGetValue(animator, out animatorState)) {
			animatorState = new DiseasedAnimatorState();
			animatorState.Animator = animator;
			diseasedAnimatorStates[animator] = animatorState;
		}

		return animatorState;
	}

	public void TriggerPlayerPush() {
		Animator.SetTrigger("AttackerDiseased.Player.Push.Trigger");
	}

	public void TriggerPlayerFendOff() {
		Animator.SetTrigger("AttackerDiseased.Player.FendOff.Trigger");
	}

	public float PlayerPushAngle {
		set => Animator.SetFloat("AttackerDiseased.Player.Push.Angle", value);
		get => Animator.GetFloat("AttackerDiseased.Player.Push.Angle");
	}
}