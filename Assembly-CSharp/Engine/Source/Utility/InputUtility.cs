using Cofe.Utility;
using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using InputServices;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Engine.Source.Utility
{
  public static class InputUtility
  {
    public static string GetTagName(ActionGroup bind) => "{ActionGroup." + bind.Name + "}";

    public static bool IsKeyDown(KeyCode key, KeyModifficator modifficators = KeyModifficator.None)
    {
      return Input.GetKeyDown(key) && InputUtility.CheckModificators(modifficators);
    }

    public static bool CheckModificators(KeyModifficator modifficators)
    {
      return (Input.GetKey(KeyCode.LeftControl) ? 1 : (Input.GetKey(KeyCode.RightControl) ? 1 : 0)) == (KeyModifficatorUtility.HasValue(modifficators, KeyModifficator.Control) ? 1 : 0) && (Input.GetKey(KeyCode.LeftShift) ? 1 : (Input.GetKey(KeyCode.RightShift) ? 1 : 0)) == (KeyModifficatorUtility.HasValue(modifficators, KeyModifficator.Shift) ? 1 : 0) && (Input.GetKey(KeyCode.LeftAlt) ? 1 : (Input.GetKey(KeyCode.RightAlt) ? 1 : 0)) == (KeyModifficatorUtility.HasValue(modifficators, KeyModifficator.Alt) ? 1 : 0);
    }

    public static string GetHotKeyText(KeyCode key, KeyModifficator modifficators)
    {
      string str1 = "";
      foreach (KeyModifficator keyModifficator in Enum.GetValues(typeof (KeyModifficator)))
      {
        if ((modifficators & keyModifficator) != 0)
          str1 = str1 + (object) keyModifficator + " + ";
      }
      string str2;
      switch (key)
      {
        case KeyCode.Mouse0:
          str2 = "MouseLeft";
          break;
        case KeyCode.Mouse1:
          str2 = "MouseRight";
          break;
        default:
          str2 = key.ToString();
          break;
      }
      string str3 = str2.Replace("Alpha", "");
      return str1 + str3;
    }

    public static string GetHotKeyNameByGroup(ActionGroup action, bool joystick, out bool isHold)
    {
      ActionGroup actionGroup = ServiceLocator.GetService<GameActionService>().GetBinds().FirstOrDefault<ActionGroup>((Func<ActionGroup, bool>) (o => o.Name == action.Name));
      string hotKeyNameByGroup = "";
      isHold = false;
      if (actionGroup != null)
      {
        if (joystick)
        {
          if (InputService.Instance.JoystickPresent)
          {
            if (actionGroup.JoystickHold)
            {
              isHold = true;
              hotKeyNameByGroup += "Hold ";
            }
            if (!actionGroup.Joystick.IsNullOrEmpty())
              hotKeyNameByGroup += actionGroup.Joystick;
          }
        }
        else if (actionGroup.Key != 0)
          hotKeyNameByGroup += InputUtility.GetHotKeyText(actionGroup.Key, KeyModifficator.None);
      }
      return hotKeyNameByGroup;
    }

    public static string GetHotKeyNameByActionWithoutHold(GameActionType action)
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      ActionGroup bind = service != null ? service.GetBinds().FirstOrDefault<ActionGroup>((Func<ActionGroup, bool>) (o =>
      {
        if (!o.Actions.Contains(action))
          return false;
        return !InputService.Instance.JoystickUsed || o.Joystick != "";
      })) : (ActionGroup) null;
      string actionWithoutHold = "";
      if (bind != null)
      {
        ActionGroup actionGroup = ServiceLocator.GetService<GameActionService>().GetBinds().FirstOrDefault<ActionGroup>((Func<ActionGroup, bool>) (o => o.Name == bind.Name));
        if (actionGroup != null && InputService.Instance.JoystickPresent && !actionGroup.Joystick.IsNullOrEmpty())
          actionWithoutHold += actionGroup.Joystick;
      }
      return actionWithoutHold;
    }

    public static string GetHotKeyNameByGroup(ActionGroup action, bool joystick)
    {
      ActionGroup bind = ServiceLocator.GetService<GameActionService>().GetBinds().FirstOrDefault<ActionGroup>((Func<ActionGroup, bool>) (o => o.Name == action.Name));
      if (joystick && bind.Joystick == "")
      {
        ActionGroup actionGroup = ServiceLocator.GetService<GameActionService>().GetBinds().FirstOrDefault<ActionGroup>((Func<ActionGroup, bool>) (o => o.Actions.Intersect<GameActionType>((IEnumerable<GameActionType>) bind.Actions).Count<GameActionType>() > 0 && o.Joystick != ""));
        if (actionGroup != null)
          bind = actionGroup;
      }
      string hotKeyNameByGroup = "";
      if (bind != null)
      {
        if (joystick)
        {
          if (InputService.Instance.JoystickPresent)
          {
            if (bind.JoystickHold)
              hotKeyNameByGroup += "Hold ";
            if (!bind.Joystick.IsNullOrEmpty())
              hotKeyNameByGroup += bind.Joystick;
          }
        }
        else if (bind.Key != 0)
          hotKeyNameByGroup += InputUtility.GetHotKeyText(bind.Key, KeyModifficator.None);
      }
      return hotKeyNameByGroup;
    }

    public static string GetHotKeyNameByAction(
      GameActionType action,
      bool joystick,
      out bool isHold)
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      ActionGroup action1 = service != null ? service.GetBinds().FirstOrDefault<ActionGroup>((Func<ActionGroup, bool>) (o =>
      {
        if (!o.Actions.Contains(action))
          return false;
        return !joystick || o.Joystick != "";
      })) : (ActionGroup) null;
      if (action1 != null)
        return InputUtility.GetHotKeyNameByGroup(action1, joystick, out isHold);
      isHold = false;
      return "";
    }

    public static string GetHotKeyNameByAction(GameActionType action, bool joystick)
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      ActionGroup action1 = service != null ? service.GetBinds().FirstOrDefault<ActionGroup>((Func<ActionGroup, bool>) (o =>
      {
        if (!o.Actions.Contains(action))
          return false;
        return !joystick || o.Joystick != "";
      })) : (ActionGroup) null;
      return action1 != null ? InputUtility.GetHotKeyNameByGroup(action1, joystick) : "";
    }

    public static string GetHotKeyByGroup(ActionGroup action, bool joystick, out bool hold)
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      if ((service != null ? service.GetBinds().FirstOrDefault<ActionGroup>((Func<ActionGroup, bool>) (o => o.Name == action.Name)) : (ActionGroup) null) == null)
      {
        hold = false;
        return (string) null;
      }
      if (joystick)
      {
        hold = action.JoystickHold;
        return action.Joystick;
      }
      hold = false;
      return action.Key.ToString();
    }

    public static string GetHotKeyByAction(GameActionType action, bool joystick, out bool hold)
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      ActionGroup action1 = service != null ? service.GetBinds().FirstOrDefault<ActionGroup>((Func<ActionGroup, bool>) (o =>
      {
        if (!o.Actions.Contains(action))
          return false;
        return !joystick || o.Joystick != "";
      })) : (ActionGroup) null;
      if (action1 != null)
        return InputUtility.GetHotKeyByGroup(action1, joystick, out hold);
      hold = false;
      return (string) null;
    }
  }
}
