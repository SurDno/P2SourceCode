using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Services;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public static class BlockDoorInteractValidator
  {
    [InteractValidator(InteractType.Block)]
    [InteractValidator(InteractType.Unblock)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      if (ServiceLocator.GetService<ISimulation>().Player.GetComponent<LocationItemComponent>().IsIndoor)
        return new ValidateResult(false, "IsIndoor");
      IDoorComponent component = interactable.GetComponent<IDoorComponent>();
      if (component == null)
        return new ValidateResult(false, "IDoorComponent not found");
      if (component.Bolted.Value != (item.Type == InteractType.Block))
        return new ValidateResult(true);
      return new ValidateResult(false, "Bolted " + component.Bolted.Value + " == " + item.Type);
    }
  }
}
