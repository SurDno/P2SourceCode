using ParadoxNotion;

namespace FlowCanvas.Nodes;

public abstract class CallableFunctionNode<TResult, T1, T2> : CallableFunctionNodeBase {
	private TResult result;

	public abstract TResult Invoke(T1 a, T2 b);

	protected sealed override void OnRegisterPorts(FlowNode node) {
		var o = node.AddFlowOutput(" ");
		var p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
		var p2 = node.AddValueInput<T2>(parameters[1].Name.SplitCamelCase());
		node.AddValueOutput("Value", () => result);
		node.AddFlowInput(" ", () => {
			result = Invoke(p1.value, p2.value);
			o.Call();
		});
	}
}