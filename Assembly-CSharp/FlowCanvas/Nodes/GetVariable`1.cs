using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Name("Graph Variable<T>")]
[Category("Variables")]
[Description(
	"Returns a constant or linked variable value.\nYou can alter between constant or linked at any time using the radio button.")]
[AppendListTypes]
public class GetVariable<T> : VariableNode {
	public BBParameter<T> value;

	protected override void RegisterPorts() {
		AddValueOutput("Value", () => value.value);
	}

	public void SetTargetVariableName(string name) {
		value.name = name;
	}

	public override void SetVariable(object o) {
		switch (o) {
			case Variable<T> _:
				value.name = (o as Variable<T>).name;
				break;
			case T obj:
				value.value = obj;
				break;
			default:
				Debug.LogError("Set Variable Error");
				break;
		}
	}
}