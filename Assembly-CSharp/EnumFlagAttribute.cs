using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class EnumFlagAttribute : PropertyAttribute
{
  public System.Type EnumType { get; set; }

  public EnumFlagAttribute(System.Type enumType) => this.EnumType = enumType;
}
