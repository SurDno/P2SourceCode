namespace Engine.Impl.UI.Controls
{
  public abstract class HideableDecorator : HideableView
  {
    [SerializeField]
    private HideableView nestedView = null;

    protected override void ApplyVisibility()
    {
      if (!((Object) nestedView != (Object) null))
        return;
      nestedView.Visible = Visible;
    }
  }
}
