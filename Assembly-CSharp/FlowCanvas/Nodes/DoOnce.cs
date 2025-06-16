using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Flow Controllers/Filters")]
[Description("Filters Out to be called only once. After the first call, Out is no longer called until Reset is called")]
public class DoOnce : FlowControlNode {
	private bool called;

	public override void OnGraphStarted() {
		called = false;
	}

	protected override void RegisterPorts() {
		var o = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			if (called)
				return;
			called = true;
			o.Call();
		});
		AddFlowInput("Reset", () => called = false);
	}
}