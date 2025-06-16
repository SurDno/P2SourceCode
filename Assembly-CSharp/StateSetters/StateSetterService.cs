// Decompiled with JetBrains decompiler
// Type: StateSetters.StateSetterService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace StateSetters
{
  public static class StateSetterService
  {
    private static Dictionary<string, IStateSetterItemController> controllers = new Dictionary<string, IStateSetterItemController>();

    public static void Register(string id, IStateSetterItemController controller)
    {
      StateSetterService.controllers.Add(id, controller);
    }

    public static void Apply(StateSetterItem item, float value)
    {
      string type = item.Type;
      if (type.IsNullOrEmpty())
        return;
      IStateSetterItemController setterItemController;
      if (StateSetterService.controllers.TryGetValue(type, out setterItemController))
        setterItemController.Apply(item, value);
      else
        Debug.LogError((object) ("Controller not found : " + item.Type));
    }
  }
}
