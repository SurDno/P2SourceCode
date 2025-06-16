using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class IndoorGraphicsOverrideNode : FlowControlNode {
	private ValueInput<bool> insideIndoorInput;
	private ValueInput<bool> isolatedInput;
	private ValueInput<bool> cutsceneIsolatedInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var flag = insideIndoorInput.value;
			PlayerIndoorCheck.Override(flag);
			PlayerIsolatedIndoorCheck.Override(flag && isolatedInput.value);
			CutsceneIndoorCheck.Set(cutsceneIsolatedInput.value);
			output.Call();
		});
		insideIndoorInput = AddValueInput<bool>("InsideIndoor");
		isolatedInput = AddValueInput<bool>("Isolated");
		cutsceneIsolatedInput = AddValueInput<bool>("CutsceneIsolated");
	}
}