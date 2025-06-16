namespace Engine.Impl.UI.Controls
{
  public class HideablePulse : HideableView
  {
    [SerializeField]
    private ProgressViewBase view;
    [SerializeField]
    private float upTime = 0.5f;
    [SerializeField]
    private float downTime = 0.5f;
    private bool goesUp;

    private void Update()
    {
      if ((Object) view == (Object) null)
        return;
      float progress = view.Progress;
      float num;
      if (goesUp)
      {
        num = Mathf.MoveTowards(progress, 1f, upTime != 0.0 ? Time.deltaTime / upTime : 1f);
        if (num == 1.0)
          goesUp = false;
      }
      else
      {
        num = Mathf.MoveTowards(progress, 0.0f, downTime != 0.0 ? Time.deltaTime / downTime : 1f);
        if (num == 0.0 && Visible)
          goesUp = true;
      }
      view.Progress = num;
    }

    protected override void ApplyVisibility()
    {
      if (!Application.isPlaying)
        SkipAnimation();
      goesUp = goesUp && Visible;
    }

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      goesUp = Visible;
      if (!((Object) view != (Object) null))
        return;
      view.Progress = 0.0f;
    }
  }
}
