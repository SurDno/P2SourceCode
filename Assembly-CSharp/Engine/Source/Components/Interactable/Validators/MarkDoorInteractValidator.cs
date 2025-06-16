// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.MarkDoorInteractValidator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Source.Commons;
using System;
using System.Linq;

#nullable disable
namespace Engine.Source.Components.Interactable.Validators
{
  [Initialisable]
  public static class MarkDoorInteractValidator
  {
    [InteractValidator(InteractType.Mark)]
    [InteractValidator(InteractType.Unmark)]
    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      LocationItemComponent component1 = player.GetComponent<LocationItemComponent>();
      if (InstanceByRequest<EngineApplication>.Instance.IsDebug)
        return new ValidateResult(true);
      if (component1.IsIndoor)
        return new ValidateResult(false, "IsIndoor " + component1.IsIndoor.ToString());
      IDoorComponent component2 = interactable.GetComponent<IDoorComponent>();
      if (component2 == null)
        return new ValidateResult(false, "IDoorComponent not found");
      if (component2.Marked.Value == (item.Type == InteractType.Mark))
        return new ValidateResult(false, "Marked " + component2.Marked.Value.ToString() + " == Type " + (object) item.Type);
      if (!component2.CanBeMarked.Value)
        return new ValidateResult(false, "CanBeMarked " + component2.CanBeMarked.Value.ToString());
      return !player.GetComponent<StorageComponent>().Items.Any<IStorableComponent>((Func<IStorableComponent, bool>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Chalk))) ? new ValidateResult(false, "CanBeMarked not found") : new ValidateResult(true);
    }
  }
}
