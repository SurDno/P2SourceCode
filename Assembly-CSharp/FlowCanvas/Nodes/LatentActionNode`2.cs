using System.Collections;
using ParadoxNotion;

namespace FlowCanvas.Nodes;

public abstract class LatentActionNode<T1, T2> : LatentActionNodeBase {
	public abstract IEnumerator Invoke(T1 a, T2 b);

	protected sealed override void OnRegisterPorts(FlowNode node) {
		base.OnRegisterPorts(node);
		var p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
		var p2 = node.AddValueInput<T2>(parameters[1].Name.SplitCamelCase());
		node.AddFlowInput("In", () => Begin(Invoke(p1.value, p2.value)));
		node.AddFlowInput("Break", () => Break());
	}
}