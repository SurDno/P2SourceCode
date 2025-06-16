using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SplitFloatView : FloatViewBase
  {
    [SerializeField]
    private FloatView[] views;

    public override void SkipAnimation()
    {
      if (views == null)
        return;
      for (int index = 0; index < views.Length; ++index)
      {
        FloatView view = views[index];
        if (view != null)
          view.SkipAnimation();
      }
    }

    protected override void ApplyFloatValue()
    {
      if (views == null)
        return;
      for (int index = 0; index < views.Length; ++index)
      {
        FloatView view = views[index];
        if (view != null)
          view.FloatValue = FloatValue;
      }
    }
  }
}
