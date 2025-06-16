using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Name("On Variable Change")]
[Category("Events/Other")]
[Description("Fires Out when the target variable change. (Not whenever it is set)")]
public class VariableChangedEvent : EventNode {
	[BlackboardOnly] public BBParameter<object> targetVariable;
	private FlowOutput outFlow;

	public override string name => string.Format("{0} [{1}]", base.name, targetVariable);

	public override void OnGraphStarted() {
		if (targetVariable.varRef == null)
			return;
		targetVariable.varRef.onValueChanged += OnChanged;
	}

	public override void OnGraphStoped() {
		if (targetVariable.varRef == null)
			return;
		targetVariable.varRef.onValueChanged -= OnChanged;
	}

	protected override void RegisterPorts() {
		outFlow = AddFlowOutput("Out");
	}

	private void OnChanged(string name, object value) {
		outFlow.Call();
	}
}