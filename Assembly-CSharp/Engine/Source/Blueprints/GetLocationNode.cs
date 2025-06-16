using Engine.Common;
using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class GetLocationNode : FlowControlNode {
	private ValueInput<IEntity> entityInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		entityInput = AddValueInput<IEntity>("Entity");
		AddValueOutput("Location", () => {
			var entity = entityInput.value;
			if (entity != null) {
				var component1 = entity.GetComponent<ILocationComponent>();
				if (component1 != null)
					return component1;
				var component2 = entity.GetComponent<ILocationItemComponent>();
				if (component2 != null)
					return component2.LogicLocation;
			}

			return null;
		});
	}
}