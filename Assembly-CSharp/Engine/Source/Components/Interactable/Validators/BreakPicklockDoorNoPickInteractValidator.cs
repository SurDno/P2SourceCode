using Cofe.Meta;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using System;
using System.Linq;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class BreakPicklockDoorNoPickInteractValidator
  {
    [InteractValidator(InteractType.BreakPicklockNoPick)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      IDoorComponent component1 = interactable.GetComponent<IDoorComponent>();
      if (component1 == null)
        return new ValidateResult(false, "IDoorComponent not found");
      if (!component1.Pickable.Value)
        return new ValidateResult(false, "Pickable " + component1.Pickable.Value.ToString());
      if (component1.LockState.Value != LockState.Locked)
        return new ValidateResult(false, "LockState " + (object) component1.LockState.Value);
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      LocationItemComponent component2 = player.GetComponent<LocationItemComponent>();
      if (component1.IsOutdoor && component2.IsIndoor)
        return new ValidateResult(false, "IsOutdoor " + component1.IsOutdoor.ToString() + " && IsIndoor " + component2.IsIndoor.ToString());
      return player.GetComponent<StorageComponent>().Items.Any<IStorableComponent>((Func<IStorableComponent, bool>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Lockpick_Door))) ? new ValidateResult(false, "Lockpick_Door found") : new ValidateResult(true);
    }
  }
}
