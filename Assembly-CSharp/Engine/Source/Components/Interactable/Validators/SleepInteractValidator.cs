using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
public static class SleepInteractValidator {
	[InteractValidator(InteractType.Sleep)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		if (interactable.GetComponent<ParametersComponent>() == null)
			return new ValidateResult(true, "ParametersComponent not found");
		var byName = ServiceLocator.GetService<ISimulation>().Player.GetComponent<ParametersComponent>()
			.GetByName<bool>(ParameterNameEnum.Sleep);
		return byName != null && byName.Value ? new ValidateResult(false, "Sleep is true") : new ValidateResult(true);
	}
}