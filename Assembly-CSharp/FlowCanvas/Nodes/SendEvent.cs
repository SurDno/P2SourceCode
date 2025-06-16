using ParadoxNotion;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Utility")]
public class SendEvent : CallableActionNode<FlowScriptController, string> {
	public override void Invoke(FlowScriptController target, string eventName) {
		target.SendEvent(new EventData(eventName));
	}
}