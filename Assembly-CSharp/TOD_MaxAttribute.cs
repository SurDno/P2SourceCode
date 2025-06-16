using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class TOD_MaxAttribute : PropertyAttribute
{
  public float max;

  public TOD_MaxAttribute(float max) => this.max = max;
}
