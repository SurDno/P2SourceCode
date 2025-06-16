// Decompiled with JetBrains decompiler
// Type: SRF.Helpers.SRReflection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;

#nullable disable
namespace SRF.Helpers
{
  public static class SRReflection
  {
    public static void SetPropertyValue(object obj, PropertyInfo p, object value)
    {
      p.GetSetMethod().Invoke(obj, new object[1]{ value });
    }

    public static object GetPropertyValue(object obj, PropertyInfo p)
    {
      return p.GetGetMethod().Invoke(obj, (object[]) null);
    }

    public static T GetAttribute<T>(MemberInfo t) where T : Attribute
    {
      return Attribute.GetCustomAttribute(t, typeof (T)) as T;
    }
  }
}
