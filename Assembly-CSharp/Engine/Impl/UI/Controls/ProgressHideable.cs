using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressHideable : ProgressDecorator
  {
    [SerializeField]
    private HideableView hideableView;
    [SerializeField]
    private Vector2 hiddenRange = Vector2.zero;

    public Vector2 HiddenRange
    {
      get => hiddenRange;
      set
      {
        if (value == hiddenRange)
          return;
        hiddenRange = value;
        ApplyProgress();
      }
    }

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      if (!(hideableView != null))
        return;
      hideableView.SkipAnimation();
    }

    protected override void ApplyProgress()
    {
      base.ApplyProgress();
      if (!(hideableView != null))
        return;
      hideableView.Visible = Progress < (double) hiddenRange.x || Progress > (double) hiddenRange.y;
    }
  }
}
