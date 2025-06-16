using Cofe.Meta;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Services;
using System;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class TradeInteractValidator
  {
    [InteractValidator(InteractType.Trade)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      ParametersComponent component1 = interactable.GetComponent<ParametersComponent>();
      if (component1 == null)
        return new ValidateResult(true, "ParametersComponent not found");
      IParameter<bool> byName1 = component1.GetByName<bool>(ParameterNameEnum.Dead);
      if (byName1 != null && byName1.Value)
        return new ValidateResult(false, "Dead is true");
      IParameter<bool> byName2 = component1.GetByName<bool>(ParameterNameEnum.IsFighting);
      if (byName2 != null && byName2.Value)
        return new ValidateResult(false, "IsFighting is true");
      IParameter<bool> byName3 = component1.GetByName<bool>(ParameterNameEnum.CanTrade);
      if (byName3 != null && !byName3.Value)
        return new ValidateResult(false, "CanTrade is false");
      float num = 0.0f;
      IParameter<FractionEnum> fraction = component1.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
      if (fraction != null)
      {
        FractionSettings fractionSettings = ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions.Find((Predicate<FractionSettings>) (x => x.Name == fraction.Value));
        if (fractionSettings != null)
          num = fractionSettings.PlayerTradeReputationThreshold;
      }
      IParameter<bool> byName4 = component1.GetByName<bool>(ParameterNameEnum.ForceTrade);
      if (byName4 == null || !byName4.Value)
      {
        NavigationComponent component2 = interactable.Owner.GetComponent<NavigationComponent>();
        if (component2 != null)
        {
          IRegionComponent region = component2.Region;
          if (region != null && (double) region.Reputation.Value < (double) num)
            return new ValidateResult(false, "Reputation " + (object) region.Reputation.Value + " " + (object) num);
        }
      }
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      ParametersComponent component3 = player.GetComponent<ParametersComponent>();
      if (component3 == null)
        return new ValidateResult(true, "ParametersComponent not found");
      IParameter<bool> byName5 = component3.GetByName<bool>(ParameterNameEnum.CanTrade);
      if (byName5 != null && !byName5.Value)
        return new ValidateResult(false, "CanTrade is false");
      ILocationItemComponent component4 = player.GetComponent<ILocationItemComponent>();
      ILocationItemComponent component5 = interactable.GetComponent<ILocationItemComponent>();
      if (component5 == null)
        return new ValidateResult(false, "ILocationItemComponent not found");
      return !LocationItemUtility.CheckLocation(component5, component4) ? new ValidateResult(false, "Different locations") : new ValidateResult(true);
    }
  }
}
