using System.Collections.Generic;
using Cofe.Utility;

namespace StateSetters
{
  public static class StateSetterService
  {
    private static Dictionary<string, IStateSetterItemController> controllers = new Dictionary<string, IStateSetterItemController>();

    public static void Register(string id, IStateSetterItemController controller)
    {
      controllers.Add(id, controller);
    }

    public static void Apply(StateSetterItem item, float value)
    {
      string type = item.Type;
      if (type.IsNullOrEmpty())
        return;
      IStateSetterItemController setterItemController;
      if (controllers.TryGetValue(type, out setterItemController))
        setterItemController.Apply(item, value);
      else
        Debug.LogError((object) ("Controller not found : " + item.Type));
    }
  }
}
