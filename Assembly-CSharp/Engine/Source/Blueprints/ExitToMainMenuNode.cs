using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class ExitToMainMenuNode : FlowControlNode {
	[Port("In")]
	private void In() {
		ServiceLocator.GetService<GameLauncher>().ExitToMainMenu();
	}
}