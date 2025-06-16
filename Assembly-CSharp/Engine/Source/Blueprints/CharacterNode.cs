using Engine.Common.Services;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
[Description("НЕ ИСПОЛЬЗОВАТЬ, ИСПОЛЬЗОВАТЬ Player")]
[Color("FF0000")]
public class CharacterNode : FlowControlNode {
	protected override void RegisterPorts() {
		base.RegisterPorts();
		AddValueOutput("Character", () => ServiceLocator.GetService<ISimulation>().Player);
	}
}