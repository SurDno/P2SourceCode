// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Main.GameWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using Engine.Source.Utility;
using InputServices;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
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
    private int currentIndex = 0;
    private Button[] buttons = (Button[]) null;
    private bool isReturnedFromGame = true;

    public GameObject Menu => this.menu;

    public override void Initialize()
    {
      this.RegisterLayer<IGameWindow>((IGameWindow) this);
      Button[] componentsInChildren = this.GetComponentsInChildren<Button>(true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((UnityAction<BaseEventData>) (eventData => this.Button_Click_Handler()));
        componentsInChildren[index].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
      }
      base.Initialize();
    }

    public void Button_Click_Handler()
    {
      if (!this.gameObject.activeInHierarchy)
        return;
      this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
    }

    public void Button_BackToGame_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

    public void Button_Settings_Click_Handler()
    {
      SettingsMenuHelper.Instatnce.Reset();
      ServiceLocator.GetService<UIService>().Push<IGameLanguageSettingsWindow>();
    }

    public void Button_ExitGame_Click_Handler() => this.ShowExitConfirmation(new Action(this.Exit));

    public void Button_NewGame_Click_Handler()
    {
      this.ShowExitConfirmation(new Action(this.ToMainMenu));
    }

    public void Button_LoadGame_Click_Handler()
    {
      ServiceLocator.GetService<UIService>().Push<IGameLoadGameWindow>();
    }

    protected override void OnEnable()
    {
      this.buttonExit.gameObject.SetActive(true);
      base.OnEnable();
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      PlayerUtility.ShowPlayerHands(false);
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      if (this.buttons == null)
        this.buttons = this.GetComponentsInChildren<Button>();
      service.AddListener(GameActionType.LStickDown, new GameActionHandle(this.ChangeSelectedItem));
      service.AddListener(GameActionType.LStickUp, new GameActionHandle(this.ChangeSelectedItem));
      service.AddListener(GameActionType.Submit, new GameActionHandle(this.ChangeSelectedItem));
      service.AddListener(GameActionType.Cancel, new GameActionHandle(this.BackButton));
      service.AddListener(GameActionType.MainMenu, new GameActionHandle(this.BackButton));
      this.selectedLine.enabled = false;
    }

    protected override void OnDisable()
    {
      if ((UnityEngine.Object) this.exitConfirmationInstance != (UnityEngine.Object) null)
        this.exitConfirmationInstance.Hide();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      CursorService.Instance.Free = CursorService.Instance.Visible = false;
      PlayerUtility.ShowPlayerHands(true);
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.ChangeSelectedItem));
      service.RemoveListener(GameActionType.Cancel, new GameActionHandle(this.BackButton));
      service.RemoveListener(GameActionType.MainMenu, new GameActionHandle(this.BackButton));
      base.OnDisable();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      this.helpPanel.SetActive(joystick);
      EventSystem.current.SetSelectedGameObject((GameObject) null);
      if (joystick)
      {
        if (this.isReturnedFromGame)
        {
          EventSystem.current.SetSelectedGameObject(this.buttons[0].gameObject);
          this.currentIndex = 0;
        }
        else
          EventSystem.current.SetSelectedGameObject(this.buttons[this.currentIndex].gameObject);
      }
      this.SetSelectionLine();
    }

    private bool BackButton(GameActionType type, bool down)
    {
      if (!down)
        return false;
      this.Button_BackToGame_Click_Handler();
      return true;
    }

    private bool ChangeSelectedItem(GameActionType type, bool down)
    {
      if (!this.helpPanel.activeInHierarchy || (UnityEngine.Object) this.exitConfirmationInstance != (UnityEngine.Object) null && this.exitConfirmationInstance.gameObject.activeInHierarchy)
        return false;
      if (type == GameActionType.LStickUp & down)
      {
        --this.currentIndex;
        this.ChangeSelection();
        return true;
      }
      if (type == GameActionType.LStickDown & down)
      {
        ++this.currentIndex;
        this.ChangeSelection();
        return true;
      }
      if (type == GameActionType.Submit & down)
      {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        this.isReturnedFromGame = this.currentIndex == 0;
        ExecuteEvents.Execute<ISubmitHandler>(this.buttons[this.currentIndex].gameObject, (BaseEventData) eventData, ExecuteEvents.submitHandler);
        return true;
      }
      if (!(type == GameActionType.Cancel & down))
        return false;
      this.isReturnedFromGame = true;
      this.currentIndex = 0;
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

    private void SetSelectionLine()
    {
      GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
      Text component = (UnityEngine.Object) selectedGameObject != (UnityEngine.Object) null ? selectedGameObject.GetComponent<Text>() : (Text) null;
      bool flag = (UnityEngine.Object) component != (UnityEngine.Object) null && InputService.Instance.JoystickUsed;
      this.selectedLine.enabled = flag;
      if (!flag)
        return;
      Vector2 sizeDelta = this.selectedLine.rectTransform.sizeDelta;
      this.selectedLine.rectTransform.position = component.rectTransform.position;
      sizeDelta.x = component.preferredWidth;
      this.selectedLine.rectTransform.sizeDelta = sizeDelta;
    }

    private void Exit() => InstanceByRequest<EngineApplication>.Instance.Exit();

    private void ToMainMenu() => ServiceLocator.GetService<GameLauncher>().ExitToMainMenu();

    private void ShowExitConfirmation(Action onAccept)
    {
      if ((UnityEngine.Object) this.exitConfirmationInstance == (UnityEngine.Object) null)
        this.exitConfirmationInstance = UnityEngine.Object.Instantiate<ConfirmationWindow>(this.exitConfirmationPrefab, this.transform, false);
      this.exitConfirmationInstance.Show("{UI.Menu.Main.Game.ExitConfirmation}", onAccept, (Action) null);
    }
  }
}
