using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Math Operators/Integers")]
[Name("-")]
public class IntegerSubtract : PureFunctionNode<int, int, int> {
	public override int Invoke(int a, int b) {
		return a - b;
	}
}