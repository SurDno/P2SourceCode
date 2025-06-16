namespace Engine.Impl.UI.Controls
{
  public class EnvironmentProbeIntensityFloatView : FloatViewBase
  {
    [SerializeField]
    private EnvironmentProbe view = null;

    protected override void ApplyFloatValue()
    {
      if (!((Object) view != (Object) null))
        return;
      view.AmbientIntensity = FloatValue;
    }

    public override void SkipAnimation()
    {
    }
  }
}
