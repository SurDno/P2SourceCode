using Engine.Impl.UI.Menu.Main;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class LoadWindowVisibleNode : FlowControlNode {
	private ValueInput<bool> visibleInput;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		AddFlowOutput("Out");
		AddFlowInput("In", () => LoadWindow.Instance.Show = visibleInput.value);
		visibleInput = AddValueInput<bool>("Visible");
	}
}