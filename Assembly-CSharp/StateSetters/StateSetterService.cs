﻿using System.Collections.Generic;
using Cofe.Utility;
using UnityEngine;

namespace StateSetters
{
  public static class StateSetterService
  {
    private static Dictionary<string, IStateSetterItemController> controllers = new();

    public static void Register(string id, IStateSetterItemController controller)
    {
      controllers.Add(id, controller);
    }

    public static void Apply(StateSetterItem item, float value)
    {
      string type = item.Type;
      if (type.IsNullOrEmpty())
        return;
      if (controllers.TryGetValue(type, out IStateSetterItemController setterItemController))
        setterItemController.Apply(item, value);
      else
        Debug.LogError("Controller not found : " + item.Type);
    }
  }
}
