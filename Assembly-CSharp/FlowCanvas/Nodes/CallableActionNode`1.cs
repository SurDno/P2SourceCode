using ParadoxNotion;

namespace FlowCanvas.Nodes;

public abstract class CallableActionNode<T1> : CallableActionNodeBase {
	public abstract void Invoke(T1 a);

	protected sealed override void OnRegisterPorts(FlowNode node) {
		var o = node.AddFlowOutput(" ");
		var p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
		node.AddFlowInput(" ", () => {
			Invoke(p1.value);
			o.Call();
		});
	}
}