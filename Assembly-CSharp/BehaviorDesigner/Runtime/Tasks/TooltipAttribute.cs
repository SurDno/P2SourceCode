using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class TooltipAttribute : Attribute
  {
    private readonly string mTooltip;

    public string Tooltip => this.mTooltip;

    public TooltipAttribute(string tooltip) => this.mTooltip = tooltip;
  }
}
