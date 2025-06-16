using System;

[AttributeUsage(AttributeTargets.Field)]
public class EnumFlagAttribute : PropertyAttribute
{
  public Type EnumType { get; set; }

  public EnumFlagAttribute(Type enumType) => EnumType = enumType;
}
