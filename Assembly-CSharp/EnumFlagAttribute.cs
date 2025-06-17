using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class EnumFlagAttribute(Type enumType) : PropertyAttribute 
  {
  public Type EnumType { get; set; } = enumType;
}
