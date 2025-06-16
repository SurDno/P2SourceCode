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
public class PlaySound3dNode : FlowControlNode {
	[Port("Clip", null)] private ValueInput<AudioClip> clipInput;
	[Port("Mixer", null)] private ValueInput<AudioMixerGroup> mixerInput;
	[Port("Volume", 1f)] private ValueInput<float> volumeInput;
	[Port("Spatialize")] private ValueInput<bool> spatialize;
	[Port("Fade", 0.0f)] private ValueInput<float> fadeTime;
	[Port("Use Pause", true)] private ValueInput<bool> usePause;
	[Port("Target")] private ValueInput<GameObject> targetInput;
	[Port("MinDistance", 1f)] private ValueInput<float> minDistance;
	[Port("MaxDistance", 10f)] private ValueInput<float> maxDistance;
	[Port("Propagation")] private ValueInput<bool> propagationInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var clip = clipInput.value;
			if (clip == null)
				output.Call();
			else {
				var mixer = mixerInput.value;
				if (mixer == null)
					output.Call();
				else {
					var gameObject = targetInput.value;
					if (gameObject == null)
						output.Call();
					else {
						var transform = gameObject.transform;
						var audioState = SoundUtility.PlayAudioClip3D(transform, clip, mixer, volumeInput.value,
							minDistance.value, maxDistance.value, spatialize.value, fadeTime.value, usePause.value,
							"(blueprint) " + graph.agent.name, (Action)(() => output.Call()));
						if (!propagationInput.value)
							return;
						audioState.AudioSource.gameObject.AddComponent<SPAudioSource>().Origin = transform;
					}
				}
			}
		});
	}
}