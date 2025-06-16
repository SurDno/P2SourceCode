using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Description("Can be used with a RelayOutput of the same (T) type to get the input port value.")]
[Category("Flow Controllers/Relay")]
public class RelayValueInput<T> : FlowControlNode {
	[Tooltip("The identifier name of the relay")]
	public string identifier = "MyRelayValueName";

	[HideInInspector] public ValueInput<T> port { get; private set; }

	public override string name => string.Format("@ {0}", identifier);

	protected override void RegisterPorts() {
		port = AddValueInput<T>("Value");
	}
}