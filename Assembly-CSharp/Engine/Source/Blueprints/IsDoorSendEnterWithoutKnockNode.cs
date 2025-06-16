using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class IsDoorSendEnterWithoutKnockNode : FlowControlNode {
	private ValueInput<IDoorComponent> doorInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		AddValueOutput("SendEnterWithoutKnock", () => {
			var doorComponent = doorInput.value;
			return doorComponent != null && doorComponent.SendEnterWithoutKnock.Value;
		});
		doorInput = AddValueInput<IDoorComponent>("Door");
	}
}