using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Components.Maps;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class SetMapFocusedItemNode : FlowControlNode {
	private ValueInput<IEntity> targetInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			ServiceLocator.GetService<MapService>().FocusedItem = targetInput.value?.GetComponent<MapItemComponent>();
			output.Call();
		});
		targetInput = AddValueInput<IEntity>("Target");
	}
}