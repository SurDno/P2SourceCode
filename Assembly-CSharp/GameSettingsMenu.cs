using System;
using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;

public class GameSettingsMenu : MonoBehaviour
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
  private Image selectedLine;
  [SerializeField]
  private GameObject toolTip;
  private static int currentIndex;
  private static int selectedIndex;
  private int bufferedViewIndex = -1;
  private Button[] buttons;

  private void Awake()
  {
    languageButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IGameLanguageSettingsWindow>));
    difficultyButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IGameDifficultySettingsWindow>));
    displayButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IGameDisplaySettingsWindow>));
    graphicsButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IGameGraphicsSettingsWindow>));
    controlButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IGameControlSettingsWindow>));
    keysButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IGameKeySettingsWindow>));
    soundButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IGameSoundSettingsWindow>));
    backerUnlocksButton.onClick.AddListener(new UnityAction(SettingsMenuHelper.Instatnce.ShowSettings<IGameBackerUnlocksWindow>));
    ShowSelectableButton();
  }

  private void ShowSelectableButton()
  {
    IWindow componentInParent = this.GetComponentInParent<IWindow>();
    languageButton.interactable = !(componentInParent is IGameLanguageSettingsWindow);
    difficultyButton.interactable = !(componentInParent is IGameDifficultySettingsWindow);
    displayButton.interactable = !(componentInParent is IGameDisplaySettingsWindow);
    graphicsButton.interactable = !(componentInParent is IGameGraphicsSettingsWindow);
    controlButton.interactable = !(componentInParent is IGameControlSettingsWindow);
    keysButton.interactable = !(componentInParent is IGameKeySettingsWindow);
    soundButton.interactable = !(componentInParent is IGameSoundSettingsWindow);
    backerUnlocksButton.interactable = !(componentInParent is IGameBackerUnlocksWindow);
  }

  private void OnJoystick(bool isUsed)
  {
    buttons = new List<Button>((IEnumerable<Button>) this.GetComponentsInChildren<Button>()).FindAll((Predicate<Button>) (b => b.gameObject.activeInHierarchy)).ToArray();
    if (isUsed)
    {
      toolTip.SetActive(!SettingsMenuHelper.Instatnce.isSelected);
      for (int index = 0; index < buttons.Length; ++index)
      {
        if (!buttons[index].interactable)
          selectedIndex = index;
        buttons[index].interactable = true;
      }
      currentIndex = selectedIndex;
      ChangeSelection();
    }
    else
    {
      if (bufferedViewIndex != -1)
      {
        Select(GameActionType.Submit, true);
        bufferedViewIndex = -1;
      }
      ShowSelectableButton();
    }
    selectedLine.gameObject.SetActive(isUsed);
  }

  private bool RefreshCurrentIndex(GameActionType type, bool down)
  {
    currentIndex = 0;
    return false;
  }

  private bool ChangeSelectedItem(GameActionType type, bool down)
  {
    if (type == GameActionType.LStickUp & down)
    {
      --currentIndex;
      ChangeSelection();
      return true;
    }
    if (!(type == GameActionType.LStickDown & down))
      return false;
    ++currentIndex;
    ChangeSelection();
    return true;
  }

  private void ChangeSelection()
  {
    buttons = new List<Button>((IEnumerable<Button>) this.GetComponentsInChildren<Button>()).FindAll((Predicate<Button>) (b => b.gameObject.activeInHierarchy)).ToArray();
    if (currentIndex > buttons.Length - 1)
      currentIndex = 0;
    if (currentIndex < 0)
      currentIndex = buttons.Length - 1;
    bufferedViewIndex = currentIndex;
    EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
    ChangeLinePosition();
  }

  private bool Select(GameActionType type, bool down)
  {
    if (!down)
      return false;
    if (selectedIndex == currentIndex)
    {
      SettingsMenuHelper.Instatnce.SetSelectedState();
      return false;
    }
    ChangeLinePosition();
    PointerEventData eventData = new PointerEventData(EventSystem.current);
    ExecuteEvents.Execute<ISubmitHandler>(buttons[currentIndex].gameObject, (BaseEventData) eventData, ExecuteEvents.submitHandler);
    selectedIndex = currentIndex;
    SettingsMenuHelper.Instatnce.SetSelectedState();
    return true;
  }

  private void ChangeLinePosition()
  {
    selectedLine.transform.SetParent(buttons[currentIndex].transform, false);
    selectedLine.rectTransform.anchoredPosition = selectedLine.rectTransform.anchoredPosition with
    {
      y = 0.0f
    };
    selectedLine.rectTransform.sizeDelta = selectedLine.rectTransform.sizeDelta with
    {
      x = buttons[currentIndex].GetComponentInChildren<Text>().preferredWidth
    };
  }

  public void OnEnable()
  {
    CursorService.Instance.Free = true;
    if (!InputService.Instance.JoystickUsed)
      CursorService.Instance.Visible = true;
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    service.AddListener(GameActionType.LStickDown, ChangeSelectedItem, true);
    service.AddListener(GameActionType.LStickUp, ChangeSelectedItem, true);
    service.AddListener(GameActionType.Submit, Select, true);
    SettingsMenuHelper.Instatnce.Activate(true);
    SettingsMenuHelper.Instatnce.OnStateSelected += OnStateSelected;
    InputService.Instance.onJoystickUsedChanged += OnJoystick;
    CoroutineService.Instance.WaitFrame(1, (Action) (() =>
    {
      OnJoystick(InputService.Instance.JoystickUsed);
      ChangeLinePosition();
    }));
  }

  public void OnDisable()
  {
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    service.RemoveListener(GameActionType.LStickDown, ChangeSelectedItem);
    service.RemoveListener(GameActionType.LStickUp, ChangeSelectedItem);
    service.RemoveListener(GameActionType.Submit, Select);
    SettingsMenuHelper.Instatnce.OnStateSelected -= OnStateSelected;
    InputService.Instance.onJoystickUsedChanged -= OnJoystick;
    SettingsMenuHelper.Instatnce.Activate(false);
    CoroutineService.Instance.WaitFrame(1, (Action) (() =>
    {
      if (SettingsMenuHelper.Instatnce.isSelected)
        return;
      currentIndex = 0;
      selectedIndex = 0;
    }));
    selectedLine.gameObject.SetActive(false);
  }

  private void OnStateSelected(bool isSelected)
  {
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    toolTip?.SetActive(!isSelected);
    if (isSelected)
    {
      service.RemoveListener(GameActionType.LStickDown, ChangeSelectedItem);
      service.RemoveListener(GameActionType.LStickUp, ChangeSelectedItem);
      service.RemoveListener(GameActionType.Submit, Select);
    }
    else
    {
      service.AddListener(GameActionType.LStickDown, ChangeSelectedItem, true);
      service.AddListener(GameActionType.LStickUp, ChangeSelectedItem, true);
      service.AddListener(GameActionType.Submit, Select);
    }
  }
}
