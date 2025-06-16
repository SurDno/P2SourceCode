using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class AskForRepairInteractValidator
  {
    [InteractValidator(InteractType.AskForRepair)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      return TradeInteractValidator.Validate(interactable, item);
    }
  }
}
