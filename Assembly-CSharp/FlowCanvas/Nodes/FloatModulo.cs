using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Math Operators/Floats")]
[Name("%")]
public class FloatModulo : PureFunctionNode<float, float, float> {
	public override float Invoke(float value, float mod) {
		return value % mod;
	}
}