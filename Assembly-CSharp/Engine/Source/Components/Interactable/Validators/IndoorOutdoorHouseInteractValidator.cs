using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
public static class IndoorOutdoorHouseInteractValidator {
	[InteractValidator(InteractType.Indoor)]
	[InteractValidator(InteractType.Outdoor)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		var component1 = interactable.GetComponent<IDoorComponent>();
		if (component1 == null)
			return new ValidateResult(false, "IDoorComponent not found");
		if (component1.Bolted.Value)
			return new ValidateResult(false, "Bolted " + component1.Bolted.Value);
		if (item.Type == InteractType.Outdoor) {
			LockState lockState;
			if (component1.LockState.TryGetValue(PriorityParameterEnum.Quest, out lockState) && lockState != 0)
				return new ValidateResult(false, "LockState Quest " + lockState);
		} else if (component1.LockState.Value != 0)
			return new ValidateResult(false, "LockState " + component1.LockState.Value);

		var component2 = ServiceLocator.GetService<ISimulation>().Player.GetComponent<LocationItemComponent>();
		if (item.Type == InteractType.Indoor && component2.IsIndoor)
			return new ValidateResult(false, "Type IsIndoor " + component2.IsIndoor);
		return item.Type == InteractType.Outdoor && !component2.IsIndoor
			? new ValidateResult(false, "Type IsIndoor " + component2.IsIndoor)
			: new ValidateResult(true);
	}
}