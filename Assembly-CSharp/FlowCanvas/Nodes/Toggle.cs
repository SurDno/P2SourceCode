using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Flow Controllers/Togglers")]
[Description(
	"When In is called, calls On or Off depending on the current toggle state. Whenever Toggle input is called the state changes.")]
public class Toggle : FlowControlNode {
	public bool open = true;
	private bool original;

	public override string name => base.name + " " + (open ? "[ON]" : "[OFF]");

	public override void OnGraphStarted() {
		original = open;
	}

	public override void OnGraphStoped() {
		open = original;
	}

	protected override void RegisterPorts() {
		var tOut = AddFlowOutput("On");
		var fOut = AddFlowOutput("Off");
		AddFlowInput("In", () => Call(open ? tOut : fOut));
		AddFlowInput(nameof(Toggle), () => open = !open);
	}
}