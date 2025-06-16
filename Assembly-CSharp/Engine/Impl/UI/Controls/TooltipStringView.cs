using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class TooltipStringView : StringView
  {
    [SerializeField]
    private TextTooltip tooltip;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyStringValue()
    {
      if (!(tooltip != null))
        return;
      tooltip.Text = StringValue;
    }
  }
}
