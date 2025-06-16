using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds;

[Category("Sounds")]
public class IsBuilding : FlowControlNode {
	[FromLocator] private ISimulation simulation;
	[Port("Building")] private ValueInput<BuildingEnum> buildingInput;

	[Port("Value")]
	private bool Value() {
		var player = simulation.Player;
		if (player != null) {
			var component = player.GetComponent<INavigationComponent>();
			if (component != null) {
				var building = component.Building;
				if (building != null)
					return building.Building == buildingInput.value;
			}
		}

		return false;
	}
}