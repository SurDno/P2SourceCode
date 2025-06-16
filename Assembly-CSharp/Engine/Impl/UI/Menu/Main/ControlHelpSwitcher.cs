using InputServices;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Main
{
  public class ControlHelpSwitcher : MonoBehaviour
  {
    [SerializeField]
    private List<GameObject> _pcControls;
    [SerializeField]
    private List<GameObject> _consoleControls;

    private void Awake() => this.OnJoystick(InputService.Instance.JoystickUsed);

    private void OnEnable()
    {
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
      this.OnJoystick(InputService.Instance.JoystickUsed);
    }

    private void OnDisable()
    {
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
    }

    public void OnJoystick(bool isUsed)
    {
      ControlHelpSwitcher.SetActiveAllGameObjects(isUsed ? this._pcControls : this._consoleControls, false);
      ControlHelpSwitcher.SetActiveAllGameObjects(isUsed ? this._consoleControls : this._pcControls, true);
    }

    private static void SetActiveAllGameObjects(List<GameObject> list, bool isActive)
    {
      if (list == null)
        return;
      foreach (GameObject gameObject in list)
        gameObject.SetActive(isActive);
    }
  }
}
