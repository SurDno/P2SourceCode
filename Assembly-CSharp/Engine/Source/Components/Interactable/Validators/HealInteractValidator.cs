using Cofe.Meta;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
internal class HealInteractValidator {
	[InteractValidator(InteractType.Heal)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		var component = interactable.GetComponent<ParametersComponent>();
		if (component == null)
			return new ValidateResult(false, "ParametersComponent not found");
		var byName1 = component.GetByName<bool>(ParameterNameEnum.Dead);
		if (byName1 != null && byName1.Value)
			return new ValidateResult(false, "Dead is true");
		var byName2 = component.GetByName<bool>(ParameterNameEnum.CanHeal);
		if (byName2 != null && !byName2.Value)
			return new ValidateResult(false, "CanHeal is false");
		var byName3 = component.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState);
		return byName3.Value == BoundHealthStateEnum.Diseased || byName3.Value == BoundHealthStateEnum.TutorialPain ||
		       byName3.Value == BoundHealthStateEnum.TutorialDiagnostics
			? new ValidateResult(true)
			: new ValidateResult(false, "Mnogo usloviy soryan");
	}
}