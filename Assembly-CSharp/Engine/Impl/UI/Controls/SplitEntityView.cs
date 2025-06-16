using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SplitEntityView : EntityViewBase
  {
    [SerializeField]
    private EntityView[] views;

    protected override void ApplyValue()
    {
      if (views == null)
        return;
      foreach (EntityView view in views)
      {
        if (view != null)
          view.Value = Value;
      }
    }

    public override void SkipAnimation()
    {
      if (views == null)
        return;
      foreach (EntityView view in views)
        view?.SkipAnimation();
    }
  }
}
