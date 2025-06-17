using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class TOD_MinAttribute(float min) : PropertyAttribute 
  {
  public float min = min;
}
