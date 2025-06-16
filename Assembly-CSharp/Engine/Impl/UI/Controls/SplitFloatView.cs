using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SplitFloatView : FloatViewBase
  {
    [SerializeField]
    private FloatView[] views = (FloatView[]) null;

    public override void SkipAnimation()
    {
      if (this.views == null)
        return;
      for (int index = 0; index < this.views.Length; ++index)
      {
        FloatView view = this.views[index];
        if ((Object) view != (Object) null)
          view.SkipAnimation();
      }
    }

    protected override void ApplyFloatValue()
    {
      if (this.views == null)
        return;
      for (int index = 0; index < this.views.Length; ++index)
      {
        FloatView view = this.views[index];
        if ((Object) view != (Object) null)
          view.FloatValue = this.FloatValue;
      }
    }
  }
}
