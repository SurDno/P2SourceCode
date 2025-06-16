using ParadoxNotion;

namespace FlowCanvas.Nodes;

public abstract class PureFunctionNode<TResult, T1> : PureFunctionNodeBase {
	public abstract TResult Invoke(T1 a);

	protected sealed override void OnRegisterPorts(FlowNode node) {
		var p1 = node.AddValueInput<T1>(parameters[0].Name.SplitCamelCase());
		node.AddValueOutput("Value", () => Invoke(p1.value));
	}
}