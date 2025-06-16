using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Math Operators/Floats")]
[Name("<=")]
public class FloatLessEqualThan : PureFunctionNode<bool, float, float> {
	public override bool Invoke(float a, float b) {
		return a <= (double)b;
	}
}