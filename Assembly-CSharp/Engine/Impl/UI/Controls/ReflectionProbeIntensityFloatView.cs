using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ReflectionProbeIntensityFloatView : FloatViewBase
  {
    [SerializeField]
    private ReflectionProbe view;

    protected override void ApplyFloatValue()
    {
      if (!(view != null))
        return;
      view.intensity = FloatValue;
    }

    public override void SkipAnimation()
    {
    }
  }
}
