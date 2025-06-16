namespace Engine.Impl.UI.Controls
{
  public class CurveFloatView : FloatViewBase
  {
    [SerializeField]
    private FloatView view = null;
    [SerializeField]
    private AnimationCurve curve = new AnimationCurve();

    public override void SkipAnimation()
    {
      if (!((Object) view != (Object) null))
        return;
      view.SkipAnimation();
    }

    protected override void ApplyFloatValue()
    {
      if (!((Object) view != (Object) null))
        return;
      view.FloatValue = curve.Evaluate(FloatValue);
    }
  }
}
