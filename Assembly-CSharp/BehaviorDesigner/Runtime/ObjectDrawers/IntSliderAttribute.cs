using System;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.ObjectDrawers
{
  [AttributeUsage(AttributeTargets.Field)]
  public class IntSliderAttribute : ObjectDrawerAttribute
  {
    public int min;
    public int max;

    public IntSliderAttribute(int min, int max)
    {
      this.min = min;
      this.max = max;
    }
  }
}
