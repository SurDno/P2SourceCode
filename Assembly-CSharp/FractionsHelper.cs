using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using System.Collections.Generic;

public class FractionsHelper
{
  public static FractionEnum GetTargetFraction(IEntity target, IEntity owner)
  {
    if (target == null)
      return FractionEnum.None;
    ParametersComponent component1 = target.GetComponent<ParametersComponent>();
    if (component1 == null)
      return FractionEnum.None;
    IParameter<FractionEnum> byName = component1.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
    if (byName == null)
      return FractionEnum.None;
    if (byName.Value != FractionEnum.Player)
      return byName.Value;
    FractionSettings fractionSettings = FractionsHelper.GetFractionSettings(owner);
    if (fractionSettings == null || (double) fractionSettings.PlayerReputationThreshold == 0.0)
      return FractionEnum.Player;
    NavigationComponent component2 = owner.GetComponent<NavigationComponent>();
    return component2 == null || component2.Region == null || component2.Region.Reputation == null ? FractionEnum.Player : ((double) component2.Region.Reputation.Value < (double) fractionSettings.PlayerReputationThreshold ? FractionEnum.PlayerLowReputation : FractionEnum.Player);
  }

  private static FractionSettings GetFractionSettings(IEntity target)
  {
    if (target == null)
      return (FractionSettings) null;
    ParametersComponent component = target.GetComponent<ParametersComponent>();
    if (component == null)
      return (FractionSettings) null;
    IParameter<FractionEnum> byName = component.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
    if (byName == null)
      return (FractionSettings) null;
    List<FractionSettings> fractions = ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions;
    for (int index = 0; index < fractions.Count; ++index)
    {
      FractionSettings fractionSettings = fractions[index];
      if (fractionSettings.Name == byName.Value)
        return fractionSettings;
    }
    return (FractionSettings) null;
  }
}
