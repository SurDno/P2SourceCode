// Decompiled with JetBrains decompiler
// Type: StartSettingsMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
public class StartSettingsMenu : MonoBehaviour
{
  [SerializeField]
  private Button languageButton;
  [SerializeField]
  private Button difficultyButton;
  [SerializeField]
  private Button displayButton;
  [SerializeField]
  private Button graphicsButton;
  [SerializeField]
  private Button controlButton;
  [SerializeField]
  private Button keysButton;
  [SerializeField]
  private Button soundButton;
  [SerializeField]
  private Button backerUnlocksButton;
  [SerializeField]
  private Button xboxLiveButton;
  [SerializeField]
  private Image selectedLine;
  [SerializeField]
  private GameObject toolTip;
  private static int currentIndex = 0;
  private static int selectedIndex = 0;
  private int bufferedViewIndex = -1;
  private Button[] buttons;

  private void Awake()
  {
    this.languageButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IStartLanguageSettingsWindow>));
    this.difficultyButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IStartDifficultySettingsWindow>));
    this.displayButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IStartDisplaySettingsWindow>));
    this.graphicsButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IStartGraphicsSettingsWindow>));
    this.controlButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IStartControlSettingsWindow>));
    this.keysButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IStartKeySettingsWindow>));
    this.soundButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IStartSoundSettingsWindow>));
    this.backerUnlocksButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IStartBackerUnlocksWindow>));
    this.ShowSelectableButton();
  }

  private void ShowSelectableButton()
  {
    IWindow componentInParent = this.GetComponentInParent<IWindow>();
    this.languageButton.interactable = !(componentInParent is IStartLanguageSettingsWindow);
    this.difficultyButton.interactable = !(componentInParent is IStartDifficultySettingsWindow);
    this.displayButton.interactable = !(componentInParent is IStartDisplaySettingsWindow);
    this.graphicsButton.interactable = !(componentInParent is IStartGraphicsSettingsWindow);
    this.controlButton.interactable = !(componentInParent is IStartControlSettingsWindow);
    this.keysButton.interactable = !(componentInParent is IStartKeySettingsWindow);
    this.soundButton.interactable = !(componentInParent is IStartSoundSettingsWindow);
    this.backerUnlocksButton.interactable = !(componentInParent is IStartBackerUnlocksWindow);
  }

  private void OnJoystick(bool isUsed)
  {
    this.buttons = new List<Button>((IEnumerable<Button>) this.GetComponentsInChildren<Button>()).FindAll((Predicate<Button>) (b => b.gameObject.activeInHierarchy)).ToArray();
    if (isUsed)
    {
      this.toolTip.SetActive(!SettingsMenuHelper.Instatnce.isSelected);
      for (int index = 0; index < this.buttons.Length; ++index)
      {
        if (!this.buttons[index].interactable)
          StartSettingsMenu.selectedIndex = index;
        this.buttons[index].interactable = true;
      }
      StartSettingsMenu.currentIndex = StartSettingsMenu.selectedIndex;
      this.ChangeSelection();
    }
    else
    {
      if (this.bufferedViewIndex != -1)
      {
        this.Select(GameActionType.Submit, true);
        this.bufferedViewIndex = -1;
      }
      this.ShowSelectableButton();
    }
    this.selectedLine.gameObject.SetActive(isUsed);
  }

  private bool RefreshCurrentIndex(GameActionType type, bool down)
  {
    StartSettingsMenu.currentIndex = 0;
    return false;
  }

  private bool ChangeSelectedItem(GameActionType type, bool down)
  {
    if (type == GameActionType.LStickUp & down)
    {
      --StartSettingsMenu.currentIndex;
      this.ChangeSelection();
      return true;
    }
    if (!(type == GameActionType.LStickDown & down))
      return false;
    ++StartSettingsMenu.currentIndex;
    this.ChangeSelection();
    return true;
  }

  private void ChangeSelection()
  {
    this.buttons = new List<Button>((IEnumerable<Button>) this.GetComponentsInChildren<Button>()).FindAll((Predicate<Button>) (b => b.gameObject.activeInHierarchy)).ToArray();
    if (StartSettingsMenu.currentIndex > this.buttons.Length - 1)
      StartSettingsMenu.currentIndex = 0;
    if (StartSettingsMenu.currentIndex < 0)
      StartSettingsMenu.currentIndex = this.buttons.Length - 1;
    this.bufferedViewIndex = StartSettingsMenu.currentIndex;
    EventSystem.current.SetSelectedGameObject(this.buttons[StartSettingsMenu.currentIndex].gameObject);
    this.ChangeLinePosition();
  }

  private bool Select(GameActionType type, bool down)
  {
    if (!down)
      return false;
    if (StartSettingsMenu.selectedIndex == StartSettingsMenu.currentIndex)
    {
      SettingsMenuHelper.Instatnce.SetSelectedState();
      return false;
    }
    this.ChangeLinePosition();
    PointerEventData eventData = new PointerEventData(EventSystem.current);
    ExecuteEvents.Execute<ISubmitHandler>(this.buttons[StartSettingsMenu.currentIndex].gameObject, (BaseEventData) eventData, ExecuteEvents.submitHandler);
    StartSettingsMenu.selectedIndex = StartSettingsMenu.currentIndex;
    SettingsMenuHelper.Instatnce.SetSelectedState();
    return true;
  }

  private void ChangeLinePosition()
  {
    this.selectedLine.transform.SetParent(this.buttons[StartSettingsMenu.currentIndex].transform, false);
    this.selectedLine.rectTransform.anchoredPosition = this.selectedLine.rectTransform.anchoredPosition with
    {
      y = 0.0f
    };
    this.selectedLine.rectTransform.sizeDelta = this.selectedLine.rectTransform.sizeDelta with
    {
      x = this.buttons[StartSettingsMenu.currentIndex].GetComponentInChildren<Text>().preferredWidth
    };
  }

  public void OnEnable()
  {
    CursorService.Instance.Free = true;
    if (!InputService.Instance.JoystickUsed)
      CursorService.Instance.Visible = true;
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    service.AddListener(GameActionType.LStickDown, new GameActionHandle(this.ChangeSelectedItem), true);
    service.AddListener(GameActionType.LStickUp, new GameActionHandle(this.ChangeSelectedItem), true);
    service.AddListener(GameActionType.Submit, new GameActionHandle(this.Select), true);
    SettingsMenuHelper.Instatnce.Activate(true);
    SettingsMenuHelper.Instatnce.OnStateSelected += new Action<bool>(this.OnStateSelected);
    InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
    CoroutineService.Instance.WaitFrame(1, (Action) (() =>
    {
      this.OnJoystick(InputService.Instance.JoystickUsed);
      this.ChangeLinePosition();
    }));
  }

  public void OnDisable()
  {
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    service.RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.ChangeSelectedItem));
    service.RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.ChangeSelectedItem));
    service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.Select));
    SettingsMenuHelper.Instatnce.OnStateSelected -= new Action<bool>(this.OnStateSelected);
    InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
    SettingsMenuHelper.Instatnce.Activate(false);
    CoroutineService.Instance.WaitFrame(1, (Action) (() =>
    {
      if (SettingsMenuHelper.Instatnce.isSelected)
        return;
      StartSettingsMenu.currentIndex = 0;
      StartSettingsMenu.selectedIndex = 0;
    }));
    this.selectedLine.gameObject.SetActive(false);
  }

  private void OnStateSelected(bool isSelected)
  {
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    this.toolTip?.SetActive(!isSelected);
    if (isSelected)
    {
      service.RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.Select));
    }
    else
    {
      service.AddListener(GameActionType.LStickDown, new GameActionHandle(this.ChangeSelectedItem), true);
      service.AddListener(GameActionType.LStickUp, new GameActionHandle(this.ChangeSelectedItem), true);
      service.AddListener(GameActionType.Submit, new GameActionHandle(this.Select));
    }
  }
}
