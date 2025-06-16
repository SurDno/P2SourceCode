using System.Collections.Generic;
using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Description("Calls one random output each time In is called")]
[ContextDefinedOutputs(typeof(int))]
public class Random : FlowControlNode, IMultiPortNode {
	[SerializeField] private int _portCount = 4;
	private int current;

	public int portCount {
		get => _portCount;
		set => _portCount = value;
	}

	protected override void RegisterPorts() {
		var outs = new List<FlowOutput>();
		for (var index = 0; index < portCount; ++index)
			outs.Add(AddFlowOutput(index.ToString()));
		AddFlowInput("In", () => {
			current = UnityEngine.Random.Range(0, portCount);
			outs[current].Call();
		});
		AddValueOutput("Current", () => current);
	}
}