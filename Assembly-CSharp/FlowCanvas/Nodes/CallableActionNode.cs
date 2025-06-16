namespace FlowCanvas.Nodes;

public abstract class CallableActionNode : CallableActionNodeBase {
	public abstract void Invoke();

	protected sealed override void OnRegisterPorts(FlowNode node) {
		var o = node.AddFlowOutput(" ");
		node.AddFlowInput(" ", () => {
			Invoke();
			o.Call();
		});
	}
}