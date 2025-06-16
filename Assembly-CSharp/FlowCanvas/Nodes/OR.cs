using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Math Operators/Boolean")]
public class OR : PureFunctionNode<bool, bool, bool> {
	public override bool Invoke(bool a, bool b) {
		return a | b;
	}
}