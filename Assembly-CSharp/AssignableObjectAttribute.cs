using System;
using UnityEngine;

public class AssignableObjectAttribute(Type baseType) : PropertyAttribute 
  {
  public Type BaseType { get; } = baseType;
}
