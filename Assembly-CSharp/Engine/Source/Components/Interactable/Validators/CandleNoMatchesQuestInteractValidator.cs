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
  public class CandleNoMatchesQuestInteractValidator
  {
    [InteractValidator(InteractType.LightCandleNoMatches)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      return ServiceLocator.GetService<ISimulation>().Player.GetComponent<StorageComponent>().Items.Any<IStorableComponent>((Func<IStorableComponent, bool>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Fuel_Lamp))) ? new ValidateResult(false, "Fuel_Lamp found") : new ValidateResult(true);
    }
  }
}
