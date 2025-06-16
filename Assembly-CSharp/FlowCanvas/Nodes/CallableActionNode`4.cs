using ParadoxNotion;

namespace FlowCanvas.Nodes;

public abstract class CallableActionNode<T1, T2, T3, T4> : CallableActionNodeBase {
	public abstract void Invoke(T1 a, T2 b, T3 c, T4 d);

	protected sealed override void OnRegisterPorts(FlowNode node) {
		var o = node.AddFlowOutput(" ");
		var p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
		var p2 = node.AddValueInput<T2>(parameters[1].Name.SplitCamelCase());
		var p3 = node.AddValueInput<T3>(parameters[2].Name.SplitCamelCase());
		var p4 = node.AddValueInput<T4>(parameters[3].Name.SplitCamelCase());
		node.AddFlowInput(" ", () => {
			Invoke(p1.value, p2.value, p3.value, p4.value);
			o.Call();
		});
	}
}