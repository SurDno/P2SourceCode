using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Math Operators/Integers")]
[Name("÷")]
public class IntegerDivide : PureFunctionNode<int, int, int> {
	public override int Invoke(int a, int b) {
		return b == 0 ? 0 : a / b;
	}
}