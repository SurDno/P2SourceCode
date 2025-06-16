using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class StelthEventNode : EventNode<FlowScriptController> {
	private FlowOutput showOutput;
	private FlowOutput hideOutput;

	public override void OnGraphStarted() {
		base.OnGraphStarted();
		ServiceLocator.GetService<StelthEnableListener>().OnVisibleChanged += OnVisibleChanged;
	}

	public override void OnGraphStoped() {
		ServiceLocator.GetService<StelthEnableListener>().OnVisibleChanged -= OnVisibleChanged;
		base.OnGraphStoped();
	}

	private void OnVisibleChanged(bool visible) {
		if (visible)
			showOutput.Call();
		else
			hideOutput.Call();
	}

	protected override void RegisterPorts() {
		base.RegisterPorts();
		showOutput = AddFlowOutput("Show");
		hideOutput = AddFlowOutput("Hide");
	}
}