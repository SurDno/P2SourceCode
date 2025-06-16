using System;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Flow Controllers/Switchers")]
[Description("Branch the Flow based on a comparison between two comparable objects")]
[ContextDefinedInputs(typeof(IComparable))]
public class SwitchComparison : FlowControlNode {
	protected override void RegisterPorts() {
		var equal = AddFlowOutput("==");
		var notEqual = AddFlowOutput("!=");
		var greater = AddFlowOutput(">");
		var less = AddFlowOutput("<");
		var a = AddValueInput<IComparable>("A");
		var b = AddValueInput<IComparable>("B");
		AddFlowInput("In", () => {
			var comparable1 = a.value;
			var comparable2 = b.value;
			if (comparable1 == null || comparable2 == null) {
				if (comparable1 == comparable2)
					equal.Call();
				if (comparable1 == comparable2)
					return;
				notEqual.Call();
			} else {
				if (comparable1.CompareTo(comparable2) == 0)
					equal.Call();
				else
					notEqual.Call();
				if (comparable1.CompareTo(comparable2) == 1)
					greater.Call();
				if (comparable1.CompareTo(comparable2) == -1)
					less.Call();
			}
		});
	}
}