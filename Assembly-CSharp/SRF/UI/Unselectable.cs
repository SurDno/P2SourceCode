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
      if ((Object) EventSystem.current.currentSelectedGameObject == (Object) CachedGameObject)
        EventSystem.current.SetSelectedGameObject((GameObject) null);
      _suspectedSelected = false;
    }
  }
}
