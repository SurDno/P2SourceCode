using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Field)]
  public class TooltipAttribute : Attribute
  {
    private readonly string mTooltip;

    public string Tooltip => mTooltip;

    public TooltipAttribute(string tooltip) => mTooltip = tooltip;
  }
}
