namespace Engine.Impl.UI.Controls
{
  public class SelectWithPointer : 
    MonoBehaviour,
    IPointerEnterHandler,
    IEventSystemHandler,
    IPointerExitHandler
  {
    public void OnPointerEnter(PointerEventData eventData)
    {
      EventSystem.current.SetSelectedGameObject(this.gameObject, (BaseEventData) eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      EventSystem current = EventSystem.current;
      if (!((Object) current.currentSelectedGameObject == (Object) this.gameObject))
        return;
      current.SetSelectedGameObject((GameObject) null, (BaseEventData) eventData);
    }
  }
}
