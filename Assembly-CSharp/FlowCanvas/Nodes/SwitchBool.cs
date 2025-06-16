using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Name("Switch Condition")]
[Category("Flow Controllers/Switchers")]
[Description("Branch the Flow based on a conditional boolean value")]
[ContextDefinedInputs(typeof(bool))]
public class SwitchBool : FlowControlNode {
	protected override void RegisterPorts() {
		var c = AddValueInput<bool>("Condition");
		var tOut = AddFlowOutput("True");
		var fOut = AddFlowOutput("False");
		AddFlowInput("In", () => Call(c.value ? tOut : fOut));
	}
}