using UnityEngine;
using UnityEngine.EventSystems;

namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/Unselectable")]
  public sealed class Unselectable : SRMonoBehaviour, ISelectHandler, IEventSystemHandler
  {
    private bool _suspectedSelected;

    public void OnSelect(BaseEventData eventData) => this._suspectedSelected = true;

    private void Update()
    {
      if (!this._suspectedSelected)
        return;
      if ((Object) EventSystem.current.currentSelectedGameObject == (Object) this.CachedGameObject)
        EventSystem.current.SetSelectedGameObject((GameObject) null);
      this._suspectedSelected = false;
    }
  }
}
