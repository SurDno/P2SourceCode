using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Description("Enumerate a value (usualy a list or array) for each of it's elements")]
[Category("Flow Controllers/Iterators")]
public class ForEach<T> : FlowControlNode {
	private T current;
	private bool broken;

	protected override void RegisterPorts() {
		var list = AddValueInput<IEnumerable<T>>("Value");
		AddValueOutput("Current", () => current);
		var fCurrent = AddFlowOutput("Do");
		var fFinish = AddFlowOutput("Done");
		AddFlowInput("In", () => {
			var objs = list.value;
			if (objs == null)
				fFinish.Call();
			else {
				broken = false;
				foreach (var obj in objs)
					if (!broken) {
						current = obj;
						fCurrent.Call();
					} else
						break;

				fFinish.Call();
			}
		});
		AddFlowInput("Break", () => broken = true);
	}
}