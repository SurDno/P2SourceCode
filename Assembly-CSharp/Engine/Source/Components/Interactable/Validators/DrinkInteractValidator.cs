using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
public class DrinkInteractValidator {
	[InteractValidator(InteractType.Drink)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		var validateResult1 = BrokenInteractValidator.Validate(interactable, item);
		if (validateResult1.Result)
			return new ValidateResult(false, validateResult1.Reason);
		var validateResult2 = EmptyInteractValidator.Validate(interactable, item);
		return validateResult2.Result
			? new ValidateResult(false, validateResult2.Reason)
			: new ValidateResult(true, "Not broken and not empty");
	}
}