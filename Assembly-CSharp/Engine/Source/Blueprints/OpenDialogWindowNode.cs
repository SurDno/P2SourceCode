using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Services.Utilities;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class OpenDialogWindowNode : FlowControlNode {
	private ValueInput<ISpeakingComponent> targetInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var target = targetInput.value;
			if (target == null)
				return;
			if (target.SpeakAvailable)
				UIServiceUtility.PushWindow<IDialogWindow>(output, window => {
					window.Target = target;
					window.Actor = ServiceLocator.GetService<ISimulation>().Player.GetComponent<ISpeakingComponent>();
				});
			else {
				Debug.LogError("Speak is not available : " + target.Owner.GetInfo());
				output.Call();
			}
		});
		targetInput = AddValueInput<ISpeakingComponent>("Speaking");
	}
}