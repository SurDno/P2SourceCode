using System.Linq;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Source.Commons;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public static class MarkDoorInteractValidator
  {
    [InteractValidator(InteractType.Mark)]
    [InteractValidator(InteractType.Unmark)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      LocationItemComponent component1 = player.GetComponent<LocationItemComponent>();
      if (InstanceByRequest<EngineApplication>.Instance.IsDebug)
        return new ValidateResult(true);
      if (component1.IsIndoor)
        return new ValidateResult(false, "IsIndoor " + component1.IsIndoor);
      IDoorComponent component2 = interactable.GetComponent<IDoorComponent>();
      if (component2 == null)
        return new ValidateResult(false, "IDoorComponent not found");
      if (component2.Marked.Value == (item.Type == InteractType.Mark))
        return new ValidateResult(false, "Marked " + component2.Marked.Value + " == Type " + item.Type);
      if (!component2.CanBeMarked.Value)
        return new ValidateResult(false, "CanBeMarked " + component2.CanBeMarked.Value);
      return !player.GetComponent<StorageComponent>().Items.Any(x => x.Groups.Contains(StorableGroup.Chalk)) ? new ValidateResult(false, "CanBeMarked not found") : new ValidateResult(true);
    }
  }
}
