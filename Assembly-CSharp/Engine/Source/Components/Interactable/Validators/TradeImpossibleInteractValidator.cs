using Cofe.Meta;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;

namespace Engine.Source.Components.Interactable.Validators;

[Initialisable]
public class TradeImpossibleInteractValidator {
	[InteractValidator(InteractType.TradeImpossible)]
	public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item) {
		var component1 = interactable.GetComponent<ParametersComponent>();
		if (component1 == null)
			return new ValidateResult(true, "ParametersComponent not found");
		var byName1 = component1.GetByName<bool>(ParameterNameEnum.Dead);
		if (byName1 != null && byName1.Value)
			return new ValidateResult(false, "Dead is true");
		var byName2 = component1.GetByName<bool>(ParameterNameEnum.IsFighting);
		if (byName2 != null && byName2.Value)
			return new ValidateResult(false, "IsFighting is true");
		var byName3 = component1.GetByName<bool>(ParameterNameEnum.CanTrade);
		if (byName3 != null && !byName3.Value)
			return new ValidateResult(false, "CanTrade is false");
		var num = 0.0f;
		var fraction = component1.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
		if (fraction != null) {
			var fractionSettings =
				ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions.Find(x => x.Name == fraction.Value);
			if (fractionSettings != null)
				num = fractionSettings.PlayerTradeReputationThreshold;
		}

		var byName4 = component1.GetByName<bool>(ParameterNameEnum.ForceTrade);
		if (byName4 != null && byName4.Value)
			return new ValidateResult(false, "ForceTrade is true");
		var component2 = interactable.Owner.GetComponent<NavigationComponent>();
		if (component2 == null)
			return new ValidateResult(false, "NavigationComponent not found");
		var region = component2.Region;
		if (region == null)
			return new ValidateResult(false, "Region not found");
		if (region.Reputation.Value >= (double)num)
			return new ValidateResult(false, "Reputation " + region.Reputation.Value + " >= " + num);
		var byName5 = ServiceLocator.GetService<ISimulation>().Player.GetComponent<ParametersComponent>()
			.GetByName<bool>(ParameterNameEnum.CanTrade);
		return byName5 != null && !byName5.Value
			? new ValidateResult(false, "CanTrade is false")
			: new ValidateResult(true);
	}
}