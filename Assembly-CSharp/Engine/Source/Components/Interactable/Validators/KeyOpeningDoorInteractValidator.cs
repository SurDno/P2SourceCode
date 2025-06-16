using System.Linq;
using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Interactable;
using Engine.Common.Services;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public static class KeyOpeningDoorInteractValidator
  {
    [InteractValidator(InteractType.KeyOpening)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      IDoorComponent door = interactable.GetComponent<IDoorComponent>();
      if (door == null)
        return new ValidateResult(false, "IDoorComponent not found");
      if (door.LockState.Value != LockState.Locked)
        return new ValidateResult(false, "LockState " + door.LockState.Value);
      if (door.Keys.Count() == 0)
        return new ValidateResult(false, "Keys is empty");
      return !ServiceLocator.GetService<ISimulation>().Player.GetComponent<StorageComponent>().Items.Select(o => o.Owner.TemplateId).Any(o => door.Keys.Select(p => p.Id).Any(p => p == o)) ? new ValidateResult(false, "keys not found") : new ValidateResult(true);
    }
  }
}
