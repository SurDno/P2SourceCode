using System.Linq;
using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Storable;
using Engine.Common.Services;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class HydrantNoBottlesInteractValidator
  {
    [InteractValidator(InteractType.HydrantNoBottles)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      return ServiceLocator.GetService<ISimulation>().Player.GetComponent<StorageComponent>().Items.Any(x => x.Groups.Contains(StorableGroup.Autopsy_Instrument_Bottle)) ? new ValidateResult(false, "Autopsy_Instrument_Bottle found") : new ValidateResult(true);
    }
  }
}
