namespace Engine.Impl.UI.Controls
{
  public abstract class ProgressDecorator : ProgressView
  {
    [SerializeField]
    private ProgressViewBase progressView = null;

    protected override void ApplyProgress()
    {
      if (!((Object) progressView != (Object) null))
        return;
      progressView.Progress = Progress;
    }

    public override void SkipAnimation()
    {
      if (!((Object) progressView != (Object) null))
        return;
      progressView.SkipAnimation();
    }
  }
}
