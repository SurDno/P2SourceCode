using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class EnvironmentProbeIntensityFloatView : FloatViewBase
  {
    [SerializeField]
    private EnvironmentProbe view = (EnvironmentProbe) null;

    protected override void ApplyFloatValue()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.AmbientIntensity = this.FloatValue;
    }

    public override void SkipAnimation()
    {
    }
  }
}
