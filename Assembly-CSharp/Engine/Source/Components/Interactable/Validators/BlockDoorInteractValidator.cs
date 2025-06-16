// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.BlockDoorInteractValidator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Services;

#nullable disable
namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public static class BlockDoorInteractValidator
  {
    [InteractValidator(InteractType.Block)]
    [InteractValidator(InteractType.Unblock)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      if (ServiceLocator.GetService<ISimulation>().Player.GetComponent<LocationItemComponent>().IsIndoor)
        return new ValidateResult(false, "IsIndoor");
      IDoorComponent component = interactable.GetComponent<IDoorComponent>();
      if (component == null)
        return new ValidateResult(false, "IDoorComponent not found");
      if (component.Bolted.Value != (item.Type == InteractType.Block))
        return new ValidateResult(true);
      return new ValidateResult(false, "Bolted " + component.Bolted.Value.ToString() + " == " + (object) item.Type);
    }
  }
}
