using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Services.Utilities;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class OpenCraftBreweryWindowNode : FlowControlNode {
	private ValueInput<IStorageComponent> targetInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var target = targetInput.value;
			if (target == null)
				return;
			UIServiceUtility.PushWindow<ICraftBreweryWindow>(output, window => {
				window.Actor = ServiceLocator.GetService<ISimulation>().Player.GetComponent<IStorageComponent>();
				window.Target = target;
			});
		});
		targetInput = AddValueInput<IStorageComponent>("Storage");
	}
}