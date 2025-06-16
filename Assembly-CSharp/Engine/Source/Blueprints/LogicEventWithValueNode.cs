using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class LogicEventWithValueNode : FlowControlNode {
	[Port("EventName")] private ValueInput<string> eventNameInput;
	[Port("Value")] private ValueInput<string> eventValueInput;
	[Port("Out")] private FlowOutput output;

	[Port("In")]
	private void In() {
		var name = eventNameInput.value;
		if (name != null)
			ServiceLocator.GetService<LogicEventService>().FireValueEvent(name, eventValueInput.value);
		output.Call();
	}
}