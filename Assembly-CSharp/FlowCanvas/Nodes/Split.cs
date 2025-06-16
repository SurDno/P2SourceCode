using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Description("Split the Flow in multiple directions. Calls all outputs in the same frame but in order")]
public class Split : FlowControlNode, IMultiPortNode {
	[SerializeField] private int _portCount = 4;

	public int portCount {
		get => _portCount;
		set => _portCount = value;
	}

	protected override void RegisterPorts() {
		var outs = new List<FlowOutput>();
		for (var index = 0; index < portCount; ++index)
			outs.Add(AddFlowOutput(index.ToString()));
		AddFlowInput("In", () => {
			for (var index = 0; index < portCount; ++index)
				outs[index].Call();
		});
	}
}