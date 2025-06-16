using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class HealEarthExaustedValidator
  {
    [InteractValidator(InteractType.HealEarthExausted)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      HerbRootsComponent component = interactable.Owner.GetComponent<HerbRootsComponent>();
      if (component == null)
        return new ValidateResult(false, "HerbRootsComponent not found");
      return !component.IsHerbsbudgetReached ? new ValidateResult(false, "IsHerbsbudgetReached is false") : new ValidateResult(true);
    }
  }
}
