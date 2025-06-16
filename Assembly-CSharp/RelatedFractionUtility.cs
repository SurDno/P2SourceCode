using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;

public static class RelatedFractionUtility {
	public static List<FractionEnum> GetFraction(IEntity entity, FractionRelationEnum relation) {
		if (entity != null) {
			var component = entity.GetComponent<ParametersComponent>();
			if (component != null) {
				var byName = component.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
				if (byName != null) {
					var fractionSettings = GetFractionSettings(byName);
					if (fractionSettings != null && fractionSettings.Relations != null) {
						var fractionRelationGroup = GetFractionRelationGroup(relation, fractionSettings);
						if (fractionRelationGroup != null)
							return fractionRelationGroup.Fractions;
					}
				}
			}
		}

		return null;
	}

	private static FractionRelationGroup GetFractionRelationGroup(
		FractionRelationEnum relation,
		FractionSettings fractionSettings) {
		for (var index = 0; index < fractionSettings.Relations.Count; ++index) {
			var relation1 = fractionSettings.Relations[index];
			if (relation1.Relation == relation)
				return relation1;
		}

		return null;
	}

	private static FractionSettings GetFractionSettings(IParameter<FractionEnum> fraction) {
		for (var index = 0; index < ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions.Count; ++index) {
			var fraction1 = ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions[index];
			if (fraction1.Name == fraction.Value)
				return fraction1;
		}

		return null;
	}
}