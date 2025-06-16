using InputServices;
using System;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class JoystickCheck : MonoBehaviour
  {
    [SerializeField]
    private HideableView view;
    private Action<bool> onJoystickAction;

    private void OnDisable()
    {
      InputService.Instance.onJoystickUsedChanged -= this.onJoystickAction;
    }

    private void OnEnable()
    {
      InputService instance = InputService.Instance;
      this.view.Visible = instance.JoystickUsed;
      if (this.onJoystickAction == null)
        this.onJoystickAction = new Action<bool>(this.OnJoystick);
      instance.onJoystickUsedChanged += this.onJoystickAction;
    }

    private void OnJoystick(bool joystick) => this.view.Visible = joystick;
  }
}
