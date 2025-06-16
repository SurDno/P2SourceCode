using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class SetEulerAnglesNode : FlowControlNode {
	private ValueInput<Transform> targetInput;
	private ValueInput<Vector3> eulerAnglesInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var transform = targetInput.value;
			if (transform != null)
				transform.eulerAngles = eulerAnglesInput.value;
			output.Call();
		});
		targetInput = AddValueInput<Transform>("Target");
		eulerAnglesInput = AddValueInput<Vector3>("EulerAngles");
	}
}