using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Math Operators")]
[Name("Is Valid")]
public class IsNotNull : PureFunctionNode<bool, object> {
	public override bool Invoke(object OBJECT) {
		return OBJECT != null;
	}
}