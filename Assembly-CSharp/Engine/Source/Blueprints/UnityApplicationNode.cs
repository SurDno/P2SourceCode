using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class UnityApplicationNode : FlowControlNode {
	private ValueInput<bool> inputValue;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			InstanceByRequest<EngineApplication>.Instance.IsPaused = inputValue.value;
			output.Call();
		});
		inputValue = AddValueInput<bool>("IsPause");
	}
}