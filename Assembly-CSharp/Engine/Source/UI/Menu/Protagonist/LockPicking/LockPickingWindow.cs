// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.LockPicking.LockPickingWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Impl.UI.Menu;
using Engine.Source.Audio;
using Engine.Source.Components;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using InputServices;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
namespace Engine.Source.UI.Menu.Protagonist.LockPicking
{
  public class LockPickingWindow : 
    UIWindow,
    ILockPickingWindow,
    IWindow,
    IPointerDownHandler,
    IEventSystemHandler
  {
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private LockPickingPin leftPin;
    [SerializeField]
    private LockPickingPin rightPin;
    [SerializeField]
    private GameObject successMessage;
    [SerializeField]
    private LockPickingSettingsData defaultSettings;
    [SerializeField]
    private float messageTime = 2f;
    [SerializeField]
    private AudioClip bumpSound;
    [SerializeField]
    private AudioClip successSound;
    [SerializeField]
    private AudioClip failureSound;
    [SerializeField]
    private AudioMixerGroup mixerGroup;
    [SerializeField]
    private ItemSelector toolSelector;
    [SerializeField]
    private SwitchingItemView2 toolSwitchingView;
    [SerializeField]
    private HideableView actionPrompts;
    private Vector2 durabilityBarSize;
    private CameraKindEnum lastCameraKind;
    private LockPickingSettings settings;
    private LockPickingWindow.State state;
    private float stateTimer;
    private bool isInteractive = false;
    [SerializeField]
    private GameObject controlPanel;

    public IStorageComponent Actor { get; set; }

    public IDoorComponent Target { get; set; }

    private bool IsInteractive
    {
      get => this.isInteractive;
      set
      {
        if (this.isInteractive == value)
          return;
        this.isInteractive = value;
        this.actionPrompts.Visible = this.isInteractive;
      }
    }

    private void SetState(LockPickingWindow.State value)
    {
      this.state = value;
      this.stateTimer = 0.0f;
      this.successMessage.SetActive(this.state == LockPickingWindow.State.Success);
      if (this.state != LockPickingWindow.State.Success)
        return;
      this.Actor.GetComponent<PlayerControllerComponent>().ComputePicklock(this.Target.Owner);
    }

    private float ToolDurability
    {
      get => this.ToolParameterValue(ParameterNameEnum.Durability, 0.0f);
      set
      {
        if (this.toolSelector.Item == null)
          return;
        ParametersComponent component = this.toolSelector.Item.GetComponent<ParametersComponent>();
        if (component == null)
          return;
        IParameter<float> byName = component.GetByName<float>(ParameterNameEnum.Durability);
        if (byName == null)
          return;
        byName.Value = value;
        if ((double) value == 0.0)
        {
          SoundUtility.PlayAudioClip2D(this.failureSound, this.mixerGroup, 1f, 0.0f);
          StorableComponentUtility.Use(this.toolSelector.Item);
        }
      }
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      this.controlPanel.SetActive(joystick);
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      service.AddListener(GameActionType.RStickRight, new GameActionHandle(this.NextLockPick));
      service.AddListener(GameActionType.RStickLeft, new GameActionHandle(this.PreviousLockPick));
      service.AddListener(GameActionType.LTrigger, new GameActionHandle(this.BumpLeft), true);
      service.AddListener(GameActionType.RTrigger, new GameActionHandle(this.BumpRight), true);
      this.Clear();
      if (this.Actor == null || this.Target == null)
        return;
      this.toolSelector.Storage = this.Actor;
      this.settings = this.defaultSettings.Settings;
      UnityEngine.Random.State state = UnityEngine.Random.state;
      UnityEngine.Random.InitState(this.Target.Owner.Id.GetHashCode());
      this.leftPin.Build(this.settings);
      this.rightPin.Build(this.settings);
      UnityEngine.Random.state = state;
      this.SetState(LockPickingWindow.State.Idle);
    }

    private bool BumpRight(GameActionType type, bool down)
    {
      if (!this.IsInteractive || !down)
        return false;
      float toolDurability = this.ToolDurability;
      float durability = toolDurability;
      float quality = this.ToolParameterValue(ParameterNameEnum.Quality, 1f);
      this.Bump(this.rightPin, ref durability, quality);
      if ((double) durability != (double) toolDurability)
      {
        if ((double) durability < 0.0)
          durability = 0.0f;
        this.ToolDurability = durability;
      }
      return true;
    }

    private bool BumpLeft(GameActionType type, bool down)
    {
      if (!this.IsInteractive || !down)
        return false;
      float toolDurability = this.ToolDurability;
      float durability = toolDurability;
      float quality = this.ToolParameterValue(ParameterNameEnum.Quality, 1f);
      this.Bump(this.leftPin, ref durability, quality);
      if ((double) durability != (double) toolDurability)
      {
        if ((double) durability < 0.0)
          durability = 0.0f;
        this.ToolDurability = durability;
      }
      return true;
    }

    private bool PreviousLockPick(GameActionType type, bool down)
    {
      if (!down)
        return false;
      ExecuteEvents.Execute<ISubmitHandler>(this.toolSelector.previousButton.gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
      return true;
    }

    private bool NextLockPick(GameActionType type, bool down)
    {
      if (!down)
        return false;
      ExecuteEvents.Execute<ISubmitHandler>(this.toolSelector.nextButton.gameObject, (BaseEventData) new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
      return true;
    }

    protected override void OnDisable()
    {
      this.Clear();
      ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      service.RemoveListener(GameActionType.RStickRight, new GameActionHandle(this.NextLockPick));
      service.RemoveListener(GameActionType.RStickLeft, new GameActionHandle(this.PreviousLockPick));
      service.RemoveListener(GameActionType.LTrigger, new GameActionHandle(this.BumpLeft));
      service.RemoveListener(GameActionType.RTrigger, new GameActionHandle(this.BumpRight));
      base.OnDisable();
    }

    public override void Initialize()
    {
      this.RegisterLayer<ILockPickingWindow>((ILockPickingWindow) this);
      this.closeButton.onClick.AddListener(new UnityAction(this.CloseWindow));
      this.toolSelector.ChangeItemEvent += new Action<ItemSelector, IStorableComponent, IStorableComponent>(this.OnItemChange);
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (ILockPickingWindow);

    public void CloseWindow() => ServiceLocator.GetService<UIService>().Pop();

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
      if (!this.IsInteractive)
        return;
      float toolDurability = this.ToolDurability;
      float durability = toolDurability;
      float quality = this.ToolParameterValue(ParameterNameEnum.Quality, 1f);
      if (eventData.button == PointerEventData.InputButton.Left)
        this.Bump(this.leftPin, ref durability, quality);
      else if (eventData.button == PointerEventData.InputButton.Right)
        this.Bump(this.rightPin, ref durability, quality);
      if ((double) durability == (double) toolDurability)
        return;
      if ((double) durability < 0.0)
        durability = 0.0f;
      this.ToolDurability = durability;
    }

    private void Bump(LockPickingPin pin, ref float durability, float quality)
    {
      if ((double) quality <= 0.0)
        durability = 0.0f;
      else
        durability -= this.settings.DurabilityCost / quality;
      SoundUtility.PlayAudioClip2D(this.bumpSound, this.mixerGroup, 1f, 0.0f);
      pin.Bump();
    }

    private void Clear()
    {
      this.leftPin.Clear();
      this.rightPin.Clear();
      this.toolSelector.Storage = (IStorageComponent) null;
    }

    private void OnItemChange(
      ItemSelector itemSelector,
      IStorableComponent prevItem,
      IStorableComponent newItem)
    {
      if (prevItem != null)
        StorableComponentUtility.PlayPutSound(prevItem);
      if (newItem == null)
        return;
      StorableComponentUtility.PlayTakeSound(newItem);
    }

    private float ToolParameterValue(ParameterNameEnum name, float defaultValue)
    {
      if (this.toolSelector.Item == null)
        return defaultValue;
      ParametersComponent component = this.toolSelector.Item.GetComponent<ParametersComponent>();
      if (component == null)
        return defaultValue;
      IParameter<float> byName = component.GetByName<float>(name);
      return byName == null ? defaultValue : byName.Value;
    }

    private void Update()
    {
      float toolDurability = this.ToolDurability;
      if (this.state != LockPickingWindow.State.Success && this.leftPin.InSweetSpot && this.rightPin.InSweetSpot)
      {
        SoundUtility.PlayAudioClip2D(this.successSound, this.mixerGroup, 1f, 0.0f);
        this.leftPin.Locked = true;
        this.rightPin.Locked = true;
        this.Target.LockState.Value = LockState.Unlocked;
        this.SetState(LockPickingWindow.State.Success);
      }
      this.IsInteractive = !this.toolSwitchingView.IsAnimated && this.toolSelector.Item != null && this.state == LockPickingWindow.State.Idle;
      if (this.state != LockPickingWindow.State.Success)
        return;
      this.stateTimer += Time.deltaTime;
      if ((double) this.stateTimer < (double) this.messageTime)
        return;
      this.CloseWindow();
    }

    private enum State
    {
      None,
      Idle,
      Fail,
      Success,
      BreakTool,
    }
  }
}
