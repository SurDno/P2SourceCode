namespace Engine.Impl.UI.Controls
{
  public class PointerInsideCheck : 
    MonoBehaviour,
    IPointerEnterHandler,
    IEventSystemHandler,
    IPointerExitHandler
  {
    [SerializeField]
    private HideableView hideableView;
    [SerializeField]
    private EventView pointerEnterEventView;
    [SerializeField]
    private EventView pointerExitEventView;

    public void OnPointerEnter(PointerEventData eventData)
    {
      pointerEnterEventView?.Invoke();
      if (!((Object) hideableView != (Object) null))
        return;
      hideableView.Visible = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if ((Object) hideableView != (Object) null)
        hideableView.Visible = false;
      pointerExitEventView?.Invoke();
    }
  }
}
