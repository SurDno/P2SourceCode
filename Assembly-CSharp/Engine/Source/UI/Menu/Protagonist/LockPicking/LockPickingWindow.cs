using System;
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
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    private State state;
    private float stateTimer;
    private bool isInteractive;
    [SerializeField]
    private GameObject controlPanel;

    public IStorageComponent Actor { get; set; }

    public IDoorComponent Target { get; set; }

    private bool IsInteractive
    {
      get => isInteractive;
      set
      {
        if (isInteractive == value)
          return;
        isInteractive = value;
        actionPrompts.Visible = isInteractive;
      }
    }

    private void SetState(State value)
    {
      state = value;
      stateTimer = 0.0f;
      successMessage.SetActive(state == State.Success);
      if (state != State.Success)
        return;
      Actor.GetComponent<PlayerControllerComponent>().ComputePicklock(Target.Owner);
    }

    private float ToolDurability
    {
      get => ToolParameterValue(ParameterNameEnum.Durability, 0.0f);
      set
      {
        if (toolSelector.Item == null)
          return;
        ParametersComponent component = toolSelector.Item.GetComponent<ParametersComponent>();
        if (component == null)
          return;
        IParameter<float> byName = component.GetByName<float>(ParameterNameEnum.Durability);
        if (byName == null)
          return;
        byName.Value = value;
        if (value == 0.0)
        {
          SoundUtility.PlayAudioClip2D(failureSound, mixerGroup, 1f, 0.0f);
          StorableComponentUtility.Use(toolSelector.Item);
        }
      }
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      controlPanel.SetActive(joystick);
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      service.AddListener(GameActionType.RStickRight, NextLockPick);
      service.AddListener(GameActionType.RStickLeft, PreviousLockPick);
      service.AddListener(GameActionType.LTrigger, BumpLeft, true);
      service.AddListener(GameActionType.RTrigger, BumpRight, true);
      Clear();
      if (Actor == null || Target == null)
        return;
      toolSelector.Storage = Actor;
      settings = defaultSettings.Settings;
      Random.State state = Random.state;
      Random.InitState(Target.Owner.Id.GetHashCode());
      leftPin.Build(settings);
      rightPin.Build(settings);
      Random.state = state;
      SetState(State.Idle);
    }

    private bool BumpRight(GameActionType type, bool down)
    {
      if (!IsInteractive || !down)
        return false;
      float toolDurability = ToolDurability;
      float durability = toolDurability;
      float quality = ToolParameterValue(ParameterNameEnum.Quality, 1f);
      Bump(rightPin, ref durability, quality);
      if (durability != (double) toolDurability)
      {
        if (durability < 0.0)
          durability = 0.0f;
        ToolDurability = durability;
      }
      return true;
    }

    private bool BumpLeft(GameActionType type, bool down)
    {
      if (!IsInteractive || !down)
        return false;
      float toolDurability = ToolDurability;
      float durability = toolDurability;
      float quality = ToolParameterValue(ParameterNameEnum.Quality, 1f);
      Bump(leftPin, ref durability, quality);
      if (durability != (double) toolDurability)
      {
        if (durability < 0.0)
          durability = 0.0f;
        ToolDurability = durability;
      }
      return true;
    }

    private bool PreviousLockPick(GameActionType type, bool down)
    {
      if (!down)
        return false;
      ExecuteEvents.Execute(toolSelector.previousButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
      return true;
    }

    private bool NextLockPick(GameActionType type, bool down)
    {
      if (!down)
        return false;
      ExecuteEvents.Execute(toolSelector.nextButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
      return true;
    }

    protected override void OnDisable()
    {
      Clear();
      ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      service.RemoveListener(GameActionType.RStickRight, NextLockPick);
      service.RemoveListener(GameActionType.RStickLeft, PreviousLockPick);
      service.RemoveListener(GameActionType.LTrigger, BumpLeft);
      service.RemoveListener(GameActionType.RTrigger, BumpRight);
      base.OnDisable();
    }

    public override void Initialize()
    {
      RegisterLayer<ILockPickingWindow>(this);
      closeButton.onClick.AddListener(CloseWindow);
      toolSelector.ChangeItemEvent += OnItemChange;
      base.Initialize();
    }

    public override Type GetWindowType() => typeof (ILockPickingWindow);

    public void CloseWindow() => ServiceLocator.GetService<UIService>().Pop();

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
      if (!IsInteractive)
        return;
      float toolDurability = ToolDurability;
      float durability = toolDurability;
      float quality = ToolParameterValue(ParameterNameEnum.Quality, 1f);
      if (eventData.button == PointerEventData.InputButton.Left)
        Bump(leftPin, ref durability, quality);
      else if (eventData.button == PointerEventData.InputButton.Right)
        Bump(rightPin, ref durability, quality);
      if (durability == (double) toolDurability)
        return;
      if (durability < 0.0)
        durability = 0.0f;
      ToolDurability = durability;
    }

    private void Bump(LockPickingPin pin, ref float durability, float quality)
    {
      if (quality <= 0.0)
        durability = 0.0f;
      else
        durability -= settings.DurabilityCost / quality;
      SoundUtility.PlayAudioClip2D(bumpSound, mixerGroup, 1f, 0.0f);
      pin.Bump();
    }

    private void Clear()
    {
      leftPin.Clear();
      rightPin.Clear();
      toolSelector.Storage = null;
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
      if (toolSelector.Item == null)
        return defaultValue;
      ParametersComponent component = toolSelector.Item.GetComponent<ParametersComponent>();
      if (component == null)
        return defaultValue;
      IParameter<float> byName = component.GetByName<float>(name);
      return byName == null ? defaultValue : byName.Value;
    }

    private void Update()
    {
      float toolDurability = ToolDurability;
      if (state != State.Success && leftPin.InSweetSpot && rightPin.InSweetSpot)
      {
        SoundUtility.PlayAudioClip2D(successSound, mixerGroup, 1f, 0.0f);
        leftPin.Locked = true;
        rightPin.Locked = true;
        Target.LockState.Value = LockState.Unlocked;
        SetState(State.Success);
      }
      IsInteractive = !toolSwitchingView.IsAnimated && toolSelector.Item != null && state == State.Idle;
      if (state != State.Success)
        return;
      stateTimer += Time.deltaTime;
      if (stateTimer < (double) messageTime)
        return;
      CloseWindow();
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
