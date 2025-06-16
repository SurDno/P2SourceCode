using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
public static class OpenCloseDoorInteractValidator {
	[InteractValidator(InteractType.OpenDoor)]
	[InteractValidator(InteractType.CloseDoor)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		var component = interactable.GetComponent<IDoorComponent>();
		if (component == null)
			return new ValidateResult(false, "IDoorComponent not found");
		if (component.Bolted.Value)
			return new ValidateResult(false, "Bolted " + component.Bolted.Value);
		if (component.LockState.Value != 0)
			return new ValidateResult(false, "LockState " + component.LockState.Value);
		if (component.Opened.Value != (item.Type == InteractType.OpenDoor))
			return new ValidateResult(true);
		return new ValidateResult(false, "Opened " + component.Opened.Value + " != Type " + item.Type);
	}
}