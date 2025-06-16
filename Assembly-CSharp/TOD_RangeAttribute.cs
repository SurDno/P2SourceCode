using System;

[AttributeUsage(AttributeTargets.Field)]
public class TOD_RangeAttribute : PropertyAttribute
{
  public float min;
  public float max;

  public TOD_RangeAttribute(float min, float max)
  {
    this.min = min;
    this.max = max;
  }
}
