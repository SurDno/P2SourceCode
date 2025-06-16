using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Math Operators/Integers")]
[Name("<=")]
public class IntegerLessEqualThan : PureFunctionNode<bool, int, int> {
	public override bool Invoke(int a, int b) {
		return a <= b;
	}
}