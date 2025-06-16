using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class CopyTransformNode : FlowControlNode {
	private ValueInput<Transform> fromInput;
	private ValueInput<Transform> toInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var transform1 = fromInput.value;
			var transform2 = toInput.value;
			if (transform1 != null && transform2 != null)
				transform2.SetPositionAndRotation(transform1.position, transform1.rotation);
			output.Call();
		});
		fromInput = AddValueInput<Transform>("From");
		toInput = AddValueInput<Transform>("To");
	}
}