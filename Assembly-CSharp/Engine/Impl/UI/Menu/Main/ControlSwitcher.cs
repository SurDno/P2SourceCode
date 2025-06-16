using System;
using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using InputServices;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Main
{
  public class ControlSwitcher : IDisposable
  {
    private List<Button> _buttons = new List<Button>();
    private Dictionary<GameActionType, List<Action>> _listeners = new Dictionary<GameActionType, List<Action>>();

    public void SubmitAction(Button button, GameActionType type, Action action)
    {
      button.onClick.AddListener(() => action());
      _buttons.Add(button);
      if (!_listeners.ContainsKey(type))
      {
        _listeners.Add(type, new List<Action>());
        ServiceLocator.GetService<GameActionService>().AddListener(type, OnAction);
      }
      _listeners[type].Add(action);
    }

    private bool OnAction(GameActionType type, bool down)
    {
      if (down && InputService.Instance.JoystickUsed && _listeners.ContainsKey(type))
      {
        foreach (Action action in _listeners[type])
          action();
      }
      return false;
    }

    public void Dispose()
    {
      foreach (Button button in _buttons)
        button.onClick.RemoveAllListeners();
      _buttons.Clear();
      foreach (KeyValuePair<GameActionType, List<Action>> listener in _listeners)
        ServiceLocator.GetService<GameActionService>().RemoveListener(listener.Key, OnAction);
      _listeners.Clear();
    }
  }
}
