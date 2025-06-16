using System.Collections;
using Engine.Source.Utility;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class WaitingPlayerCanControllingNode : FlowControlNode {
	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => StartCoroutine(WaitingPlayerCanControlling(output)));
	}

	private IEnumerator WaitingPlayerCanControlling(FlowOutput output) {
		while (!PlayerUtility.IsPlayerCanControlling)
			yield return null;
		output.Call();
	}
}