using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class UnlockAchievementNode : FlowControlNode {
	private ValueInput<string> nameInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		AddFlowInput("In", () => ServiceLocator.GetService<IAchievementService>().Unlock(nameInput.value));
		nameInput = AddValueInput<string>("Name");
	}
}