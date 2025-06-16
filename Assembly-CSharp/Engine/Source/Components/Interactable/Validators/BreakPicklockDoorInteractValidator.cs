using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Interactable;
using Engine.Common.Services;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public static class BreakPicklockDoorInteractValidator
  {
    [InteractValidator(InteractType.BreakPicklock)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      IDoorComponent component1 = interactable.GetComponent<IDoorComponent>();
      if (component1 == null)
        return new ValidateResult(false, "IDoorComponent not found");
      if (!component1.Pickable.Value)
        return new ValidateResult(false, "Pickable " + component1.Pickable.Value);
      if (component1.LockState.Value != LockState.Locked)
        return new ValidateResult(false, "LockState " + component1.LockState.Value);
      LocationItemComponent component2 = ServiceLocator.GetService<ISimulation>().Player.GetComponent<LocationItemComponent>();
      return component1.IsOutdoor && component2.IsIndoor ? new ValidateResult(false, "IsOutdoor " + component1.IsOutdoor + " && IsIndoor " + component2.IsIndoor) : new ValidateResult(true);
    }
  }
}
