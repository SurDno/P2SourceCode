using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SplitEntityView : EntityViewBase
  {
    [SerializeField]
    private EntityView[] views;

    protected override void ApplyValue()
    {
      if (this.views == null)
        return;
      foreach (EntityView view in this.views)
      {
        if ((Object) view != (Object) null)
          view.Value = this.Value;
      }
    }

    public override void SkipAnimation()
    {
      if (this.views == null)
        return;
      foreach (EntityView view in this.views)
        view?.SkipAnimation();
    }
  }
}
