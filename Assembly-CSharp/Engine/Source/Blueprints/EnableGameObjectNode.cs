using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class EnableGameObjectNode : FlowControlNode {
	private ValueInput<GameObject> goInput;
	private ValueInput<bool> enableInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var gameObject = goInput.value;
			if (gameObject != null)
				gameObject.SetActive(enableInput.value);
			output.Call();
		});
		goInput = AddValueInput<GameObject>("GameObject");
		enableInput = AddValueInput<bool>("Enable");
	}
}