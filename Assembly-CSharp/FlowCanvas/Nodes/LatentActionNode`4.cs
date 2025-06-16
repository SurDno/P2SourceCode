using System.Collections;
using ParadoxNotion;

namespace FlowCanvas.Nodes;

public abstract class LatentActionNode<T1, T2, T3, T4> : LatentActionNodeBase {
	public abstract IEnumerator Invoke(T1 a, T2 b, T3 c, T4 d);

	protected sealed override void OnRegisterPorts(FlowNode node) {
		base.OnRegisterPorts(node);
		var p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
		var p2 = node.AddValueInput<T2>(parameters[1].Name.SplitCamelCase());
		var p3 = node.AddValueInput<T3>(parameters[2].Name.SplitCamelCase());
		var p4 = node.AddValueInput<T4>(parameters[3].Name.SplitCamelCase());
		node.AddFlowInput("In", () => Begin(Invoke(p1.value, p2.value, p3.value, p4.value)));
		node.AddFlowInput("Break", () => Break());
	}
}