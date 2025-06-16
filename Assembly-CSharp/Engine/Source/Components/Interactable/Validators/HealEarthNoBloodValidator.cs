using System.Linq;
using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Storable;
using Engine.Common.Services;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class HealEarthNoBloodValidator
  {
    [InteractValidator(InteractType.HealEarthNoBlood)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      return ServiceLocator.GetService<ISimulation>().Player.GetComponent<StorageComponent>().Items.Any(x => x.Groups.Contains(StorableGroup.Craft_Organ) && !x.Groups.Contains(StorableGroup.Autopsy_Chest) && !x.Groups.Contains(StorableGroup.Autopsy_Head) && !x.Groups.Contains(StorableGroup.Autopsy_Stomach)) ? new ValidateResult(false, "Mnogo usloviy soryan") : new ValidateResult(true);
    }
  }
}
