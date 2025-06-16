using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.VisualEffects;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints.Effects;

[Category("Effects")]
public class VignetteInputParametersNode : FlowControlNode {
	[Port("Name")] private ValueInput<string> nameInput;
	[FromLocator] private EffectsService effects;
	private List<IParameter<IntensityParameter<Color>>> result = new();

	[Port("Value")]
	private IList<IParameter<IntensityParameter<Color>>> Value() {
		effects.GetParameters(nameInput.value, result);
		return result;
	}
}