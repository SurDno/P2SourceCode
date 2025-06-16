// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.CollectItemsInteractValidator
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
  public static class CollectItemsInteractValidator
  {
    [InteractValidator(InteractType.Collect)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      CollectControllerComponent component = interactable.GetComponent<CollectControllerComponent>();
      if (component == null)
        return new ValidateResult(false, "CollectControllerComponent not found");
      bool flag = component.ValidateCollect();
      return !flag ? new ValidateResult(false, "ValidateCollect " + flag.ToString()) : new ValidateResult(true);
    }
  }
}
