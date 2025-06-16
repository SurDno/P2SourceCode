namespace Engine.Impl.UI.Controls
{
  public class HideableCouple : HideableView
  {
    [SerializeField]
    private HideableView positiveView = null;
    [SerializeField]
    private HideableView negativeView = null;

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      if ((Object) positiveView != (Object) null)
        positiveView.SkipAnimation();
      if (!((Object) negativeView != (Object) null))
        return;
      negativeView.SkipAnimation();
    }

    protected override void ApplyVisibility()
    {
      if ((Object) positiveView != (Object) null)
        positiveView.Visible = Visible;
      if (!((Object) negativeView != (Object) null))
        return;
      negativeView.Visible = !Visible;
    }
  }
}
