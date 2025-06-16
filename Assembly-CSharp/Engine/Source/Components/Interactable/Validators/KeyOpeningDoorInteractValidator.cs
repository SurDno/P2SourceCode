using Cofe.Meta;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Interactable;
using Engine.Common.Services;
using System;
using System.Linq;

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
        return new ValidateResult(false, "LockState " + (object) door.LockState.Value);
      if (door.Keys.Count<IEntity>() == 0)
        return new ValidateResult(false, "Keys is empty");
      return !ServiceLocator.GetService<ISimulation>().Player.GetComponent<StorageComponent>().Items.Select<IStorableComponent, Guid>((Func<IStorableComponent, Guid>) (o => o.Owner.TemplateId)).Any<Guid>((Func<Guid, bool>) (o => door.Keys.Select<IEntity, Guid>((Func<IEntity, Guid>) (p => p.Id)).Any<Guid>((Func<Guid, bool>) (p => p == o)))) ? new ValidateResult(false, "keys not found") : new ValidateResult(true);
    }
  }
}
