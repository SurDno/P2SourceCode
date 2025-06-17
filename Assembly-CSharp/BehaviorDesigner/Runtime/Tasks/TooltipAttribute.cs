using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Field)]
  public class TooltipAttribute(string tooltip) : Attribute 
  {
    public string Tooltip => tooltip;
  }
}
