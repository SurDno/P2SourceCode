using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using System;
using System.Linq;

namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class HealEarthNoBloodValidator
  {
    [InteractValidator(InteractType.HealEarthNoBlood)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      return ServiceLocator.GetService<ISimulation>().Player.GetComponent<StorageComponent>().Items.Any<IStorableComponent>((Func<IStorableComponent, bool>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Craft_Organ) && !x.Groups.Contains<StorableGroup>(StorableGroup.Autopsy_Chest) && !x.Groups.Contains<StorableGroup>(StorableGroup.Autopsy_Head) && !x.Groups.Contains<StorableGroup>(StorableGroup.Autopsy_Stomach))) ? new ValidateResult(false, "Mnogo usloviy soryan") : new ValidateResult(true);
    }
  }
}
