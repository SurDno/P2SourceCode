namespace Engine.Impl.UI.Controls
{
  public class ProgressCurve : ProgressView
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

    protected override void ApplyProgress()
    {
      if (!((Object) view != (Object) null))
        return;
      view.FloatValue = curve.Evaluate(FloatValue);
    }
  }
}
