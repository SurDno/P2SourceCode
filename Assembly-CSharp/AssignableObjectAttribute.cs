using UnityEngine;

public class AssignableObjectAttribute : PropertyAttribute
{
  public System.Type BaseType { get; }

  public AssignableObjectAttribute(System.Type baseType) => this.BaseType = baseType;
}
