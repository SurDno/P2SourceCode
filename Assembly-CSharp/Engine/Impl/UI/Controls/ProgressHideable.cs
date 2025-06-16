using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressHideable : ProgressDecorator
  {
    [SerializeField]
    private HideableView hideableView = (HideableView) null;
    [SerializeField]
    private Vector2 hiddenRange = Vector2.zero;

    public Vector2 HiddenRange
    {
      get => this.hiddenRange;
      set
      {
        if (value == this.hiddenRange)
          return;
        this.hiddenRange = value;
        this.ApplyProgress();
      }
    }

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      if (!((Object) this.hideableView != (Object) null))
        return;
      this.hideableView.SkipAnimation();
    }

    protected override void ApplyProgress()
    {
      base.ApplyProgress();
      if (!((Object) this.hideableView != (Object) null))
        return;
      this.hideableView.Visible = (double) this.Progress < (double) this.hiddenRange.x || (double) this.Progress > (double) this.hiddenRange.y;
    }
  }
}
