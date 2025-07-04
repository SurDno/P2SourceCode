﻿using System;
using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;

namespace Engine.Source.Components.Interactable
{
  public static class InteractValidationService
  {
    private static Dictionary<InteractType, Func<IInteractableComponent, InteractItem, ValidateResult>> validators = new();

    public static void AddValidator(
      InteractType type,
      Func<IInteractableComponent, InteractItem, ValidateResult> validator)
    {
      validators.Add(type, validator);
    }

    public static ValidateResult Validate(IInteractableComponent interactable, InteractItem item)
    {
      if (!interactable.Owner.IsEnabledInHierarchy)
        return new ValidateResult(false, "Entity disabled");
      if (!interactable.IsEnabled)
        return new ValidateResult(false, "Interactable disabled");
      return validators.TryGetValue(item.Type, out Func<IInteractableComponent, InteractItem, ValidateResult> func) ? func(interactable, item) : new ValidateResult(true, "Validator not found");
    }
  }
}
