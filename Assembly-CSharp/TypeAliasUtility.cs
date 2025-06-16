// Decompiled with JetBrains decompiler
// Type: TypeAliasUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
public static class TypeAliasUtility
{
  private static readonly Dictionary<Type, string> aliases = new Dictionary<Type, string>()
  {
    {
      typeof (byte),
      "byte"
    },
    {
      typeof (sbyte),
      "sbyte"
    },
    {
      typeof (short),
      "short"
    },
    {
      typeof (ushort),
      "ushort"
    },
    {
      typeof (int),
      "int"
    },
    {
      typeof (uint),
      "uint"
    },
    {
      typeof (long),
      "long"
    },
    {
      typeof (ulong),
      "ulong"
    },
    {
      typeof (float),
      "float"
    },
    {
      typeof (double),
      "double"
    },
    {
      typeof (Decimal),
      "decimal"
    },
    {
      typeof (object),
      "object"
    },
    {
      typeof (bool),
      "bool"
    },
    {
      typeof (char),
      "char"
    },
    {
      typeof (string),
      "string"
    },
    {
      typeof (void),
      "void"
    }
  };

  public static string GetName(this Type type)
  {
    string str;
    return TypeAliasUtility.aliases.TryGetValue(type, out str) ? str : type.Name;
  }
}
