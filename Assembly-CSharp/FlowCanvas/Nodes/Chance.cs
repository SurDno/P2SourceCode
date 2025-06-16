using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Flow Controllers/Filters")]
[Description("Filter the Flow based on a chance of 0 to 1 for 0% - 100%")]
[ContextDefinedInputs(typeof(float))]
public class Chance : FlowControlNode {
	protected override void RegisterPorts() {
		var o = AddFlowOutput("Out");
		var c = AddValueInput<float>("Percentage");
		AddFlowInput("In", () => {
			if (UnityEngine.Random.Range(0.0f, 1f) >= (double)c.value)
				return;
			o.Call();
		});
	}
}