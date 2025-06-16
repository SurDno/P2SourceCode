using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class LerpNode : FlowControlNode {
	[Port("A")] private ValueInput<float> aInput;
	[Port("B")] private ValueInput<float> bInput;
	[Port("T")] private ValueInput<float> tInput;

	[Port("Value")]
	private float Value() {
		return Mathf.Lerp(aInput.value, bInput.value, tInput.value);
	}
}