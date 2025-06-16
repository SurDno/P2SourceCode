using System;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Connections;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class PlayLipSyncAudioNode : FlowControlNode {
	[Port("LipSync", null)] private ValueInput<LipSyncObjectSerializable> lipSyncInput;
	[Port("Mixer", null)] private ValueInput<AudioMixerGroup> mixerInput;
	[Port("Volume", 1f)] private ValueInput<float> volumeInput;
	[Port("Fade", 0.0f)] private ValueInput<float> fadeTime;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var lipSync = lipSyncInput.value.Value;
			if (lipSync == null)
				output.Call();
			else {
				var mixer = mixerInput.value;
				if (mixer == null)
					output.Call();
				else {
					var lipSyncInfo = LipSyncUtility.GetLipSyncInfo(lipSync);
					if (lipSyncInfo == null)
						output.Call();
					else {
						var clip = lipSyncInfo.Clip.Value;
						if (clip == null)
							output.Call();
						else {
							var state = SoundUtility.PlayAudioClip2D(clip, mixer, volumeInput.value, fadeTime.value,
								true, "(blueprint) " + graph.agent.name, (Action)(() => output.Call()));
							ServiceLocator.GetService<SubtitlesService>()
								.AddSubtitles(null, lipSyncInfo.Tag, state, graphAgent);
						}
					}
				}
			}
		});
	}
}