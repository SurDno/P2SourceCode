using System.Collections.Generic;
using InputServices;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Main
{
  public class ControlHelpSwitcher : MonoBehaviour
  {
    [SerializeField]
    private List<GameObject> _pcControls;
    [SerializeField]
    private List<GameObject> _consoleControls;

    private void Awake() => OnJoystick(InputService.Instance.JoystickUsed);

    private void OnEnable()
    {
      InputService.Instance.onJoystickUsedChanged += OnJoystick;
      OnJoystick(InputService.Instance.JoystickUsed);
    }

    private void OnDisable()
    {
      InputService.Instance.onJoystickUsedChanged -= OnJoystick;
    }

    public void OnJoystick(bool isUsed)
    {
      SetActiveAllGameObjects(isUsed ? _pcControls : _consoleControls, false);
      SetActiveAllGameObjects(isUsed ? _consoleControls : _pcControls, true);
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
