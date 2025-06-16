using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Interactable;
using Engine.Common.Services;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
public static class KnockDoorInteractValidator {
	[InteractValidator(InteractType.Knock)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		var component1 = interactable.GetComponent<IDoorComponent>();
		if (component1 == null)
			return new ValidateResult(false, "IDoorComponent not found");
		var component2 = ServiceLocator.GetService<ISimulation>().Player.GetComponent<LocationItemComponent>();
		if (component1.Bolted.Value)
			return new ValidateResult(false, "Bolted " + component1.Bolted.Value);
		if (!component1.Knockable.Value)
			return new ValidateResult(false, "Knockable " + component1.Knockable.Value);
		if (component1.LockState.Value != LockState.Locked)
			return new ValidateResult(false, "LockState " + component1.LockState.Value);
		if (component1.Opened.Value)
			return new ValidateResult(false, "Opened " + component1.Opened.Value);
		return component1.IsOutdoor && component2.IsIndoor
			? new ValidateResult(false, "IsOutdoor " + component1.IsOutdoor + " && IsIndoor " + component2.IsIndoor)
			: new ValidateResult(true);
	}
}