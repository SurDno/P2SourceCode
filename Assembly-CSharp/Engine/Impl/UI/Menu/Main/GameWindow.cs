using System;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using Engine.Source.Utility;
using InputServices;

namespace Engine.Impl.UI.Menu.Main
{
  public class GameWindow : UIWindow, IGameWindow, IWindow, IPauseMenu
  {
    [Header("Main menu")]
    [SerializeField]
    [FormerlySerializedAs("Button_NewGame")]
    private Button buttonNewGame;
    [SerializeField]
    private Button buttonExit;
    [Header("Sounds")]
    [SerializeField]
    [FormerlySerializedAs("ClickSound")]
    private AudioClip clickSound;
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private ConfirmationWindow exitConfirmationPrefab;
    private ConfirmationWindow exitConfirmationInstance;
    private CameraKindEnum lastCameraKind;
    [SerializeField]
    private GameObject helpPanel;
    [SerializeField]
    private Image selectedLine;
    private int currentIndex;
    private Button[] buttons = (Button[]) null;
    private bool isReturnedFromGame = true;

    public GameObject Menu => menu;

    public override void Initialize()
    {
      RegisterLayer((IGameWindow) this);
      Button[] componentsInChildren = this.GetComponentsInChildren<Button>(true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((UnityAction<BaseEventData>) (eventData => Button_Click_Handler()));
        componentsInChildren[index].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
      }
      base.Initialize();
    }

    public void Button_Click_Handler()
    {
      if (!this.gameObject.activeInHierarchy)
        return;
      this.gameObject.GetComponent<AudioSource>().PlayOneShot(clickSound);
    }

    public void Button_BackToGame_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

    public void Button_Settings_Click_Handler()
    {
      SettingsMenuHelper.Instatnce.Reset();
      ServiceLocator.GetService<UIService>().Push<IGameLanguageSettingsWindow>();
    }

    public void Button_ExitGame_Click_Handler() => ShowExitConfirmation(Exit);

    public void Button_NewGame_Click_Handler()
    {
      ShowExitConfirmation(ToMainMenu);
    }

    public void Button_LoadGame_Click_Handler()
    {
      ServiceLocator.GetService<UIService>().Push<IGameLoadGameWindow>();
    }

    protected override void OnEnable()
    {
      buttonExit.gameObject.SetActive(true);
      base.OnEnable();
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      PlayerUtility.ShowPlayerHands(false);
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      if (buttons == null)
        buttons = this.GetComponentsInChildren<Button>();
      service.AddListener(GameActionType.LStickDown, ChangeSelectedItem);
      service.AddListener(GameActionType.LStickUp, ChangeSelectedItem);
      service.AddListener(GameActionType.Submit, ChangeSelectedItem);
      service.AddListener(GameActionType.Cancel, BackButton);
      service.AddListener(GameActionType.MainMenu, BackButton);
      selectedLine.enabled = false;
    }

    protected override void OnDisable()
    {
      if ((UnityEngine.Object) exitConfirmationInstance != (UnityEngine.Object) null)
        exitConfirmationInstance.Hide();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      CursorService.Instance.Free = CursorService.Instance.Visible = false;
      PlayerUtility.ShowPlayerHands(true);
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.LStickDown, ChangeSelectedItem);
      service.RemoveListener(GameActionType.LStickUp, ChangeSelectedItem);
      service.RemoveListener(GameActionType.Submit, ChangeSelectedItem);
      service.RemoveListener(GameActionType.Cancel, BackButton);
      service.RemoveListener(GameActionType.MainMenu, BackButton);
      base.OnDisable();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      helpPanel.SetActive(joystick);
      EventSystem.current.SetSelectedGameObject((GameObject) null);
      if (joystick)
      {
        if (isReturnedFromGame)
        {
          EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
          currentIndex = 0;
        }
        else
          EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
      }
      SetSelectionLine();
    }

    private bool BackButton(GameActionType type, bool down)
    {
      if (!down)
        return false;
      Button_BackToGame_Click_Handler();
      return true;
    }

    private bool ChangeSelectedItem(GameActionType type, bool down)
    {
      if (!helpPanel.activeInHierarchy || (UnityEngine.Object) exitConfirmationInstance != (UnityEngine.Object) null && exitConfirmationInstance.gameObject.activeInHierarchy)
        return false;
      if (type == GameActionType.LStickUp & down)
      {
        --currentIndex;
        ChangeSelection();
        return true;
      }
      if (type == GameActionType.LStickDown & down)
      {
        ++currentIndex;
        ChangeSelection();
        return true;
      }
      if (type == GameActionType.Submit & down)
      {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        isReturnedFromGame = currentIndex == 0;
        ExecuteEvents.Execute<ISubmitHandler>(buttons[currentIndex].gameObject, (BaseEventData) eventData, ExecuteEvents.submitHandler);
        return true;
      }
      if (!(type == GameActionType.Cancel & down))
        return false;
      isReturnedFromGame = true;
      currentIndex = 0;
      return true;
    }

    private void ChangeSelection()
    {
      if (currentIndex > buttons.Length - 1)
        currentIndex = 0;
      if (currentIndex < 0)
        currentIndex = buttons.Length - 1;
      EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
      SetSelectionLine();
    }

    private void SetSelectionLine()
    {
      GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
      Text component = (UnityEngine.Object) selectedGameObject != (UnityEngine.Object) null ? selectedGameObject.GetComponent<Text>() : (Text) null;
      bool flag = (UnityEngine.Object) component != (UnityEngine.Object) null && InputService.Instance.JoystickUsed;
      selectedLine.enabled = flag;
      if (!flag)
        return;
      Vector2 sizeDelta = selectedLine.rectTransform.sizeDelta;
      selectedLine.rectTransform.position = component.rectTransform.position;
      sizeDelta.x = component.preferredWidth;
      selectedLine.rectTransform.sizeDelta = sizeDelta;
    }

    private void Exit() => InstanceByRequest<EngineApplication>.Instance.Exit();

    private void ToMainMenu() => ServiceLocator.GetService<GameLauncher>().ExitToMainMenu();

    private void ShowExitConfirmation(Action onAccept)
    {
      if ((UnityEngine.Object) exitConfirmationInstance == (UnityEngine.Object) null)
        exitConfirmationInstance = UnityEngine.Object.Instantiate<ConfirmationWindow>(exitConfirmationPrefab, this.transform, false);
      exitConfirmationInstance.Show("{UI.Menu.Main.Game.ExitConfirmation}", onAccept, null);
    }
  }
}
