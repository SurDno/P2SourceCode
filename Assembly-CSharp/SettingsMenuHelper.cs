using System;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;

public class SettingsMenuHelper
{
  private static SettingsMenuHelper _instance;
  public bool isSelected;
  private bool isActive;

  private event Action<bool> _onStateSelected;

  public event Action<bool> OnStateSelected
  {
    add
    {
      value(isSelected);
      _onStateSelected += value;
    }
    remove => _onStateSelected -= value;
  }

  public static SettingsMenuHelper Instatnce
  {
    get
    {
      if (_instance == null)
        _instance = new SettingsMenuHelper();
      return _instance;
    }
  }

  private SettingsMenuHelper()
  {
    if (InputService.Instance.JoystickUsed)
      return;
    SetSelectedState();
  }

  private void OnJoystick(bool isUsed)
  {
    if (isUsed)
      return;
    SetSelectedState();
  }

  public void ShowSettings<T>() where T : class, IWindow
  {
    ServiceLocator.GetService<UIService>().Swap<T>();
  }

  public void SetSelectedState()
  {
    isSelected = true;
    Action<bool> onStateSelected = _onStateSelected;
    if (onStateSelected != null)
      onStateSelected(true);
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, OnBack, true);
  }

  private bool OnBack(GameActionType type, bool down)
  {
    if (!InputService.Instance.JoystickUsed)
      return false;
    if (down)
      SetChangingState();
    return down;
  }

  public void SetChangingState()
  {
    isSelected = false;
    Action<bool> onStateSelected = _onStateSelected;
    if (onStateSelected != null)
      onStateSelected(false);
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, OnBack);
  }

  public void Reset()
  {
    if (!InputService.Instance.JoystickUsed)
      return;
    SetChangingState();
  }

  public void Activate(bool active)
  {
    isActive = active;
    if (isActive)
    {
      if (!InputService.Instance.JoystickUsed)
        SetSelectedState();
      InputService.Instance.onJoystickUsedChanged += OnJoystick;
    }
    else
    {
      SetChangingState();
      InputService.Instance.onJoystickUsedChanged -= OnJoystick;
    }
  }
}
