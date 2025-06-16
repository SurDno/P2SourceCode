// Decompiled with JetBrains decompiler
// Type: EnumHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public static class EnumHelper
{
  public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
  {
    object[] customAttributes = enumVal.GetType().GetMember(enumVal.ToString())[0].GetCustomAttributes(typeof (T), false);
    return customAttributes.Length != 0 ? (T) customAttributes[0] : default (T);
  }
}
