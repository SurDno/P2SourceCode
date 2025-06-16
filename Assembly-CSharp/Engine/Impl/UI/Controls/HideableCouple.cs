using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableCouple : HideableView
  {
    [SerializeField]
    private HideableView positiveView = (HideableView) null;
    [SerializeField]
    private HideableView negativeView = (HideableView) null;

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      if ((Object) this.positiveView != (Object) null)
        this.positiveView.SkipAnimation();
      if (!((Object) this.negativeView != (Object) null))
        return;
      this.negativeView.SkipAnimation();
    }

    protected override void ApplyVisibility()
    {
      if ((Object) this.positiveView != (Object) null)
        this.positiveView.Visible = this.Visible;
      if (!((Object) this.negativeView != (Object) null))
        return;
      this.negativeView.Visible = !this.Visible;
    }
  }
}
