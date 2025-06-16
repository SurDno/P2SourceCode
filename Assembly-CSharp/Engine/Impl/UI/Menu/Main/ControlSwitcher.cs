// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Main.ControlSwitcher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using InputServices;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
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
