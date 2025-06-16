using System;

namespace Inspectors
{
  public abstract class BaseMethodDrawer<T> : IMethodDrawer where T : class, IMethodDrawer, new()
  {
    public static readonly T Instance = new T();

    public abstract void DrawInspected(
      string name,
      Type type,
      object value,
      bool mutable,
      IInspectedProvider context,
      IInspectedDrawer drawer,
      Action<object> setter);
  }
}
