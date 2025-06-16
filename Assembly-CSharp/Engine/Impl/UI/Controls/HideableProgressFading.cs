namespace Engine.Impl.UI.Controls
{
  public class HideableProgressFading : HideableView
  {
    [SerializeField]
    private ProgressViewBase progressView;
    [SerializeField]
    private float fadeInTime = 0.5f;
    [SerializeField]
    private float fadeOutTime = 0.5f;

    private void Update()
    {
      if ((Object) progressView == (Object) null)
        return;
      float progress = progressView.Progress;
      float target = Visible ? 1f : 0.0f;
      if (progress == (double) target)
        return;
      float num = progress >= (double) target ? fadeOutTime : fadeInTime;
      if (num > 0.0)
        progressView.Progress = Mathf.MoveTowards(progress, target, Time.deltaTime / num);
      else
        progressView.Progress = target;
    }

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      if ((Object) progressView == (Object) null)
        return;
      progressView.Progress = Visible ? 1f : 0.0f;
      progressView.SkipAnimation();
    }

    protected override void ApplyVisibility()
    {
      if (Application.isPlaying)
        return;
      SkipAnimation();
    }
  }
}
