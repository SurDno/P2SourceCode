using System;

namespace Inspectors
{
  public class StoreToSelectionMethodDrawer : BaseMethodDrawer<StoreToSelectionMethodDrawer>
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
      for (int index = 0; index < 10; ++index)
      {
        int storeIndex = index;
        menu.AddItem("Store Object To/Slot " + (object) storeIndex, false, (Action) (() =>
        {
          Action<object> action = setter;
          if (action == null)
            return;
          action((object) new object[1]
          {
            (object) storeIndex
          });
        }));
      }
      menu.Show();
    }
  }
}
