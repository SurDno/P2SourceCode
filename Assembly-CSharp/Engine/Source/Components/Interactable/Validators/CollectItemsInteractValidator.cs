using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
public static class CollectItemsInteractValidator {
	[InteractValidator(InteractType.Collect)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		var component = interactable.GetComponent<CollectControllerComponent>();
		if (component == null)
			return new ValidateResult(false, "CollectControllerComponent not found");
		var flag = component.ValidateCollect();
		return !flag ? new ValidateResult(false, "ValidateCollect " + flag) : new ValidateResult(true);
	}
}