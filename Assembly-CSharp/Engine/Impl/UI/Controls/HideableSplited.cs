using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableSplited : HideableView
  {
    [SerializeField]
    private HideableView[] views = new HideableView[0];

    public override void SkipAnimation()
    {
      foreach (HideableView view in this.views)
      {
        if ((Object) view != (Object) null)
          view.SkipAnimation();
      }
    }

    protected override void ApplyVisibility()
    {
      foreach (HideableView view in this.views)
      {
        if ((Object) view != (Object) null)
          view.Visible = this.Visible;
      }
    }
  }
}
