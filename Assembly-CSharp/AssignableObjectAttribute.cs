using System;

public class AssignableObjectAttribute : PropertyAttribute
{
  public Type BaseType { get; }

  public AssignableObjectAttribute(Type baseType) => BaseType = baseType;
}
