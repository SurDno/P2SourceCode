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
      if (!((Object) this.tooltip != (Object) null))
        return;
      this.tooltip.Text = this.StringValue;
    }
  }
}
