using System;

namespace Inspectors
{
  public class DefaultMethodDrawer : BaseMethodDrawer<DefaultMethodDrawer>
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
      if (!drawer.ButtonField(name) || setter == null)
        return;
      setter(null);
    }
  }
}
