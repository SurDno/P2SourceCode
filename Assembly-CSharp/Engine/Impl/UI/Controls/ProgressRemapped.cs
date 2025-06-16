namespace Engine.Impl.UI.Controls
{
  public class ProgressRemapped : ProgressView
  {
    [SerializeField]
    private ProgressViewBase nestedView;
    [SerializeField]
    private Vector2 targetRange = new Vector2(0.0f, 1f);

    protected override void ApplyProgress()
    {
      if (!((Object) nestedView != (Object) null))
        return;
      nestedView.Progress = Mathf.Lerp(targetRange.x, targetRange.y, Progress);
    }

    public void SetMin(float min)
    {
      targetRange.x = min;
      ApplyProgress();
    }

    public void SetMax(float max)
    {
      targetRange.y = max;
      ApplyProgress();
    }

    public override void SkipAnimation()
    {
      if (!((Object) nestedView != (Object) null))
        return;
      nestedView.SkipAnimation();
    }
  }
}
