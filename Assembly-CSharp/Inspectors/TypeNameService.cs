// Decompiled with JetBrains decompiler
// Type: Inspectors.TypeNameService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using System;
using System.Collections.Generic;

#nullable disable
namespace Inspectors
{
  public static class TypeNameService
  {
    private static Dictionary<Type, TypeNameService.Info> names = new Dictionary<Type, TypeNameService.Info>();

    static TypeNameService()
    {
      TypeNameService.RegisterTypeName(typeof (bool), "bool", "bool");
      TypeNameService.RegisterTypeName(typeof (byte), "byte", "byte");
      TypeNameService.RegisterTypeName(typeof (sbyte), "sbyte", "sbyte");
      TypeNameService.RegisterTypeName(typeof (char), "char", "char");
      TypeNameService.RegisterTypeName(typeof (Decimal), "decimal", "decimal");
      TypeNameService.RegisterTypeName(typeof (double), "double", "double");
      TypeNameService.RegisterTypeName(typeof (float), "float", "float");
      TypeNameService.RegisterTypeName(typeof (int), "int", "int");
      TypeNameService.RegisterTypeName(typeof (uint), "uint", "uint");
      TypeNameService.RegisterTypeName(typeof (long), "long", "long");
      TypeNameService.RegisterTypeName(typeof (ulong), "ulong", "ulong");
      TypeNameService.RegisterTypeName(typeof (object), "object", "object");
      TypeNameService.RegisterTypeName(typeof (short), "short", "short");
      TypeNameService.RegisterTypeName(typeof (ushort), "ushort", "ushort");
      TypeNameService.RegisterTypeName(typeof (string), "string", "string");
    }

    public static void RegisterTypeName(Type type, string name, string item)
    {
      TypeNameService.names.Add(type, new TypeNameService.Info()
      {
        Name = name != null ? name : type.Name,
        Item = item != null ? item : type.Name
      });
    }

    public static string GetTypeName(Type type, bool menuItemName = false)
    {
      type = ProxyFactory.GetType(type);
      TypeNameService.Info info;
      if (TypeNameService.names.TryGetValue(type, out info))
        return menuItemName ? info.Item : info.Name;
      string name = TypeNameService.GetName(type, menuItemName);
      TypeNameService.names.Add(type, new TypeNameService.Info()
      {
        Name = name,
        Item = name
      });
      return name;
    }

    private static string GetName(Type type, bool menuItemName)
    {
      string name = type.Name;
      if (type.IsGenericType)
      {
        int length = name.IndexOf('`');
        if (length != -1)
          name = name.Substring(0, length);
        Type[] genericArguments = type.GetGenericArguments();
        if (genericArguments.Length == 0)
        {
          name += "<>";
        }
        else
        {
          string str = name + "<" + TypeNameService.GetTypeName(genericArguments[0], menuItemName);
          for (int index = 1; index < genericArguments.Length; ++index)
            str = str + "," + TypeNameService.GetTypeName(genericArguments[index], menuItemName);
          name = str + ">";
        }
      }
      return name;
    }

    public struct Info
    {
      public string Name;
      public string Item;
    }
  }
}
