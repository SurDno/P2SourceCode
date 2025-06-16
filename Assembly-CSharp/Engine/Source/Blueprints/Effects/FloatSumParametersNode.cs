using System.Collections.Generic;
using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class FloatSumParametersNode : FlowControlNode {
	[Port("Value")] private ValueInput<IList<IParameter<float>>> parametersInput;

	[Port("Value")]
	private float Value() {
		var num = 0.0f;
		var parameterList = parametersInput.value;
		if (parameterList != null)
			for (var index = 0; index < parameterList.Count; ++index) {
				var parameter = parameterList[index];
				num += parameter.Value;
			}

		return num;
	}
}