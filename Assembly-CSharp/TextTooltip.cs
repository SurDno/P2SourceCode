using UnityEngine;
using UnityEngine.EventSystems;

public class TextTooltip :
	MonoBehaviour,
	IPointerEnterHandler,
	IEventSystemHandler,
	IPointerExitHandler {
	[SerializeField] private string text = string.Empty;
	private TextTooltipView view;

	public string Text {
		get => text;
		set => text = value;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		view = TextTooltipView.Current;
		view?.Show(eventData.position, text);
	}

	public void OnPointerExit(PointerEventData eventData) {
		view?.Hide();
		view = null;
	}
}