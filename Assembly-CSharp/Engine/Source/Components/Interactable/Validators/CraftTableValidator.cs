using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
public class CraftTableValidator {
	[InteractValidator(InteractType.CraftTable)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		var validateResult = BrokenInteractValidator.Validate(interactable, item);
		return validateResult.Result ? new ValidateResult(false, validateResult.Reason) : new ValidateResult(true);
	}
}