using System;

namespace Inspectors
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class SliderAttribute : Attribute
  {
    public float Min { get; set; } = 0.0f;

    public float Max { get; set; } = 1f;
  }
}
