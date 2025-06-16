using UnityEngine;
using UnityEngine.EventSystems;

namespace Engine.Impl.UI.Controls
{
  public class SelectedCheck : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
  {
    [SerializeField]
    private HideableView hideableView;

    public void OnDeselect(BaseEventData eventData)
    {
      if (!((Object) this.hideableView != (Object) null))
        return;
      this.hideableView.Visible = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
      if (!((Object) this.hideableView != (Object) null))
        return;
      this.hideableView.Visible = true;
    }
  }
}
