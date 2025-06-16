using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Utility")]
[Description("Caches the value only when the node is called.")]
public class Cache<T> : CallableFunctionNode<T, T> {
	public override T Invoke(T value) {
		return value;
	}
}