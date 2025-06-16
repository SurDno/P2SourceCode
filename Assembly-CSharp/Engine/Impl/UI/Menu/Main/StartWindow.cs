using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.Services.Profiles;
using Engine.Source.UI;
using Engine.Source.UI.Menu.Main;
using InputServices;
using Scripts.Data;

namespace Engine.Impl.UI.Menu.Main
{
  public class StartWindow : UIWindow, IStartWindow, IWindow, IMainMenu
  {
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private Button newGameButton;
    [SerializeField]
    private Button newMarbleNestGameButton;
    [SerializeField]
    private Button loadGameButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button creditsButton;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private string marbleNestGameDataName;
    [SerializeField]
    private GameObject helpPanel;
    [SerializeField]
    private Image selectedLine;
    private int currentIndex;
    private Button[] buttons = (Button[]) null;

    public override void Initialize()
    {
      RegisterLayer((IStartWindow) this);
      if (!BuildSettingsUtility.IsDataExist("MarbleNest"))
        newMarbleNestGameButton.gameObject.SetActive(false);
      continueButton.onClick.AddListener(new UnityAction(ContinueGame));
      newGameButton.onClick.AddListener(new UnityAction(NewGame));
      newMarbleNestGameButton.onClick.AddListener(new UnityAction(NewMarbleNestGame));
      loadGameButton.onClick.AddListener(new UnityAction(OpenLoadGameWindow));
      settingsButton.onClick.AddListener(new UnityAction(OpenSettingsWindow));
      creditsButton.onClick.AddListener(new UnityAction(OpenCreditsWindow));
      exitButton.onClick.AddListener(new UnityAction(ExitGame));
      base.Initialize();
    }

    public void ContinueGame()
    {
      string lastSaveName = ServiceLocator.GetService<ProfilesService>().GetLastSaveName();
      if (!ProfilesUtility.IsSaveExist(lastSaveName))
        Debug.LogError((object) ("Save name not found : " + lastSaveName));
      else
        CoroutineService.Instance.Route(LoadGameUtility.StartGameWithSave(lastSaveName));
    }

    public void NewGame() => CoroutineService.Instance.Route(LoadGameUtility.StartNewGame());

    public void NewMarbleNestGame()
    {
      CoroutineService.Instance.Route(LoadGameUtility.StartNewGame(marbleNestGameDataName));
    }

    public void OpenLoadGameWindow()
    {
      ServiceLocator.GetService<UIService>().Push<IStartProfileWindow>();
    }

    public void OpenSettingsWindow()
    {
      SettingsMenuHelper.Instatnce.Reset();
      ServiceLocator.GetService<UIService>().Push<IStartLanguageSettingsWindow>();
    }

    public void OpenCreditsWindow()
    {
      ServiceLocator.GetService<UIService>().Push<IEndCreditsWindow>();
    }

    public void ExitGame() => InstanceByRequest<EngineApplication>.Instance.Exit();

    protected override void OnEnable()
    {
      exitButton.gameObject.SetActive(true);
      base.OnEnable();
      CursorService.Instance.Free = true;
      if (!InputService.Instance.JoystickUsed)
        CursorService.Instance.Visible = true;
      UpdateContinueButton();
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      currentIndex = 0;
      buttons = this.GetComponentsInChildren<Button>();
      service.AddListener(GameActionType.Forward, ChangeSelectedItem);
      service.AddListener(GameActionType.Backward, ChangeSelectedItem);
      service.AddListener(GameActionType.LStickDown, ChangeSelectedItem);
      service.AddListener(GameActionType.LStickUp, ChangeSelectedItem);
      service.AddListener(GameActionType.Submit, ChangeSelectedItem);
      selectedLine.enabled = false;
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.Forward, ChangeSelectedItem);
      service.RemoveListener(GameActionType.Backward, ChangeSelectedItem);
      service.RemoveListener(GameActionType.LStickDown, ChangeSelectedItem);
      service.RemoveListener(GameActionType.LStickUp, ChangeSelectedItem);
      service.RemoveListener(GameActionType.Submit, ChangeSelectedItem);
    }

    private bool ChangeSelectedItem(GameActionType type, bool down)
    {
      if (!helpPanel.activeInHierarchy)
        return false;
      if (((type == GameActionType.LStickUp ? 1 : (type == GameActionType.Forward ? 1 : 0)) & (down ? 1 : 0)) != 0)
      {
        --currentIndex;
        ChangeSelection();
        return true;
      }
      if (((type == GameActionType.LStickDown ? 1 : (type == GameActionType.Backward ? 1 : 0)) & (down ? 1 : 0)) != 0)
      {
        ++currentIndex;
        ChangeSelection();
        return true;
      }
      if (!(type == GameActionType.Submit & down))
        return false;
      ExecuteEvents.Execute<ISubmitHandler>(buttons[currentIndex].gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
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

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      helpPanel.SetActive(joystick);
      EventSystem.current.SetSelectedGameObject((GameObject) null);
      if (joystick)
      {
        if (currentIndex != 0)
          EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
        else
          EventSystem.current.SetSelectedGameObject(continueButton.gameObject.activeSelf ? continueButton.gameObject : newGameButton.gameObject);
      }
      SetSelectionLine();
    }

    private void SetSelectionLine()
    {
      GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
      Text component = (Object) selectedGameObject != (Object) null ? selectedGameObject.GetComponent<Text>() : (Text) null;
      bool flag = (Object) component != (Object) null && InputService.Instance.JoystickUsed;
      selectedLine.enabled = flag;
      if (!flag)
        return;
      Vector2 sizeDelta = selectedLine.rectTransform.sizeDelta;
      selectedLine.rectTransform.position = component.rectTransform.position;
      sizeDelta.x = component.preferredWidth;
      selectedLine.rectTransform.sizeDelta = sizeDelta;
    }

    private void UpdateContinueButton()
    {
      if (!((Object) continueButton != (Object) null))
        return;
      continueButton.gameObject.SetActive(ProfilesUtility.IsSaveExist(ServiceLocator.GetService<ProfilesService>().GetLastSaveName()));
    }
  }
}
