using ParadoxNotion;

namespace FlowCanvas.Nodes;

public abstract class CallableFunctionNode<TResult, T1, T2, T3> : CallableFunctionNodeBase {
	private TResult result;

	public abstract TResult Invoke(T1 a, T2 b, T3 c);

	protected sealed override void OnRegisterPorts(FlowNode node) {
		var o = node.AddFlowOutput(" ");
		var p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
		var p2 = node.AddValueInput<T2>(parameters[1].Name.SplitCamelCase());
		var p3 = node.AddValueInput<T3>(parameters[2].Name.SplitCamelCase());
		node.AddValueOutput("Value", () => result);
		node.AddFlowInput(" ", () => {
			result = Invoke(p1.value, p2.value, p3.value);
			o.Call();
		});
	}
}