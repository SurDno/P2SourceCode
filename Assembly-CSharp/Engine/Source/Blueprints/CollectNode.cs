using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class CollectNode : FlowControlNode {
	private ValueInput<CollectControllerComponent> controllerInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			controllerInput.value?.Collect();
			output.Call();
		});
		controllerInput = AddValueInput<CollectControllerComponent>("Controller");
	}
}