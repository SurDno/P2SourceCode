// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Main.StartWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
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
    private int currentIndex = 0;
    private Button[] buttons = (Button[]) null;

    public override void Initialize()
    {
      this.RegisterLayer<IStartWindow>((IStartWindow) this);
      if (!BuildSettingsUtility.IsDataExist("MarbleNest"))
        this.newMarbleNestGameButton.gameObject.SetActive(false);
      this.continueButton.onClick.AddListener(new UnityAction(this.ContinueGame));
      this.newGameButton.onClick.AddListener(new UnityAction(this.NewGame));
      this.newMarbleNestGameButton.onClick.AddListener(new UnityAction(this.NewMarbleNestGame));
      this.loadGameButton.onClick.AddListener(new UnityAction(this.OpenLoadGameWindow));
      this.settingsButton.onClick.AddListener(new UnityAction(this.OpenSettingsWindow));
      this.creditsButton.onClick.AddListener(new UnityAction(this.OpenCreditsWindow));
      this.exitButton.onClick.AddListener(new UnityAction(this.ExitGame));
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
      CoroutineService.Instance.Route(LoadGameUtility.StartNewGame(this.marbleNestGameDataName));
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
      this.exitButton.gameObject.SetActive(true);
      base.OnEnable();
      CursorService.Instance.Free = true;
      if (!InputService.Instance.JoystickUsed)
        CursorService.Instance.Visible = true;
      this.UpdateContinueButton();
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      this.currentIndex = 0;
      this.buttons = this.GetComponentsInChildren<Button>();
      service.AddListener(GameActionType.Forward, new GameActionHandle(this.ChangeSelectedItem));
      service.AddListener(GameActionType.Backward, new GameActionHandle(this.ChangeSelectedItem));
      service.AddListener(GameActionType.LStickDown, new GameActionHandle(this.ChangeSelectedItem));
      service.AddListener(GameActionType.LStickUp, new GameActionHandle(this.ChangeSelectedItem));
      service.AddListener(GameActionType.Submit, new GameActionHandle(this.ChangeSelectedItem));
      this.selectedLine.enabled = false;
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.Forward, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.Backward, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.ChangeSelectedItem));
    }

    private bool ChangeSelectedItem(GameActionType type, bool down)
    {
      if (!this.helpPanel.activeInHierarchy)
        return false;
      if (((type == GameActionType.LStickUp ? 1 : (type == GameActionType.Forward ? 1 : 0)) & (down ? 1 : 0)) != 0)
      {
        --this.currentIndex;
        this.ChangeSelection();
        return true;
      }
      if (((type == GameActionType.LStickDown ? 1 : (type == GameActionType.Backward ? 1 : 0)) & (down ? 1 : 0)) != 0)
      {
        ++this.currentIndex;
        this.ChangeSelection();
        return true;
      }
      if (!(type == GameActionType.Submit & down))
        return false;
      ExecuteEvents.Execute<ISubmitHandler>(this.buttons[this.currentIndex].gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
      return true;
    }

    private void ChangeSelection()
    {
      if (this.currentIndex > this.buttons.Length - 1)
        this.currentIndex = 0;
      if (this.currentIndex < 0)
        this.currentIndex = this.buttons.Length - 1;
      EventSystem.current.SetSelectedGameObject(this.buttons[this.currentIndex].gameObject);
      this.SetSelectionLine();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      this.helpPanel.SetActive(joystick);
      EventSystem.current.SetSelectedGameObject((GameObject) null);
      if (joystick)
      {
        if (this.currentIndex != 0)
          EventSystem.current.SetSelectedGameObject(this.buttons[this.currentIndex].gameObject);
        else
          EventSystem.current.SetSelectedGameObject(this.continueButton.gameObject.activeSelf ? this.continueButton.gameObject : this.newGameButton.gameObject);
      }
      this.SetSelectionLine();
    }

    private void SetSelectionLine()
    {
      GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
      Text component = (Object) selectedGameObject != (Object) null ? selectedGameObject.GetComponent<Text>() : (Text) null;
      bool flag = (Object) component != (Object) null && InputService.Instance.JoystickUsed;
      this.selectedLine.enabled = flag;
      if (!flag)
        return;
      Vector2 sizeDelta = this.selectedLine.rectTransform.sizeDelta;
      this.selectedLine.rectTransform.position = component.rectTransform.position;
      sizeDelta.x = component.preferredWidth;
      this.selectedLine.rectTransform.sizeDelta = sizeDelta;
    }

    private void UpdateContinueButton()
    {
      if (!((Object) this.continueButton != (Object) null))
        return;
      this.continueButton.gameObject.SetActive(ProfilesUtility.IsSaveExist(ServiceLocator.GetService<ProfilesService>().GetLastSaveName()));
    }
  }
}
