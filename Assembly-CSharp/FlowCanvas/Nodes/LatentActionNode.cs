using System.Collections;

namespace FlowCanvas.Nodes;

public abstract class LatentActionNode : LatentActionNodeBase {
	public abstract IEnumerator Invoke();

	protected sealed override void OnRegisterPorts(FlowNode node) {
		base.OnRegisterPorts(node);
		node.AddFlowInput("In", () => Begin(Invoke()));
		node.AddFlowInput("Break", () => Break());
	}
}