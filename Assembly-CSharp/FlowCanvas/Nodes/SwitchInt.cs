using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Name("Switch Integer")]
[Category("Flow Controllers/Switchers")]
[Description(
	"Branch the Flow based on an integer value. The Default output is called when the Index value is out of range.")]
[ContextDefinedInputs(typeof(int))]
public class SwitchInt : FlowControlNode, IMultiPortNode {
	[SerializeField] private int _portCount = 4;

	public int portCount {
		get => _portCount;
		set => _portCount = value;
	}

	protected override void RegisterPorts() {
		var index = AddValueInput<int>("Index");
		var outs = new List<FlowOutput>();
		for (var index1 = 0; index1 < portCount; ++index1)
			outs.Add(AddFlowOutput(index1.ToString()));
		var def = AddFlowOutput("Default");
		AddFlowInput("In", () => {
			var index2 = index.value;
			if (index2 >= 0 && index2 < outs.Count)
				outs[index2].Call();
			else
				def.Call();
		});
	}
}