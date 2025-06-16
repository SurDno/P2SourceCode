using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects;

[Category("Engine")]
public class FloatCeilToIntNode : PureFunctionNode<int, float> {
	public override int Invoke(float a) {
		return Mathf.CeilToInt(a);
	}
}