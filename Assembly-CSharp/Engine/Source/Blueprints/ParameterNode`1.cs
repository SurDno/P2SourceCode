using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;

namespace Engine.Source.Blueprints;

public abstract class ParameterNode<T> : FlowControlNode where T : struct {
	[Port("Parameters")] private ValueInput<ParametersComponent> parametersInput;
	[Port("Name")] private ValueInput<ParameterNameEnum> nameInput;

	[Port("Value")]
	private T Value() {
		var parametersComponent = parametersInput.value;
		if (parametersComponent != null) {
			var byName = parametersComponent.GetByName<T>(nameInput.value);
			if (byName != null)
				return byName.Value;
		}

		return default;
	}
}