// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.fsTypeExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer
{
  public static class fsTypeExtensions
  {
    public static string CSharpName(this Type type) => type.CSharpName(false);

    public static string CSharpName(
      this Type type,
      bool includeNamespace,
      bool ensureSafeDeclarationName)
    {
      string str = type.CSharpName(includeNamespace);
      if (ensureSafeDeclarationName)
        str = str.Replace('>', '_').Replace('<', '_').Replace('.', '_');
      return str;
    }

    public static string CSharpName(this Type type, bool includeNamespace)
    {
      if (type == typeof (void))
        return "void";
      if (type == typeof (int))
        return "int";
      if (type == typeof (float))
        return "float";
      if (type == typeof (bool))
        return "bool";
      if (type == typeof (double))
        return "double";
      if (type == typeof (string))
        return "string";
      if (type.IsGenericParameter)
        return type.ToString();
      string str1 = "";
      IEnumerable<Type> source = (IEnumerable<Type>) type.GetGenericArguments();
      if (type.IsNested)
      {
        str1 = str1 + type.DeclaringType.CSharpName() + ".";
        if (type.DeclaringType.GetGenericArguments().Length != 0)
          source = source.Skip<Type>(type.DeclaringType.GetGenericArguments().Length);
      }
      string str2 = source.Any<Type>() ? str1 + type.Name.Substring(0, type.Name.IndexOf('`')) + "<" + string.Join(",", source.Select<Type, string>((Func<Type, string>) (t => t.CSharpName(includeNamespace))).ToArray<string>()) + ">" : str1 + type.Name;
      if (includeNamespace && type.Namespace != null)
        str2 = type.Namespace + "." + str2;
      return str2;
    }
  }
}
