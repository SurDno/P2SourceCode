using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class DontStopLipSyncInPauseNode : FlowControlNode {
	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			InstanceByRequest<EngineApplication>.Instance.DontStopLipSyncInPause = true;
			output.Call();
		});
	}
}