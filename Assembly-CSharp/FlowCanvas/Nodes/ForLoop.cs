using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;

namespace FlowCanvas.Nodes;

[Description("Perform a for loop")]
[Category("Flow Controllers/Iterators")]
[ContextDefinedInputs(typeof(int))]
[ContextDefinedOutputs(typeof(int))]
public class ForLoop : FlowControlNode {
	private int current;
	private bool broken;

	protected override void RegisterPorts() {
		var n = AddValueInput<int>("Loops");
		AddValueOutput("Index", () => current);
		var fCurrent = AddFlowOutput("Do");
		var fFinish = AddFlowOutput("Done");
		AddFlowInput("In", () => {
			current = 0;
			broken = false;
			for (var index = 0; index < n.value && !broken; ++index) {
				current = index;
				fCurrent.Call();
			}

			fFinish.Call();
		});
		AddFlowInput("Break", () => broken = true);
	}
}