using UnityEngine;
using UnityEngine.EventSystems;

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
      this.pointerEnterEventView?.Invoke();
      if (!((Object) this.hideableView != (Object) null))
        return;
      this.hideableView.Visible = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if ((Object) this.hideableView != (Object) null)
        this.hideableView.Visible = false;
      this.pointerExitEventView?.Invoke();
    }
  }
}
