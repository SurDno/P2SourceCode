using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class VignetteEffectNode : FlowControlNode, IUpdatable {
	[Port("Value")] private ValueInput<IntensityParameter<Color>> valueInput;
	private PostProcessingStackOverride postProcessingOverride;

	public void Update() {
		if (postProcessingOverride == null) {
			postProcessingOverride = GameCamera.Instance.GamePostProcessingOverride;
			if (postProcessingOverride != null) {
				postProcessingOverride.Vignette.Override = true;
				postProcessingOverride.Vignette.Enabled = true;
			}
		}

		if (postProcessingOverride == null)
			return;
		postProcessingOverride.Vignette.Intensity = valueInput.value.Intensity;
		postProcessingOverride.Vignette.Color = valueInput.value.Value;
	}

	public override void OnDestroy() {
		base.OnDestroy();
		if (!(postProcessingOverride != null))
			return;
		postProcessingOverride.Vignette.Override = false;
	}
}