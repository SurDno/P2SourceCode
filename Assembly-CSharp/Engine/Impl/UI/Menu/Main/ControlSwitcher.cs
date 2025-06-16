using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using InputServices;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Main
{
  public class ControlSwitcher : IDisposable
  {
    private List<Button> _buttons = new List<Button>();
    private Dictionary<GameActionType, List<Action>> _listeners = new Dictionary<GameActionType, List<Action>>();

    public void SubmitAction(Button button, GameActionType type, Action action)
    {
      button.onClick.AddListener((UnityAction) (() => action()));
      this._buttons.Add(button);
      if (!this._listeners.ContainsKey(type))
      {
        this._listeners.Add(type, new List<Action>());
        ServiceLocator.GetService<GameActionService>().AddListener(type, new GameActionHandle(this.OnAction));
      }
      this._listeners[type].Add(action);
    }

    private bool OnAction(GameActionType type, bool down)
    {
      if (down && InputService.Instance.JoystickUsed && this._listeners.ContainsKey(type))
      {
        foreach (Action action in this._listeners[type])
          action();
      }
      return false;
    }

    public void Dispose()
    {
      foreach (Button button in this._buttons)
        button.onClick.RemoveAllListeners();
      this._buttons.Clear();
      foreach (KeyValuePair<GameActionType, List<Action>> listener in this._listeners)
        ServiceLocator.GetService<GameActionService>().RemoveListener(listener.Key, new GameActionHandle(this.OnAction));
      this._listeners.Clear();
    }
  }
}
