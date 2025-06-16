using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
public static class FastTravelInteractValidator {
	[InteractValidator(InteractType.FastTravel)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		var component = interactable.GetComponent<ParametersComponent>();
		if (component == null)
			return new ValidateResult(false, "ParametersComponent not found");
		var byName = component.GetByName<bool>(ParameterNameEnum.CanFastTravel);
		return byName == null || !byName.Value
			? new ValidateResult(false, "CanFastTravel is false")
			: new ValidateResult(true);
	}
}