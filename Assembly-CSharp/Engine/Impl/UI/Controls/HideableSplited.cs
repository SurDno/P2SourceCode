using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableSplited : HideableView
  {
    [SerializeField]
    private HideableView[] views = [];

    public override void SkipAnimation()
    {
      foreach (HideableView view in views)
      {
        if (view != null)
          view.SkipAnimation();
      }
    }

    protected override void ApplyVisibility()
    {
      foreach (HideableView view in views)
      {
        if (view != null)
          view.Visible = Visible;
      }
    }
  }
}
