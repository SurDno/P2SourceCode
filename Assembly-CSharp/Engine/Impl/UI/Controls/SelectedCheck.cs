namespace Engine.Impl.UI.Controls
{
  public class SelectedCheck : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
  {
    [SerializeField]
    private HideableView hideableView;

    public void OnDeselect(BaseEventData eventData)
    {
      if (!((Object) hideableView != (Object) null))
        return;
      hideableView.Visible = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
      if (!((Object) hideableView != (Object) null))
        return;
      hideableView.Visible = true;
    }
  }
}
