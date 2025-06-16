using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class IsDeadEventNode : EventNode<FlowScriptController> {
	private FlowOutput deadOutput;
	private FlowOutput resurrectOutput;

	public override void OnGraphStarted() {
		base.OnGraphStarted();
		ServiceLocator.GetService<IsDeadListener>().OnIsDeadChanged += OnIsDeadChanged;
	}

	public override void OnGraphStoped() {
		ServiceLocator.GetService<IsDeadListener>().OnIsDeadChanged -= OnIsDeadChanged;
		base.OnGraphStoped();
	}

	private void OnIsDeadChanged(bool visible) {
		if (visible)
			deadOutput.Call();
		else
			resurrectOutput.Call();
	}

	protected override void RegisterPorts() {
		base.RegisterPorts();
		deadOutput = AddFlowOutput("Dead");
		resurrectOutput = AddFlowOutput("Resurrect");
	}
}