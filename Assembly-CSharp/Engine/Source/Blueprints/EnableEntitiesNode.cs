using System.Collections.Generic;
using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class EnableEntitiesNode : FlowControlNode {
	[Port("Entities")] private ValueInput<IEnumerable<IEntity>> entitiesInput;
	[Port("Enable")] private ValueInput<bool> enableInput;
	[Port("Out")] private FlowOutput output;

	[Port("In")]
	private void In() {
		var entities = entitiesInput.value;
		if (entities != null)
			foreach (var entity in entities)
				entity.IsEnabled = enableInput.value;
		output.Call();
	}
}