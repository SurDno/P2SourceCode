using System;

namespace Inspectors
{
  public interface IMethodDrawer
  {
    void DrawInspected(
      string name,
      Type type,
      object value,
      bool mutable,
      IInspectedProvider context,
      IInspectedDrawer drawer,
      Action<object> setter);
  }
}
