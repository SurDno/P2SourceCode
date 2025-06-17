using System;
using System.Collections.Generic;
using Cofe.Proxies;

namespace Inspectors
{
  public static class TypeNameService
  {
    private static Dictionary<Type, Info> names = new();

    static TypeNameService()
    {
      RegisterTypeName(typeof (bool), "bool", "bool");
      RegisterTypeName(typeof (byte), "byte", "byte");
      RegisterTypeName(typeof (sbyte), "sbyte", "sbyte");
      RegisterTypeName(typeof (char), "char", "char");
      RegisterTypeName(typeof (Decimal), "decimal", "decimal");
      RegisterTypeName(typeof (double), "double", "double");
      RegisterTypeName(typeof (float), "float", "float");
      RegisterTypeName(typeof (int), "int", "int");
      RegisterTypeName(typeof (uint), "uint", "uint");
      RegisterTypeName(typeof (long), "long", "long");
      RegisterTypeName(typeof (ulong), "ulong", "ulong");
      RegisterTypeName(typeof (object), "object", "object");
      RegisterTypeName(typeof (short), "short", "short");
      RegisterTypeName(typeof (ushort), "ushort", "ushort");
      RegisterTypeName(typeof (string), "string", "string");
    }

    public static void RegisterTypeName(Type type, string name, string item)
    {
      names.Add(type, new Info {
        Name = name != null ? name : type.Name,
        Item = item != null ? item : type.Name
      });
    }

    public static string GetTypeName(Type type, bool menuItemName = false)
    {
      type = ProxyFactory.GetType(type);
      if (names.TryGetValue(type, out Info info))
        return menuItemName ? info.Item : info.Name;
      string name = GetName(type, menuItemName);
      names.Add(type, new Info {
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
          string str = name + "<" + GetTypeName(genericArguments[0], menuItemName);
          for (int index = 1; index < genericArguments.Length; ++index)
            str = str + "," + GetTypeName(genericArguments[index], menuItemName);
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
