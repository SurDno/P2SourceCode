namespace Engine.Impl.UI.Controls
{
  public class HideableButtonInteractable : HideableView
  {
    [SerializeField]
    private Button button;

    protected override void ApplyVisibility()
    {
      if (!((Object) button != (Object) null))
        return;
      button.interactable = Visible;
    }
  }
}
