using System.Collections;
using ParadoxNotion;

namespace FlowCanvas.Nodes;

public abstract class LatentActionNode<T1, T2, T3, T4, T5, T6, T7> : LatentActionNodeBase {
	public abstract IEnumerator Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g);

	protected sealed override void OnRegisterPorts(FlowNode node) {
		base.OnRegisterPorts(node);
		var p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
		var p2 = node.AddValueInput<T2>(parameters[1].Name.SplitCamelCase());
		var p3 = node.AddValueInput<T3>(parameters[2].Name.SplitCamelCase());
		var p4 = node.AddValueInput<T4>(parameters[3].Name.SplitCamelCase());
		var p5 = node.AddValueInput<T5>(parameters[4].Name.SplitCamelCase());
		var p6 = node.AddValueInput<T6>(parameters[5].Name.SplitCamelCase());
		var p7 = node.AddValueInput<T7>(parameters[6].Name.SplitCamelCase());
		node.AddFlowInput("In",
			() => Begin(Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value)));
		node.AddFlowInput("Break", () => Break());
	}
}