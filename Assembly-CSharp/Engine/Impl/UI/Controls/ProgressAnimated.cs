namespace Engine.Impl.UI.Controls
{
  public class ProgressAnimated : ProgressView
  {
    [SerializeField]
    private ProgressViewBase progressView = null;
    [SerializeField]
    private HideableView increasingEffect = null;
    [SerializeField]
    private HideableView decreasingEffect = null;
    [SerializeField]
    private float smoothTime = 1f;
    [SerializeField]
    private float effectThreshold = 0.0f;
    private float velocity;

    private void Update()
    {
      if ((Object) progressView == (Object) null)
        return;
      float progress = progressView.Progress;
      if (progress == (double) Progress)
        return;
      float num = Mathf.MoveTowards(Mathf.SmoothDamp(progress, Progress, ref velocity, smoothTime), Progress, Time.deltaTime * (1f / 1000f));
      progressView.Progress = num;
      if ((Object) increasingEffect == (Object) decreasingEffect)
      {
        if (!((Object) increasingEffect != (Object) null))
          return;
        increasingEffect.Visible = Progress - (double) num > effectThreshold || num - (double) Progress > effectThreshold;
      }
      else
      {
        if ((Object) increasingEffect != (Object) null)
          increasingEffect.Visible = Progress - (double) num > effectThreshold;
        if ((Object) decreasingEffect != (Object) null)
          decreasingEffect.Visible = num - (double) Progress > effectThreshold;
      }
    }

    public override void SkipAnimation()
    {
      if ((Object) progressView != (Object) null)
        progressView.Progress = Progress;
      velocity = 0.0f;
      if ((Object) increasingEffect != (Object) null)
        increasingEffect.Visible = false;
      if (!((Object) decreasingEffect != (Object) null))
        return;
      decreasingEffect.Visible = false;
    }

    protected override void ApplyProgress()
    {
      if (Application.isPlaying)
        return;
      SkipAnimation();
    }
  }
}
