using System;

public static class EnumHelper
{
  public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
  {
    object[] customAttributes = enumVal.GetType().GetMember(enumVal.ToString())[0].GetCustomAttributes(typeof (T), false);
    return customAttributes.Length != 0 ? (T) customAttributes[0] : default (T);
  }
}
