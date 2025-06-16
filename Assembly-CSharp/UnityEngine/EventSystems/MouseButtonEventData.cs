namespace UnityEngine.EventSystems
{
  public class MouseButtonEventData
  {
    public PointerEventData.FramePressState buttonState;
    public PointerEventData buttonData;

    public bool PressedThisFrame()
    {
      return buttonState == PointerEventData.FramePressState.Pressed || buttonState == PointerEventData.FramePressState.PressedAndReleased;
    }

    public bool ReleasedThisFrame()
    {
      return buttonState == PointerEventData.FramePressState.Released || buttonState == PointerEventData.FramePressState.PressedAndReleased;
    }
  }
}
