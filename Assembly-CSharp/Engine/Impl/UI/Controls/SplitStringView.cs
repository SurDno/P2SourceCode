namespace Engine.Impl.UI.Controls
{
  public class SplitStringView : StringView
  {
    [SerializeField]
    private StringView[] views;

    public override void SkipAnimation()
    {
      if (views == null)
        return;
      for (int index = 0; index < views.Length; ++index)
      {
        StringView view = views[index];
        if ((Object) view != (Object) null)
          view.SkipAnimation();
      }
    }

    protected override void ApplyStringValue()
    {
      if (views == null)
        return;
      for (int index = 0; index < views.Length; ++index)
      {
        StringView view = views[index];
        if ((Object) view != (Object) null)
          view.StringValue = StringValue;
      }
    }
  }
}
