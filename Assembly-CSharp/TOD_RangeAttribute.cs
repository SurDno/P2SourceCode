using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class TOD_RangeAttribute(float min, float max) : PropertyAttribute 
  {
  public float min = min;
  public float max = max;
}
