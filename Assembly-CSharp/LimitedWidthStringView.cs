using Engine.Impl.UI.Controls;

public class LimitedWidthStringView : StringView
{
  [SerializeField]
  private StringView view;
  [SerializeField]
  private LayoutElement layout;
  [SerializeField]
  private Text text;
  [SerializeField]
  private float maxWidth;

  public override void SkipAnimation()
  {
  }

  protected override void ApplyStringValue()
  {
    if (!((Object) view != (Object) null))
      return;
    view.StringValue = StringValue;
    if ((Object) layout != (Object) null && (Object) text != (Object) null)
      layout.preferredWidth = (double) text.preferredWidth > maxWidth ? maxWidth : -1f;
  }
}
