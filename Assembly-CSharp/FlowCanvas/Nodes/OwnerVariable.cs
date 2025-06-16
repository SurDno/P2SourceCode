using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Name("Self")]
[Description("Returns the Owner GameObject")]
public class OwnerVariable : VariableNode {
	public override string name => "<size=20>SELF</size>";

	protected override void RegisterPorts() {
		AddValueOutput("Value", () => (bool)(Object)graphAgent ? graphAgent.gameObject : null);
	}

	public override void SetVariable(object o) { }
}