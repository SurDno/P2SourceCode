namespace UnityEngine.EventSystems
{
  public class ButtonState
  {
    private PointerEventData.InputButton m_Button = PointerEventData.InputButton.Left;
    private MouseButtonEventData m_EventData;

    public MouseButtonEventData eventData
    {
      get => this.m_EventData;
      set => this.m_EventData = value;
    }

    public PointerEventData.InputButton button
    {
      get => this.m_Button;
      set => this.m_Button = value;
    }
  }
}
