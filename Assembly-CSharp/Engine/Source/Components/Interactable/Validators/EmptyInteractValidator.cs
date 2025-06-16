using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
public static class EmptyInteractValidator {
	[InteractValidator(InteractType.Empty)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		var component = interactable.GetComponent<ParametersComponent>();
		if (component == null)
			return new ValidateResult(false, "ParametersComponent not found");
		var byName = component.GetByName<int>(ParameterNameEnum.Bullets);
		if (byName == null)
			return new ValidateResult(false, "Bullets not found");
		return byName.Value > 0 ? new ValidateResult(false, "Bullets not empty") : new ValidateResult(true);
	}
}