using Engine.Common;
using Engine.Common.Services;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class PlayerNode : FlowControlNode {
	[Port("Player")]
	public IEntity Player() {
		return ServiceLocator.GetService<ISimulation>().Player;
	}
}