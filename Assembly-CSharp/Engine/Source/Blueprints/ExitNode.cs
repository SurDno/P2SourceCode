using Engine.Source.Commons;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class ExitNode : FlowControlNode {
	[Port("In")]
	private void In() {
		InstanceByRequest<EngineApplication>.Instance.Exit();
	}
}