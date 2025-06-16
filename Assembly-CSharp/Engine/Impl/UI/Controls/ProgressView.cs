using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class ProgressView : ProgressViewBase
  {
    [SerializeField]
    [Range(0.0f, 1f)]
    private float progress = 0.0f;

    public override float Progress
    {
      get => this.progress;
      set
      {
        if ((double) this.progress == (double) value)
          return;
        this.progress = value;
        this.ApplyProgress();
      }
    }

    protected virtual void OnValidate() => this.ApplyProgress();

    protected abstract void ApplyProgress();
  }
}
