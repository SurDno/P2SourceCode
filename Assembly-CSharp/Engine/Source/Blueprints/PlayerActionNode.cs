using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class PlayerActionNode : FlowControlNode {
	private ValueInput<ActionEnum> actionInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			ServiceLocator.GetService<ISimulation>().Player?.GetComponent<PlayerControllerComponent>()
				?.ComputeAction(actionInput.value);
			output.Call();
		});
		actionInput = AddValueInput<ActionEnum>("Action");
	}
}