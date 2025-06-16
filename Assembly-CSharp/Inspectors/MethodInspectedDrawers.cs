// Decompiled with JetBrains decompiler
// Type: Inspectors.MethodInspectedDrawers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using System;
using System.Reflection;

#nullable disable
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
