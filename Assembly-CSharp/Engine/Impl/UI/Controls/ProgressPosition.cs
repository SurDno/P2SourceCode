using UnityEngine;

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
      get => this.minPosition;
      set
      {
        if (this.minPosition == value)
          return;
        this.minPosition = value;
        this.ApplyProgress();
      }
    }

    public Vector3 MaxPosition
    {
      get => this.maxPosition;
      set
      {
        if (this.maxPosition == value)
          return;
        this.maxPosition = value;
        this.ApplyProgress();
      }
    }

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      this.transform.localPosition = Vector3.Lerp(this.minPosition, this.maxPosition, this.Progress);
    }
  }
}
