using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Category("Functions/Math Operators/Vector3")]
[Name("==")]
public class Vector3Equal : PureFunctionNode<bool, Vector3, Vector3> {
	public override bool Invoke(Vector3 a, Vector3 b) {
		return a == b;
	}
}