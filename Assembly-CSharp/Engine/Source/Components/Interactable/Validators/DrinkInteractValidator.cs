// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.DrinkInteractValidator
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
  public class DrinkInteractValidator
  {
    [InteractValidator(InteractType.Drink)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      ValidateResult validateResult1 = BrokenInteractValidator.Validate(interactable, item);
      if (validateResult1.Result)
        return new ValidateResult(false, validateResult1.Reason);
      ValidateResult validateResult2 = EmptyInteractValidator.Validate(interactable, item);
      return validateResult2.Result ? new ValidateResult(false, validateResult2.Reason) : new ValidateResult(true, "Not broken and not empty");
    }
  }
}
