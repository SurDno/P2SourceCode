using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class RepairInteractValidator
  {
    [InteractValidator(InteractType.Repair)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      ParametersComponent component = interactable.GetComponent<ParametersComponent>();
      if (component == null)
        return new ValidateResult(false, "ParametersComponent not found");
      IParameter<float> byName = component.GetByName<float>(ParameterNameEnum.Durability);
      if (byName == null || byName.Value >= (double) byName.MaxValue)
        return new ValidateResult(false, "Durability >= max");
      return interactable.GetComponent<RepairableComponent>() == null ? new ValidateResult(false, "RepairableComponent not found") : new ValidateResult(true);
    }
  }
}
