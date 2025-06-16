namespace FlowCanvas.Nodes;

public abstract class CallableFunctionNode<TResult> : CallableFunctionNodeBase {
	private TResult result;

	public abstract TResult Invoke();

	protected sealed override void OnRegisterPorts(FlowNode node) {
		var o = node.AddFlowOutput(" ");
		node.AddValueOutput("Value", () => result);
		node.AddFlowInput(" ", () => {
			result = Invoke();
			o.Call();
		});
	}
}