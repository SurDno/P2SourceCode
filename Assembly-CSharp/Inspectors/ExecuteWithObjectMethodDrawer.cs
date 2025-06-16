using Engine.Common;
using Engine.Common.Services;
using Engine.Services;
using System;

namespace Inspectors
{
  public class ExecuteWithObjectMethodDrawer : BaseMethodDrawer<ExecuteWithObjectMethodDrawer>
  {
    public override void DrawInspected(
      string name,
      Type type,
      object value,
      bool mutable,
      IInspectedProvider context,
      IInspectedDrawer drawer,
      Action<object> setter)
    {
      if (!drawer.ButtonField(name))
        return;
      IContextMenu menu = drawer.CreateMenu();
      menu.AddItem("Execute With Object/Player", false, (Action) (() =>
      {
        Action<object> action = setter;
        if (action == null)
          return;
        action((object) new object[1]
        {
          (object) ServiceLocator.GetService<ISimulation>().Player
        });
      }));
      menu.AddSeparator("Execute With Object/");
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int index2 = index1;
        IObject item = ServiceLocator.GetService<SelectionService>().GetSelection(index2) as IObject;
        menu.AddItem("Execute With Object/From Slot " + (object) index2 + " : " + (item != null ? (object) item.Name : (object) "null"), false, (Action) (() =>
        {
          Action<object> action = setter;
          if (action == null)
            return;
          action((object) new object[1]{ (object) item });
        }));
      }
      menu.Show();
    }
  }
}
