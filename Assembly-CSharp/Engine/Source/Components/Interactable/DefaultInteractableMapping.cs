using System;
using System.Collections.Generic;
using Cofe.Utility;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;

namespace Engine.Source.Components.Interactable
{
  public static class DefaultInteractableMapping
  {
    public static InteractableWindow.IconType GetIconType(
      IInteractableComponent interactable,
      List<InteractItemInfo> validateItems)
    {
      foreach (InteractItemInfo validateItem in validateItems)
      {
        if (validateItem.Item.Type == InteractType.IconNormal)
          return InteractableWindow.IconType.Normal;
        if (validateItem.Item.Type == InteractType.IconLocked)
          return InteractableWindow.IconType.Locked;
        if (validateItem.Item.Type == InteractType.IconBlocked)
          return InteractableWindow.IconType.Blocked;
      }
      IDoorComponent component = interactable.GetComponent<IDoorComponent>();
      if (component == null)
        return InteractableWindow.IconType.None;
      IPriorityParameterValue<LockState> lockState = component.LockState;
      if (lockState.Value == LockState.Blocked)
        return InteractableWindow.IconType.Blocked;
      return lockState.Value == LockState.Locked ? InteractableWindow.IconType.Locked : InteractableWindow.IconType.Normal;
    }

    public static string GetText(List<InteractItemInfo> validateItems)
    {
      LocalizationService service = ServiceLocator.GetService<LocalizationService>();
      string text1 = "";
      foreach (InteractItemInfo validateItem in validateItems)
      {
        if (InstanceByRequest<EngineApplication>.Instance.IsDebug || !validateItem.Invalid)
        {
          bool isHold;
          string hotKeyNameByAction1 = InputUtility.GetHotKeyNameByAction(validateItem.Item.Action, InputService.Instance.JoystickUsed, out isHold);
          if (!hotKeyNameByAction1.IsNullOrEmpty())
          {
            if (!text1.IsNullOrEmpty())
              text1 += "\n";
            if (validateItem.Invalid)
            {
              string hotKeyNameByAction2 = InputUtility.GetHotKeyNameByAction(validateItem.OverrideAction, InputService.Instance.JoystickUsed, out isHold);
              text1 = text1 + "<color=" + ScriptableObjectInstance<GameSettingsData>.Instance.InteractableDebugTextColor.ToRGBHex() + ">[" + hotKeyNameByAction2 + "   ( " + hotKeyNameByAction1 + " ) ] : " + service.GetText(validateItem.Item.Title);
              text1 += "   [ Debug ]";
              if (validateItem.Crime)
                text1 += "   [ Crime ]";
              if (validateItem.Dublicate)
                text1 += "   [ Dublicate ]";
            }
            else if (validateItem.Crime)
            {
              text1 = text1 + "<color=" + ScriptableObjectInstance<GameSettingsData>.Instance.InteractableCrimeTextColor.ToRGBHex() + ">[" + hotKeyNameByAction1 + "] : " + service.GetText(validateItem.Item.Title);
              string text2 = service.GetText("{Interact.Crime}");
              text1 = text1 + "   [" + text2 + "]";
            }
            else if (validateItem.Item.Blueprint.Id == Guid.Empty)
              text1 = text1 + "<color=" + ScriptableObjectInstance<GameSettingsData>.Instance.InteractableDisabledTextColor.ToRGBHex() + ">[" + hotKeyNameByAction1 + "] : " + service.GetText(validateItem.Item.Title);
            else
              text1 = text1 + "<color=" + ScriptableObjectInstance<GameSettingsData>.Instance.InteractableNormalTextColor.ToRGBHex() + ">[" + hotKeyNameByAction1 + "] : " + service.GetText(validateItem.Item.Title);
            if (InstanceByRequest<EngineApplication>.Instance.IsDebug)
            {
              if (validateItem.Reason != "")
                text1 = text1 + "   ( " + validateItem.Reason + " )";
              text1 += "   ( ";
              text1 = text1 + "Type : " + validateItem.Item.Type + " , ";
              text1 = text1 + "Action : " + validateItem.Item.Action;
              text1 += " )";
            }
            text1 += "</color>";
          }
        }
      }
      return text1;
    }

    public static string[] GetText(
      List<InteractItemInfo> validateItems,
      out List<KeyValuePair<Sprite, bool>> iconSprites,
      out List<KeyValuePair<GameActionType, bool>> actions)
    {
      LocalizationService service = ServiceLocator.GetService<LocalizationService>();
      List<string> stringList = new List<string>();
      iconSprites = new List<KeyValuePair<Sprite, bool>>();
      actions = new List<KeyValuePair<GameActionType, bool>>();
      foreach (InteractItemInfo validateItem in validateItems)
      {
        string str1 = "";
        bool isHold;
        if ((InstanceByRequest<EngineApplication>.Instance.IsDebug || !validateItem.Invalid) && !InputUtility.GetHotKeyNameByAction(validateItem.Item.Action, InputService.Instance.JoystickUsed, out isHold).IsNullOrEmpty())
        {
          iconSprites.Add(new KeyValuePair<Sprite, bool>(ControlIconsManager.Instance.GetIconSprite(validateItem.Item.Action, out isHold), isHold));
          string str2;
          if (validateItem.Crime)
          {
            str2 = "<color=" + ScriptableObjectInstance<GameSettingsData>.Instance.InteractableCrimeTextColor.ToRGBHex() + "> : " + service.GetText(validateItem.Item.Title) + "   [" + service.GetText("{Interact.Crime}") + "]";
            actions.Add(new KeyValuePair<GameActionType, bool>(validateItem.Item.Action, isHold));
          }
          else if (validateItem.Item.Blueprint.Id == Guid.Empty)
          {
            str2 = str1 + "<color=" + ScriptableObjectInstance<GameSettingsData>.Instance.InteractableDisabledTextColor.ToRGBHex() + "> : " + service.GetText(validateItem.Item.Title);
          }
          else
          {
            str2 = str1 + "<color=" + ScriptableObjectInstance<GameSettingsData>.Instance.InteractableNormalTextColor.ToRGBHex() + "> : " + service.GetText(validateItem.Item.Title);
            actions.Add(new KeyValuePair<GameActionType, bool>(validateItem.Item.Action, isHold));
          }
          string str3 = str2 + "</color>";
          stringList.Add(str3);
        }
      }
      return stringList.ToArray();
    }
  }
}
