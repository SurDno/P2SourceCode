using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class ChromaticAbberationEffectNode : FlowControlNode, IUpdatable {
	[Port("Intensity")] private ValueInput<float> intensityInput;
	private float prevIntensity;
	private PostProcessingStackOverride postProcessingOverride;

	public void Update() {
		if (postProcessingOverride == null)
			GetOverrideColorGrading();
		else {
			var num = intensityInput.value;
			if (prevIntensity == (double)num)
				return;
			postProcessingOverride.ChromaticAberration.Intensity = num;
			prevIntensity = num;
		}
	}

	private void GetOverrideColorGrading() {
		postProcessingOverride = GameCamera.Instance.GamePostProcessingOverride;
		if (!(postProcessingOverride != null))
			return;
		postProcessingOverride.ChromaticAberration.Override = true;
		postProcessingOverride.ChromaticAberration.Enabled = true;
	}

	public override void OnDestroy() {
		base.OnDestroy();
		if (!(postProcessingOverride != null))
			return;
		postProcessingOverride.ChromaticAberration.Override = false;
	}
}