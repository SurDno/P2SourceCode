namespace Engine.Impl.UI.Controls
{
  public class SetHideableEventView : EventView
  {
    [SerializeField]
    private HideableView view;
    [SerializeField]
    private bool value;

    public override void Invoke()
    {
      if (!((Object) view != (Object) null))
        return;
      view.Visible = value;
    }
  }
}
