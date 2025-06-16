using System.Linq;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Category("Functions/Utility")]
public class GetChildTransforms : PureFunctionNode<Transform[], Transform> {
	public override Transform[] Invoke(Transform parent) {
		return parent.transform.Cast<Transform>().ToArray();
	}
}