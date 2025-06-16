namespace Engine.Impl.UI.Controls
{
  public class ReflectionProbeIntensityFloatView : FloatViewBase
  {
    [SerializeField]
    private ReflectionProbe view = (ReflectionProbe) null;

    protected override void ApplyFloatValue()
    {
      if (!((Object) view != (Object) null))
        return;
      view.intensity = FloatValue;
    }

    public override void SkipAnimation()
    {
    }
  }
}
