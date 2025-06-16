// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.KnockDoorInteractValidator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Interactable;
using Engine.Common.Services;

#nullable disable
namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public static class KnockDoorInteractValidator
  {
    [InteractValidator(InteractType.Knock)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      IDoorComponent component1 = interactable.GetComponent<IDoorComponent>();
      if (component1 == null)
        return new ValidateResult(false, "IDoorComponent not found");
      LocationItemComponent component2 = ServiceLocator.GetService<ISimulation>().Player.GetComponent<LocationItemComponent>();
      if (component1.Bolted.Value)
        return new ValidateResult(false, "Bolted " + component1.Bolted.Value.ToString());
      if (!component1.Knockable.Value)
        return new ValidateResult(false, "Knockable " + component1.Knockable.Value.ToString());
      if (component1.LockState.Value != LockState.Locked)
        return new ValidateResult(false, "LockState " + (object) component1.LockState.Value);
      if (component1.Opened.Value)
        return new ValidateResult(false, "Opened " + component1.Opened.Value.ToString());
      return component1.IsOutdoor && component2.IsIndoor ? new ValidateResult(false, "IsOutdoor " + component1.IsOutdoor.ToString() + " && IsIndoor " + component2.IsIndoor.ToString()) : new ValidateResult(true);
    }
  }
}
