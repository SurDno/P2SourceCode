using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Events")]
[Color("5cdd5c")]
public abstract class EventNode : FlowNode {
	public override string name => string.Format("{0}", base.name.ToUpper());
}