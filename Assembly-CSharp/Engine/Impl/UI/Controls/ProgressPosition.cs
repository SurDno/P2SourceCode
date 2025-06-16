namespace Engine.Impl.UI.Controls
{
  public class ProgressPosition : ProgressView
  {
    [SerializeField]
    private Vector3 minPosition = Vector3.zero;
    [SerializeField]
    private Vector3 maxPosition = Vector3.zero;

    public Vector3 MinPosition
    {
      get => minPosition;
      set
      {
        if (minPosition == value)
          return;
        minPosition = value;
        ApplyProgress();
      }
    }

    public Vector3 MaxPosition
    {
      get => maxPosition;
      set
      {
        if (maxPosition == value)
          return;
        maxPosition = value;
        ApplyProgress();
      }
    }

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      this.transform.localPosition = Vector3.Lerp(minPosition, maxPosition, Progress);
    }
  }
}
