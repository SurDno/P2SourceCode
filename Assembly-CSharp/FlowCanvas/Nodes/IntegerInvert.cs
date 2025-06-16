using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Math Operators/Integers")]
[Name("Invert")]
[Description("Inverts the input ( value = value * -1 )")]
public class IntegerInvert : PureFunctionNode<int, int> {
	public override int Invoke(int value) {
		return value * -1;
	}
}