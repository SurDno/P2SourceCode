using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class CurveFloatView : FloatViewBase
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

    protected override void ApplyFloatValue()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.FloatValue = this.curve.Evaluate(this.FloatValue);
    }
  }
}
