using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class ProgressDecorator : ProgressView
  {
    [SerializeField]
    private ProgressViewBase progressView = (ProgressViewBase) null;

    protected override void ApplyProgress()
    {
      if (!((Object) this.progressView != (Object) null))
        return;
      this.progressView.Progress = this.Progress;
    }

    public override void SkipAnimation()
    {
      if (!((Object) this.progressView != (Object) null))
        return;
      this.progressView.SkipAnimation();
    }
  }
}
