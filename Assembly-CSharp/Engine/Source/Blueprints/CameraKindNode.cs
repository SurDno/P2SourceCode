using Engine.Common.Services;
using Engine.Source.Services.CameraServices;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class CameraKindNode : FlowControlNode {
	private ValueInput<CameraKindEnum> cameraKindValue;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			ServiceLocator.GetService<CameraService>().Kind = cameraKindValue.value;
			output.Call();
		});
		cameraKindValue = AddValueInput<CameraKindEnum>("Kind");
	}
}