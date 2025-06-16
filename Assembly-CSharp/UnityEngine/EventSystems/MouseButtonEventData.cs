namespace UnityEngine.EventSystems
{
  public class MouseButtonEventData
  {
    public PointerEventData.FramePressState buttonState;
    public PointerEventData buttonData;

    public bool PressedThisFrame()
    {
      return this.buttonState == PointerEventData.FramePressState.Pressed || this.buttonState == PointerEventData.FramePressState.PressedAndReleased;
    }

    public bool ReleasedThisFrame()
    {
      return this.buttonState == PointerEventData.FramePressState.Released || this.buttonState == PointerEventData.FramePressState.PressedAndReleased;
    }
  }
}
