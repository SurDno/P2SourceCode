using System.Collections.Generic;
using Cofe.Utility;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;

namespace Engine.Impl.UI.Menu.Main
{
  public abstract class KeySettingsWindow : UIWindow
  {
    private CameraKindEnum lastCameraKind;
    [Header("Sounds")]
    [SerializeField]
    [FormerlySerializedAs("ClickSound")]
    private AudioClip clickSound;
    [SerializeField]
    private GameObject keyItem;
    [SerializeField]
    private RectTransform keyView;
    private KeyItem listeningItem;
    private List<KeyItem> keyItems = new List<KeyItem>();
    private LocalizationService localizationService;
    private GameActionService gameActionService;

    protected abstract void RegisterLayer();

    public override void Initialize()
    {
      RegisterLayer();
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

    public void Button_Back_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

    public void Button_Reset_Click_Handler() => ResetKeys();

    protected override void OnEnable()
    {
      base.OnEnable();
      lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      Fill();
      gameActionService.OnKeyPressed += OnKeyPressed;
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      Clear();
      gameActionService.OnKeyPressed -= OnKeyPressed;
      base.OnDisable();
    }

    public void Fill()
    {
      localizationService = ServiceLocator.GetService<LocalizationService>();
      gameActionService = ServiceLocator.GetService<GameActionService>();
      foreach (ActionGroup bind in gameActionService.GetBinds())
      {
        if (!bind.Hide)
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(keyItem);
          gameObject.transform.SetParent((Transform) keyView, false);
          KeyItem component = gameObject.GetComponent<KeyItem>();
          keyItems.Add(component);
          component.Bind = bind;
          component.SetText(GetText(bind));
          if (bind.IsChangeble)
            component.OnPressed += OnKeyItemPressed;
          else
            component.SetReadonly();
        }
      }
    }

    public void Clear()
    {
      foreach (KeyItem keyItem in keyItems)
      {
        keyItem.OnPressed -= OnKeyItemPressed;
        UnityEngine.Object.Destroy((UnityEngine.Object) keyItem.gameObject);
      }
      keyItems.Clear();
      listeningItem = null;
    }

    private string GetText(ActionGroup bind)
    {
      string text = localizationService.GetText(InputUtility.GetTagName(bind));
      return text.IsNullOrEmpty() ? "" : text + "  [" + InputUtility.GetHotKeyNameByGroup(bind, InputService.Instance.JoystickUsed) + "]";
    }

    public void ResetKeys()
    {
      gameActionService.ResetAllRebinds();
      Clear();
      Fill();
    }

    private void OnKeyItemPressed(KeyItem item)
    {
      if ((UnityEngine.Object) listeningItem != (UnityEngine.Object) null)
        return;
      listeningItem = item;
      listeningItem.Selection = true;
    }

    private void OnKeyPressed(KeyCode code)
    {
      if (!IsWaiting)
        return;
      gameActionService.AddRebind(listeningItem.Bind, code);
      listeningItem = null;
      Clear();
      Fill();
    }

    public bool IsWaiting => (UnityEngine.Object) listeningItem != (UnityEngine.Object) null;
  }
}
