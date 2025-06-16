using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
public static class BrokenInteractValidator {
	[InteractValidator(InteractType.Broken)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		var component = interactable.GetComponent<ParametersComponent>();
		if (component == null)
			return new ValidateResult(false, "ParametersComponent not found");
		var byName = component.GetByName<float>(ParameterNameEnum.Durability);
		if (byName == null)
			return new ValidateResult(false, "Durability not found");
		return byName.Value > 0.0 ? new ValidateResult(false, "Durability " + byName.Value) : new ValidateResult(true);
	}
}