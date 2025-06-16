using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class BlurEffectNode : FlowControlNode, IUpdatable {
	[Port("Value")] private ValueInput<float> valueInput;
	private MotionTrail motionTrail;

	public void Update() {
		if (motionTrail == null)
			motionTrail = GameCamera.Instance.Camera.GetComponent<MotionTrail>();
		if (motionTrail == null)
			return;
		motionTrail.enabled = valueInput.value > 1.0 / 256.0;
		motionTrail.Strength = valueInput.value;
	}

	public override void OnDestroy() {
		base.OnDestroy();
		if (!(bool)(Object)motionTrail)
			return;
		motionTrail.enabled = false;
	}
}