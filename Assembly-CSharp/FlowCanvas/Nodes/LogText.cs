using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Category("Functions/Utility")]
public class LogText : CallableActionNode<string> {
	public override void Invoke(string text) {
		Debug.Log(text);
	}
}