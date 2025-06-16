// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.IndoorOutdoorHouseInteractValidator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;

#nullable disable
namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public static class IndoorOutdoorHouseInteractValidator
  {
    [InteractValidator(InteractType.Indoor)]
    [InteractValidator(InteractType.Outdoor)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      IDoorComponent component1 = interactable.GetComponent<IDoorComponent>();
      if (component1 == null)
        return new ValidateResult(false, "IDoorComponent not found");
      if (component1.Bolted.Value)
        return new ValidateResult(false, "Bolted " + component1.Bolted.Value.ToString());
      if (item.Type == InteractType.Outdoor)
      {
        LockState lockState;
        if (component1.LockState.TryGetValue(PriorityParameterEnum.Quest, out lockState) && lockState != 0)
          return new ValidateResult(false, "LockState Quest " + (object) lockState);
      }
      else if (component1.LockState.Value != 0)
        return new ValidateResult(false, "LockState " + (object) component1.LockState.Value);
      LocationItemComponent component2 = ServiceLocator.GetService<ISimulation>().Player.GetComponent<LocationItemComponent>();
      if (item.Type == InteractType.Indoor && component2.IsIndoor)
        return new ValidateResult(false, "Type IsIndoor " + component2.IsIndoor.ToString());
      return item.Type == InteractType.Outdoor && !component2.IsIndoor ? new ValidateResult(false, "Type IsIndoor " + component2.IsIndoor.ToString()) : new ValidateResult(true);
    }
  }
}
