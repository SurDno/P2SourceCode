// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.Validators.CandleNoMatchesQuestInteractValidator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using System;
using System.Linq;

#nullable disable
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
