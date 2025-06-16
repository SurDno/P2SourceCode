using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class MoveTransformNode : FlowControlNode {
	private ValueInput<Transform> targetInput;
	private ValueInput<Vector3> offsetInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var transform = targetInput.value;
			if (transform != null) {
				var vector3 = transform.transform.position + offsetInput.value;
				transform.transform.position = vector3;
			}

			output.Call();
		});
		targetInput = AddValueInput<Transform>("Target");
		offsetInput = AddValueInput<Vector3>("Offset");
	}
}