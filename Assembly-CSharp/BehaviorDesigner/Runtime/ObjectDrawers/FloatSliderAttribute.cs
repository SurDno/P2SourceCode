using System;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.ObjectDrawers
{
  [AttributeUsage(AttributeTargets.Field)]
  public class FloatSliderAttribute(float min, float max) : ObjectDrawerAttribute 
  {
    public float min = min;
    public float max = max;
  }
}
