using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class ResetTransformNode : FlowControlNode {
	private ValueInput<Transform> transformInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var transform = transformInput.value;
			if (transform != null)
				transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			output.Call();
		});
		transformInput = AddValueInput<Transform>("Transform");
	}
}