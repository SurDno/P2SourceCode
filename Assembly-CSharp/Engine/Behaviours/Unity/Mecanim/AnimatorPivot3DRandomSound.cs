using System;
using System.Collections.Generic;
using Cofe.Utility;
using Engine.Source.Audio;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Engine.Behaviours.Unity.Mecanim;

[DisallowMultipleComponent]
public class AnimatorPivot3DRandomSound : StateMachineBehaviour {
	[SerializeField] private List<AudioClip> Clips;
	[SerializeField] private AudioMixerGroup AudioMixer2;
	[SerializeField] private float Volume = 1f;
	[SerializeField] private float Delay;
	[SerializeField] private float MinDistance = 1f;
	[SerializeField] private float MaxDistance = 10f;

	public override void OnStateEnter(
		Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex) {
		if (Clips == null || Clips.Count == 0 || animator.GetLayerWeight(layerIndex) < 0.5)
			return;
		var clip = Clips[Random.Range(0, Clips.Count)];
		CoroutineService.Instance.WaitSecond(Delay, (Action)(() => {
			if (!(animator != null))
				return;
			SoundUtility.PlayAudioClip3D(animator.transform, clip, AudioMixer2, Volume, MinDistance, MaxDistance, true,
				0.0f, context: TypeUtility.GetTypeName(GetType()) + " " + name);
		}));
	}
}