namespace UnityEngine.EventSystems;

public class ButtonState {
	private PointerEventData.InputButton m_Button = PointerEventData.InputButton.Left;
	private MouseButtonEventData m_EventData;

	public MouseButtonEventData eventData {
		get => m_EventData;
		set => m_EventData = value;
	}

	public PointerEventData.InputButton button {
		get => m_Button;
		set => m_Button = value;
	}
}