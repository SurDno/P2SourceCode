using Cofe.Utility;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
      this.RegisterLayer();
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

    public void Button_Back_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

    public void Button_Reset_Click_Handler() => this.ResetKeys();

    protected override void OnEnable()
    {
      base.OnEnable();
      this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      this.Fill();
      this.gameActionService.OnKeyPressed += new Action<KeyCode>(this.OnKeyPressed);
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      this.Clear();
      this.gameActionService.OnKeyPressed -= new Action<KeyCode>(this.OnKeyPressed);
      base.OnDisable();
    }

    public void Fill()
    {
      this.localizationService = ServiceLocator.GetService<LocalizationService>();
      this.gameActionService = ServiceLocator.GetService<GameActionService>();
      foreach (ActionGroup bind in this.gameActionService.GetBinds())
      {
        if (!bind.Hide)
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.keyItem);
          gameObject.transform.SetParent((Transform) this.keyView, false);
          KeyItem component = gameObject.GetComponent<KeyItem>();
          this.keyItems.Add(component);
          component.Bind = bind;
          component.SetText(this.GetText(bind));
          if (bind.IsChangeble)
            component.OnPressed += new Action<KeyItem>(this.OnKeyItemPressed);
          else
            component.SetReadonly();
        }
      }
    }

    public void Clear()
    {
      foreach (KeyItem keyItem in this.keyItems)
      {
        keyItem.OnPressed -= new Action<KeyItem>(this.OnKeyItemPressed);
        UnityEngine.Object.Destroy((UnityEngine.Object) keyItem.gameObject);
      }
      this.keyItems.Clear();
      this.listeningItem = (KeyItem) null;
    }

    private string GetText(ActionGroup bind)
    {
      string text = this.localizationService.GetText(InputUtility.GetTagName(bind));
      return text.IsNullOrEmpty() ? "" : text + "  [" + InputUtility.GetHotKeyNameByGroup(bind, InputService.Instance.JoystickUsed) + "]";
    }

    public void ResetKeys()
    {
      this.gameActionService.ResetAllRebinds();
      this.Clear();
      this.Fill();
    }

    private void OnKeyItemPressed(KeyItem item)
    {
      if ((UnityEngine.Object) this.listeningItem != (UnityEngine.Object) null)
        return;
      this.listeningItem = item;
      this.listeningItem.Selection = true;
    }

    private void OnKeyPressed(KeyCode code)
    {
      if (!this.IsWaiting)
        return;
      this.gameActionService.AddRebind(this.listeningItem.Bind, code);
      this.listeningItem = (KeyItem) null;
      this.Clear();
      this.Fill();
    }

    public bool IsWaiting => (UnityEngine.Object) this.listeningItem != (UnityEngine.Object) null;
  }
}
