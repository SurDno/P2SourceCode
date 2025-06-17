using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class TOD_MaxAttribute(float max) : PropertyAttribute 
  {
  public float max = max;
}
