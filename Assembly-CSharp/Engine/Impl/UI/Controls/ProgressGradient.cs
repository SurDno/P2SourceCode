using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressGradient : ProgressView
  {
    [SerializeField]
    private Gradient endGradient;
    [SerializeField]
    private Gradient startGradient;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if ((Object) this.endGradient != (Object) null)
        this.endGradient.EndPosition = this.Progress;
      if (!((Object) this.startGradient != (Object) null))
        return;
      this.startGradient.StartPosition = this.Progress;
    }
  }
}
