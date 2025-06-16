using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Utility")]
[Description("Returns either one of the two inputs, based on the boolean condition")]
public class SwitchValue<T> : PureFunctionNode<T, bool, T, T> {
	public override T Invoke(bool condition, T isTrue, T isFalse) {
		return condition ? isTrue : isFalse;
	}
}