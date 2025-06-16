using System.Collections.Generic;
using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class VignetteBlenderNode : FlowControlNode {
	[Port("Value")] private ValueInput<IList<IParameter<IntensityParameter<Color>>>> intensityParametersInput;

	[Port("Value")]
	private IntensityParameter<Color> Value() {
		var num1 = 0.0f;
		var num2 = 0.0f;
		var black = Color.black;
		var parameterList = intensityParametersInput.value;
		if (parameterList != null) {
			for (var index = 0; index < parameterList.Count; ++index) {
				var intensity = parameterList[index].Value.Intensity;
				if (num1 < (double)intensity)
					num1 = intensity;
				num2 += intensity;
			}

			if (num2 == 0.0)
				return new IntensityParameter<Color> {
					Intensity = 0.0f,
					Value = black
				};
			for (var index = 0; index < parameterList.Count; ++index) {
				var parameter = parameterList[index];
				var intensity = parameter.Value.Intensity;
				var color = parameter.Value.Value;
				black += color * intensity / num2;
			}
		}

		return new IntensityParameter<Color> {
			Intensity = num1,
			Value = black
		};
	}
}