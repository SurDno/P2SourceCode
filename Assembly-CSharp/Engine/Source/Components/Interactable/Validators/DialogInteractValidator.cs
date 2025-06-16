using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Services;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public static class DialogInteractValidator
  {
    [InteractValidator(InteractType.Dialog)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      ParametersComponent component1 = interactable.GetComponent<ParametersComponent>();
      if (component1 == null)
        return new ValidateResult(false, "ParametersComponent not found");
      ISpeakingComponent component2 = interactable.GetComponent<ISpeakingComponent>();
      if (component2 == null)
        return new ValidateResult(false, "ISpeakingComponent not found");
      if (!component2.IsEnabled)
        return new ValidateResult(false, "ISpeakingComponent IsEnabled " + component2.IsEnabled.ToString());
      IParameter<bool> byName1 = component1.GetByName<bool>(ParameterNameEnum.IsFighting);
      if (byName1 != null && byName1.Value)
        return new ValidateResult(false, "IsFighting is true");
      IParameter<bool> byName2 = component1.GetByName<bool>(ParameterNameEnum.Dead);
      if (byName2 != null && byName2.Value)
        return new ValidateResult(false, "Dead is true");
      if (!component2.SpeakAvailable)
        return new ValidateResult(false, "SpeakAvailable " + component2.SpeakAvailable.ToString());
      ILocationItemComponent component3 = ServiceLocator.GetService<ISimulation>().Player.GetComponent<ILocationItemComponent>();
      ILocationItemComponent component4 = interactable.GetComponent<ILocationItemComponent>();
      if (component4 == null)
        return new ValidateResult(false, "ILocationItemComponent not found");
      return !LocationItemUtility.CheckLocation(component4, component3) ? new ValidateResult(false, "Different locations") : new ValidateResult(true);
    }
  }
}
