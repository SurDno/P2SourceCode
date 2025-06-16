// Decompiled with JetBrains decompiler
// Type: UnityEngine.EventSystems.MouseState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace UnityEngine.EventSystems
{
  public class MouseState
  {
    private List<ButtonState> m_TrackedButtons = new List<ButtonState>();

    public bool AnyPressesThisFrame()
    {
      for (int index = 0; index < this.m_TrackedButtons.Count; ++index)
      {
        if (this.m_TrackedButtons[index].eventData.PressedThisFrame())
          return true;
      }
      return false;
    }

    public bool AnyReleasesThisFrame()
    {
      for (int index = 0; index < this.m_TrackedButtons.Count; ++index)
      {
        if (this.m_TrackedButtons[index].eventData.ReleasedThisFrame())
          return true;
      }
      return false;
    }

    public ButtonState GetButtonState(PointerEventData.InputButton button)
    {
      ButtonState buttonState = (ButtonState) null;
      for (int index = 0; index < this.m_TrackedButtons.Count; ++index)
      {
        if (this.m_TrackedButtons[index].button == button)
        {
          buttonState = this.m_TrackedButtons[index];
          break;
        }
      }
      if (buttonState == null)
      {
        buttonState = new ButtonState()
        {
          button = button,
          eventData = new MouseButtonEventData()
        };
        this.m_TrackedButtons.Add(buttonState);
      }
      return buttonState;
    }

    public void SetButtonState(
      PointerEventData.InputButton button,
      PointerEventData.FramePressState stateForMouseButton,
      PointerEventData data)
    {
      ButtonState buttonState = this.GetButtonState(button);
      buttonState.eventData.buttonState = stateForMouseButton;
      buttonState.eventData.buttonData = data;
    }
  }
}
