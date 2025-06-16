using System;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.ObjectDrawers
{
  [AttributeUsage(AttributeTargets.Field)]
  public class FloatSliderAttribute : ObjectDrawerAttribute
  {
    public float min;
    public float max;

    public FloatSliderAttribute(float min, float max)
    {
      this.min = min;
      this.max = max;
    }
  }
}
