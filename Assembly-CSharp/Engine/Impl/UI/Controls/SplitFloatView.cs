namespace Engine.Impl.UI.Controls
{
  public class SplitFloatView : FloatViewBase
  {
    [SerializeField]
    private FloatView[] views = null;

    public override void SkipAnimation()
    {
      if (views == null)
        return;
      for (int index = 0; index < views.Length; ++index)
      {
        FloatView view = views[index];
        if ((Object) view != (Object) null)
          view.SkipAnimation();
      }
    }

    protected override void ApplyFloatValue()
    {
      if (views == null)
        return;
      for (int index = 0; index < views.Length; ++index)
      {
        FloatView view = views[index];
        if ((Object) view != (Object) null)
          view.FloatValue = FloatValue;
      }
    }
  }
}
