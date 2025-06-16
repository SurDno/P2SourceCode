using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressCurve : ProgressView
  {
    [SerializeField]
    private FloatView view = (FloatView) null;
    [SerializeField]
    private AnimationCurve curve = new AnimationCurve();

    public override void SkipAnimation()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }

    protected override void ApplyProgress()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.FloatValue = this.curve.Evaluate(this.FloatValue);
    }
  }
}
