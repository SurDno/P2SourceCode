using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class WaveDistortionEffectNode : FlowControlNode, IUpdatable {
	[Port("Value")] private ValueInput<float> valueInput;
	private WaveDistortion waveDistortion;

	public void Update() {
		if (waveDistortion == null)
			waveDistortion = GameCamera.Instance.Camera.GetComponent<WaveDistortion>();
		if (waveDistortion == null)
			return;
		waveDistortion.enabled = valueInput.value > 0.0;
		waveDistortion.Intensity = valueInput.value;
	}

	public override void OnDestroy() {
		base.OnDestroy();
		if (!(bool)(Object)waveDistortion)
			return;
		waveDistortion.enabled = false;
	}
}