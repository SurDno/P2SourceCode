using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class LootInteractValidator
  {
    [InteractValidator(InteractType.Loot)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      ParametersComponent component = interactable.GetComponent<ParametersComponent>();
      if (component == null)
        return new ValidateResult(true, "ParametersComponent not found");
      if (interactable.GetComponent<INpcControllerComponent>() == null)
        return new ValidateResult(true, "INpcControllerComponent not found");
      IParameter<bool> byName1 = component.GetByName<bool>(ParameterNameEnum.Surrender);
      if (byName1 != null && byName1.Value)
        return new ValidateResult(true, "Surrender is true");
      IParameter<bool> byName2 = component.GetByName<bool>(ParameterNameEnum.Dead);
      return byName2 != null && byName2.Value ? new ValidateResult(true, "Dead is false") : new ValidateResult(false, "Mnogo usloviy soryan");
    }
  }
}
