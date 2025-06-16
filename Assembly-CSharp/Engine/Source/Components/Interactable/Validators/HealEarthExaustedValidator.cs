// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.HealEarthExaustedValidator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;

#nullable disable
namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public class HealEarthExaustedValidator
  {
    [InteractValidator(InteractType.HealEarthExausted)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      HerbRootsComponent component = interactable.Owner.GetComponent<HerbRootsComponent>();
      if (component == null)
        return new ValidateResult(false, "HerbRootsComponent not found");
      return !component.IsHerbsbudgetReached ? new ValidateResult(false, "IsHerbsbudgetReached is false") : new ValidateResult(true);
    }
  }
}
