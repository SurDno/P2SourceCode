using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ReflectionProbeIntensityFloatView : FloatViewBase
  {
    [SerializeField]
    private ReflectionProbe view = (ReflectionProbe) null;

    protected override void ApplyFloatValue()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.intensity = this.FloatValue;
    }

    public override void SkipAnimation()
    {
    }
  }
}
