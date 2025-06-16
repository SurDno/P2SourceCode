using Engine.Common.Services;
using Engine.Impl.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class SmallLoadingNode : FlowControlNode {
	private ValueInput<bool> visibleInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			ServiceLocator.GetService<UIService>().SmallLoading.gameObject.SetActive(visibleInput.value);
			output.Call();
		});
		visibleInput = AddValueInput<bool>("Visible");
	}
}