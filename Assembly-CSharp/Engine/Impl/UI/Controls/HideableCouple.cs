using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableCouple : HideableView
  {
    [SerializeField]
    private HideableView positiveView;
    [SerializeField]
    private HideableView negativeView;

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      if (positiveView != null)
        positiveView.SkipAnimation();
      if (!(negativeView != null))
        return;
      negativeView.SkipAnimation();
    }

    protected override void ApplyVisibility()
    {
      if (positiveView != null)
        positiveView.Visible = Visible;
      if (!(negativeView != null))
        return;
      negativeView.Visible = !Visible;
    }
  }
}
