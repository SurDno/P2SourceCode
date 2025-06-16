using Cofe.Meta;
using System;
using System.Reflection;

namespace Inspectors
{
  [Initialisable]
  public static class MethodInspectedDrawers
  {
    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      InspectedDrawerService.AddConditional((Func<Type, bool>) (type => typeof (IMethodDrawer).IsAssignableFrom(type)), (InspectedDrawerService.DrawerHandle) ((name, type, value, mutable, context, drawer, target, member, setter) =>
      {
        FieldInfo field = type.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        if (!(field != (FieldInfo) null) || !(field.GetValue((object) null) is IMethodDrawer methodDrawer2))
          return;
        methodDrawer2.DrawInspected(name, type, value, mutable, context, drawer, setter);
      }));
    }
  }
}
