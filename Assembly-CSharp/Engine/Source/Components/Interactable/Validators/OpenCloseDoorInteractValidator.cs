// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.OpenCloseDoorInteractValidator
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
  public static class OpenCloseDoorInteractValidator
  {
    [InteractValidator(InteractType.OpenDoor)]
    [InteractValidator(InteractType.CloseDoor)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      IDoorComponent component = interactable.GetComponent<IDoorComponent>();
      if (component == null)
        return new ValidateResult(false, "IDoorComponent not found");
      if (component.Bolted.Value)
        return new ValidateResult(false, "Bolted " + component.Bolted.Value.ToString());
      if (component.LockState.Value != 0)
        return new ValidateResult(false, "LockState " + (object) component.LockState.Value);
      if (component.Opened.Value != (item.Type == InteractType.OpenDoor))
        return new ValidateResult(true);
      return new ValidateResult(false, "Opened " + component.Opened.Value.ToString() + " != Type " + (object) item.Type);
    }
  }
}
