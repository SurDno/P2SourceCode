using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class SurgeryInteractValidator
  {
    [InteractValidator(InteractType.Surgery)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      ParametersComponent component = interactable.GetComponent<ParametersComponent>();
      if (component == null)
        return new ValidateResult(true, "ParametersComponent not found");
      IParameter<bool> byName1 = component.GetByName<bool>(ParameterNameEnum.Dead);
      if (byName1 != null && byName1.Value)
        return new ValidateResult(false, "Dead is true");
      IParameter<bool> byName2 = component.GetByName<bool>(ParameterNameEnum.CanAutopsy);
      return byName2 != null && !byName2.Value ? new ValidateResult(false, "CanAutopsy is false") : new ValidateResult(true);
    }
  }
}
