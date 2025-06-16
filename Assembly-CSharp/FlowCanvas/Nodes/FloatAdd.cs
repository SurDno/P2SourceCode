using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Math Operators/Floats")]
[Name("+")]
public class FloatAdd : PureFunctionNode<float, float, float> {
	public override float Invoke(float a, float b) {
		return a + b;
	}
}