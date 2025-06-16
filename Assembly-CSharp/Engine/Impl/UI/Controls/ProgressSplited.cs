namespace Engine.Impl.UI.Controls
{
  public class ProgressSplited : ProgressView
  {
    [SerializeField]
    private FloatView[] nestedViews = new FloatView[0];

    protected override void ApplyProgress()
    {
      foreach (FloatView nestedView in nestedViews)
      {
        if ((Object) nestedView != (Object) null)
          nestedView.FloatValue = Progress;
      }
    }

    public override void SkipAnimation()
    {
      foreach (FloatView nestedView in nestedViews)
      {
        if ((Object) nestedView != (Object) null)
          nestedView.SkipAnimation();
      }
    }
  }
}
