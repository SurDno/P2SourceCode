using Engine.Common;
using Engine.Common.Components.Locations;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class FindRegionLocation : FlowControlNode {
	private ValueInput<IEntity> entityInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		entityInput = AddValueInput<IEntity>("Entity");
		AddValueOutput("Location", () => {
			for (var parent = entityInput.value; parent != null; parent = parent.Parent) {
				var component = parent.GetComponent<LocationComponent>();
				if (component != null && component.LocationType == LocationType.Region)
					return component.Owner;
			}

			return null;
		});
	}
}