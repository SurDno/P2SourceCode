// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Interactable.InteractValidationService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Components.Interactable
{
  public static class InteractValidationService
  {
    private static Dictionary<InteractType, Func<IInteractableComponent, InteractItem, ValidateResult>> validators = new Dictionary<InteractType, Func<IInteractableComponent, InteractItem, ValidateResult>>();

    public static void AddValidator(
      InteractType type,
      Func<IInteractableComponent, InteractItem, ValidateResult> validator)
    {
      InteractValidationService.validators.Add(type, validator);
    }

    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      if (!interactable.Owner.IsEnabledInHierarchy)
        return new ValidateResult(false, "Entity disabled");
      if (!interactable.IsEnabled)
        return new ValidateResult(false, "Interactable disabled");
      Func<IInteractableComponent, InteractItem, ValidateResult> func;
      return InteractValidationService.validators.TryGetValue(item.Type, out func) ? func(interactable, item) : new ValidateResult(true, "Validator not found");
    }
  }
}
