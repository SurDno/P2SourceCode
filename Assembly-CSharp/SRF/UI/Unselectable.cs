using UnityEngine;
using UnityEngine.EventSystems;

namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/Unselectable")]
  public sealed class Unselectable : SRMonoBehaviour, ISelectHandler, IEventSystemHandler
  {
    private bool _suspectedSelected;

    public void OnSelect(BaseEventData eventData) => _suspectedSelected = true;

    private void Update()
    {
      if (!_suspectedSelected)
        return;
      if (EventSystem.current.currentSelectedGameObject == CachedGameObject)
        EventSystem.current.SetSelectedGameObject(null);
      _suspectedSelected = false;
    }
  }
}
