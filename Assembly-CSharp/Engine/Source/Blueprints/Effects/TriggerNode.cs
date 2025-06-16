using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class TriggerNode : FlowControlNode {
	private bool value;

	[Port("Set")]
	public void Set() {
		value = true;
	}

	[Port("Reset")]
	public void Reset() {
		value = false;
	}

	[Port("Value")]
	private bool Value() {
		return value;
	}
}