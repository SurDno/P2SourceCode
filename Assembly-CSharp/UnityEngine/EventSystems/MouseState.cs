using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
  public class MouseState
  {
    private List<ButtonState> m_TrackedButtons = new List<ButtonState>();

    public bool AnyPressesThisFrame()
    {
      for (int index = 0; index < m_TrackedButtons.Count; ++index)
      {
        if (m_TrackedButtons[index].eventData.PressedThisFrame())
          return true;
      }
      return false;
    }

    public bool AnyReleasesThisFrame()
    {
      for (int index = 0; index < m_TrackedButtons.Count; ++index)
      {
        if (m_TrackedButtons[index].eventData.ReleasedThisFrame())
          return true;
      }
      return false;
    }

    public ButtonState GetButtonState(PointerEventData.InputButton button)
    {
      ButtonState buttonState = null;
      for (int index = 0; index < m_TrackedButtons.Count; ++index)
      {
        if (m_TrackedButtons[index].button == button)
        {
          buttonState = m_TrackedButtons[index];
          break;
        }
      }
      if (buttonState == null)
      {
        buttonState = new ButtonState {
          button = button,
          eventData = new MouseButtonEventData()
        };
        m_TrackedButtons.Add(buttonState);
      }
      return buttonState;
    }

    public void SetButtonState(
      PointerEventData.InputButton button,
      PointerEventData.FramePressState stateForMouseButton,
      PointerEventData data)
    {
      ButtonState buttonState = GetButtonState(button);
      buttonState.eventData.buttonState = stateForMouseButton;
      buttonState.eventData.buttonData = data;
    }
  }
}
