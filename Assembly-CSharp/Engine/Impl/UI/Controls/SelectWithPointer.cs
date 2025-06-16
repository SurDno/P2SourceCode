using UnityEngine;
using UnityEngine.EventSystems;

namespace Engine.Impl.UI.Controls;

public class SelectWithPointer :
	MonoBehaviour,
	IPointerEnterHandler,
	IEventSystemHandler,
	IPointerExitHandler {
	public void OnPointerEnter(PointerEventData eventData) {
		EventSystem.current.SetSelectedGameObject(gameObject, eventData);
	}

	public void OnPointerExit(PointerEventData eventData) {
		var current = EventSystem.current;
		if (!(current.currentSelectedGameObject == gameObject))
			return;
		current.SetSelectedGameObject(null, eventData);
	}
}