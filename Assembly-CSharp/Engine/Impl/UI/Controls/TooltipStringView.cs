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
      if (!((Object) tooltip != (Object) null))
        return;
      tooltip.Text = StringValue;
    }
  }
}
