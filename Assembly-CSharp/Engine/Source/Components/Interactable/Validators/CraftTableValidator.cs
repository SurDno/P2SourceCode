// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.CraftTableValidator
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
  public class CraftTableValidator
  {
    [InteractValidator(InteractType.CraftTable)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      ValidateResult validateResult = BrokenInteractValidator.Validate(interactable, item);
      return validateResult.Result ? new ValidateResult(false, validateResult.Reason) : new ValidateResult(true);
    }
  }
}
