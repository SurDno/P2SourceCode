namespace Engine.Impl.UI.Controls
{
  public class NullCheckEntityView : EntityViewBase
  {
    [SerializeField]
    private HideableView view;

    protected override void ApplyValue()
    {
      if (!((Object) view != (Object) null))
        return;
      view.Visible = Value == null;
    }

    public override void SkipAnimation()
    {
      if (!((Object) view != (Object) null))
        return;
      view.SkipAnimation();
    }
  }
}
