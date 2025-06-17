using System;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.ObjectDrawers
{
  [AttributeUsage(AttributeTargets.Field)]
  public class IntSliderAttribute(int min, int max) : ObjectDrawerAttribute 
  {
    public int min = min;
    public int max = max;
  }
}
