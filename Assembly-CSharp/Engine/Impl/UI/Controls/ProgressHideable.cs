namespace Engine.Impl.UI.Controls
{
  public class ProgressHideable : ProgressDecorator
  {
    [SerializeField]
    private HideableView hideableView = null;
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
      if (!((Object) hideableView != (Object) null))
        return;
      hideableView.SkipAnimation();
    }

    protected override void ApplyProgress()
    {
      base.ApplyProgress();
      if (!((Object) hideableView != (Object) null))
        return;
      hideableView.Visible = Progress < (double) hiddenRange.x || Progress > (double) hiddenRange.y;
    }
  }
}
