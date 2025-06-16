// Decompiled with JetBrains decompiler
// Type: Inspectors.InspectedDrawerService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace Inspectors
{
  public static class InspectedDrawerService
  {
    private static Dictionary<System.Type, InspectedDrawerService.DrawerHandle> drawers = new Dictionary<System.Type, InspectedDrawerService.DrawerHandle>();
    private static List<KeyValuePair<Func<System.Type, bool>, InspectedDrawerService.DrawerHandle>> conditionalDrawer = new List<KeyValuePair<Func<System.Type, bool>, InspectedDrawerService.DrawerHandle>>();
    private static Dictionary<System.Type, System.Type> elementTypes = new Dictionary<System.Type, System.Type>();

    public static object CopyPasteObject { get; set; }

    public static void Add(System.Type type, InspectedDrawerService.DrawerHandle action)
    {
      if (!InspectedDrawerService.drawers.ContainsKey(type))
        InspectedDrawerService.drawers.Add(type, (InspectedDrawerService.DrawerHandle) ((name, type2, value, mutable, context, drawer, target, member, setter) => action(name, type2, value, mutable, context, drawer, target, member, setter)));
      else
        Debug.LogError((object) ("Drawer type already exist : " + (object) type));
    }

    public static void Add<T>(InspectedDrawerService.DrawerHandle action)
    {
      InspectedDrawerService.Add(typeof (T), action);
    }

    public static void AddConditional(
      Func<System.Type, bool> condition,
      InspectedDrawerService.DrawerHandle action)
    {
      InspectedDrawerService.conditionalDrawer.Add(new KeyValuePair<Func<System.Type, bool>, InspectedDrawerService.DrawerHandle>(condition, action));
    }

    public static System.Type GetElementType(System.Type type)
    {
      System.Type elementType;
      if (InspectedDrawerService.elementTypes.TryGetValue(type, out elementType))
        return elementType;
      if (type.HasElementType)
        return type.GetElementType();
      System.Type[] genericArguments = type.GetGenericArguments();
      if (genericArguments.Length == 1)
        return genericArguments[0];
      return genericArguments.Length == 2 && type.IsGenericType && (typeof (Dictionary<,>) == type.GetGenericTypeDefinition() || typeof (IDictionary<,>) == type.GetGenericTypeDefinition()) ? typeof (KeyValuePair<,>).MakeGenericType(genericArguments) : typeof (object);
    }

    public static InspectedDrawerService.DrawerHandle GetDrawer(System.Type type)
    {
      InspectedDrawerService.DrawerHandle drawer;
      InspectedDrawerService.drawers.TryGetValue(type, out drawer);
      if (drawer != null)
        return drawer;
      foreach (KeyValuePair<Func<System.Type, bool>, InspectedDrawerService.DrawerHandle> keyValuePair in InspectedDrawerService.conditionalDrawer)
      {
        if (keyValuePair.Key(type))
          return keyValuePair.Value;
      }
      return (InspectedDrawerService.DrawerHandle) null;
    }

    public static void AddElementType(System.Type container, System.Type element)
    {
      InspectedDrawerService.elementTypes.Add(container, element);
    }

    public delegate void DrawerHandle(
      string name,
      System.Type type,
      object value,
      bool mutable,
      IInspectedProvider context,
      IInspectedDrawer drawer,
      object target,
      MemberInfo member,
      Action<object> setter);
  }
}
