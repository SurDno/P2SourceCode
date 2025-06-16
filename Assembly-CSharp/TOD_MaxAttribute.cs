using System;

[AttributeUsage(AttributeTargets.Field)]
public class TOD_MaxAttribute : PropertyAttribute
{
  public float max;

  public TOD_MaxAttribute(float max) => this.max = max;
}
