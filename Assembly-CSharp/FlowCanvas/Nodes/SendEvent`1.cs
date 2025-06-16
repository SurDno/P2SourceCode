using ParadoxNotion;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Utility")]
public class SendEvent<T> : CallableActionNode<FlowScriptController, string, T> {
	public override void Invoke(FlowScriptController target, string eventName, T eventValue) {
		target.SendEvent(new EventData<T>(eventName, eventValue));
	}
}