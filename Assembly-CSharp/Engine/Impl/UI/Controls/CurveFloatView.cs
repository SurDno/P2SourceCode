using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class CurveFloatView : FloatViewBase
  {
    [SerializeField]
    private FloatView view;
    [SerializeField]
    private AnimationCurve curve = new AnimationCurve();

    public override void SkipAnimation()
    {
      if (!(view != null))
        return;
      view.SkipAnimation();
    }

    protected override void ApplyFloatValue()
    {
      if (!(view != null))
        return;
      view.FloatValue = curve.Evaluate(FloatValue);
    }
  }
}
