using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;

public class FractionsHelper {
	public static FractionEnum GetTargetFraction(IEntity target, IEntity owner) {
		if (target == null)
			return FractionEnum.None;
		var component1 = target.GetComponent<ParametersComponent>();
		if (component1 == null)
			return FractionEnum.None;
		var byName = component1.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
		if (byName == null)
			return FractionEnum.None;
		if (byName.Value != FractionEnum.Player)
			return byName.Value;
		var fractionSettings = GetFractionSettings(owner);
		if (fractionSettings == null || fractionSettings.PlayerReputationThreshold == 0.0)
			return FractionEnum.Player;
		var component2 = owner.GetComponent<NavigationComponent>();
		return component2 == null || component2.Region == null || component2.Region.Reputation == null
			?
			FractionEnum.Player
			: component2.Region.Reputation.Value < (double)fractionSettings.PlayerReputationThreshold
				? FractionEnum.PlayerLowReputation
				: FractionEnum.Player;
	}

	private static FractionSettings GetFractionSettings(IEntity target) {
		if (target == null)
			return null;
		var component = target.GetComponent<ParametersComponent>();
		if (component == null)
			return null;
		var byName = component.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
		if (byName == null)
			return null;
		var fractions = ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions;
		for (var index = 0; index < fractions.Count; ++index) {
			var fractionSettings = fractions[index];
			if (fractionSettings.Name == byName.Value)
				return fractionSettings;
		}

		return null;
	}
}