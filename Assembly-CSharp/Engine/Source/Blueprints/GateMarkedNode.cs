using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class GateMarkedNode : FlowControlNode {
	private ValueInput<IDoorComponent> gateInput;
	private ValueInput<bool> marketInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var doorComponent = gateInput.value;
			if (doorComponent != null)
				doorComponent.Marked.Value = marketInput.value;
			output.Call();
		});
		gateInput = AddValueInput<IDoorComponent>("Gate");
		marketInput = AddValueInput<bool>("Marked");
	}
}