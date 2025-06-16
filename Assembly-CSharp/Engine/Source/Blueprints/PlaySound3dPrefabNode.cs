using System;
using Engine.Source.Audio;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using SoundPropagation;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class PlaySound3dPrefabNode : FlowControlNode {
	[Port("Prefab")] private ValueInput<GameObject> sourcePrefab;
	[Port("Volume", 1f)] private ValueInput<float> volumeInput;
	[Port("FadeTime", 0.0f)] private ValueInput<float> fadeTime;
	[Port("Use Pause", true)] private ValueInput<bool> usePause;
	[Port("Target")] private ValueInput<GameObject> target;
	[Port("Mixer", null)] private ValueInput<AudioMixerGroup> mixerInput;
	[Port("Clip", null)] private ValueInput<AudioClip> clipInput;
	[Port("Propagation")] private ValueInput<bool> propagationInput;
	private AudioSource source;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var clip = clipInput.value;
			if (!(clip != null))
				return;
			var mixer = mixerInput.value;
			if (mixer != null) {
				var gameObject = target.value;
				if (gameObject != null) {
					var unscaledTime = Time.unscaledTime;
					var transform = gameObject.transform;
					var audioState = SoundUtility.PlayAudioClip(sourcePrefab.value, transform, clip, mixer,
						volumeInput.value, fadeTime.value, usePause.value, "(blueprint) " + graph.agent.name,
						(Action)(() => output.Call()));
					if (propagationInput.value && transform != null)
						audioState.AudioSource.gameObject.AddComponent<SPAudioSource>().Origin = transform;
				}
			}
		});
	}
}