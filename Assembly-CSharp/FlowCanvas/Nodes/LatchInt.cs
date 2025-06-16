using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Name("Latch Integer")]
[Category("Flow Controllers/Flow Convert")]
[Description("Convert a Flow signal to an integer value")]
[ContextDefinedInputs(typeof(int))]
public class LatchInt : FlowControlNode, IMultiPortNode {
	[SerializeField] private int _portCount = 4;
	private int latched;

	public int portCount {
		get => _portCount;
		set => _portCount = value;
	}

	protected override void RegisterPorts() {
		var o = AddFlowOutput("Out");
		for (var index = 0; index < portCount; ++index) {
			var i = index;
			AddFlowInput(i.ToString(), () => {
				latched = i;
				o.Call();
			});
		}

		AddValueOutput("Value", () => latched);
	}
}