namespace Engine.Impl.UI.Controls
{
  public class ParagraphSpaceStringView : StringView
  {
    [SerializeField]
    private StringView view;
    [SerializeField]
    private int size;

    public override void SkipAnimation()
    {
      if (!((Object) view != (Object) null))
        return;
      view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if ((Object) view == (Object) null)
        return;
      view.StringValue = StringValue?.Replace("\n", "\n<size=" + size + ">\n</size>");
    }
  }
}
