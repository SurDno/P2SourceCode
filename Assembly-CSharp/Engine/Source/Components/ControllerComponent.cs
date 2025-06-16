using System;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Impl.Services.Simulations;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components.Interactable;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using Engine.Source.Utility;
using InputServices;
using Inspectors;

namespace Engine.Source.Components
{
  [Required(typeof (PlayerLocationComponent))]
  [Required(typeof (PlayerInteractableComponent))]
  [Required(typeof (AttackerPlayerComponent))]
  [Factory(typeof (IControllerComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ControllerComponent : 
    EngineComponent,
    IControllerComponent,
    IComponent,
    IUpdatable,
    IPlayerActivated
  {
    [FromThis]
    private AttackerPlayerComponent attackerPlayer;
    [FromThis]
    private ParametersComponent parameters;
    [FromLocator]
    private InterfaceBlockingService interfaceBlockingService;
    private bool added;
    private bool forward;
    private bool backward;
    private bool left;
    private bool right;
    private float forwardValue;
    private float backwardValue;
    private float leftValue;
    private float rightValue;
    private IParameter<float> stamina;

    public event Action<IEntity, IInteractableComponent, IInteractItem> BeginInteractEvent;

    public event Action<IEntity, IInteractableComponent, IInteractItem> EndInteractEvent;

    public event Action<WeaponKind, bool> OnWeaponEnableChanged;

    [Inspected]
    public IParameterValue<bool> IsFighting { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsStelth { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsRun { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsWalk { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsFlashlight { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<float> WalkModifier { get; } = new ParameterValue<float>();

    [Inspected]
    public IParameterValue<bool> WalkBlock { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<float> RunModifier { get; } = new ParameterValue<float>();

    [Inspected]
    public bool IsJump { get; set; }

    [Inspected]
    public bool IsForward => ForwardValue != 0.0;

    [Inspected]
    public bool IsBackward => BackwardValue != 0.0;

    [Inspected]
    public bool IsLeft => LeftValue != 0.0;

    [Inspected]
    public bool IsRight => RightValue != 0.0;

    [Inspected]
    public float ForwardValue => forward ? 1f : forwardValue;

    [Inspected]
    public float BackwardValue => backward ? 1f : backwardValue;

    [Inspected]
    public float LeftValue => left ? 1f : leftValue;

    [Inspected]
    public float RightValue => right ? 1f : rightValue;

    [Inspected]
    public Vector3 PushVelocity { get; set; }

    public override void OnAdded()
    {
      base.OnAdded();
      IsFighting.Set(parameters.GetByName<bool>(ParameterNameEnum.IsFighting));
      IsStelth.Set(parameters.GetByName<bool>(ParameterNameEnum.Stealth));
      IsRun.Set(parameters.GetByName<bool>(ParameterNameEnum.Run));
      IsWalk.Set(parameters.GetByName<bool>(ParameterNameEnum.Walk));
      IsFlashlight.Set(parameters.GetByName<bool>(ParameterNameEnum.Flashlight));
      WalkModifier.Set(parameters.GetByName<float>(ParameterNameEnum.WalkSpeedModifier));
      WalkBlock.Set(parameters.GetByName<bool>(ParameterNameEnum.MovementControlBlock));
      RunModifier.Set(parameters.GetByName<float>(ParameterNameEnum.RunSpeedModifier));
      UpdatePlayer();
    }

    public override void OnRemoved()
    {
      RemovePlayer();
      IsFighting.Set(null);
      IsStelth.Set(null);
      IsRun.Set(null);
      IsWalk.Set(null);
      IsFlashlight.Set(null);
      WalkModifier.Set(null);
      WalkBlock.Set(null);
      RunModifier.Set(null);
      base.OnRemoved();
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      UpdatePlayer();
    }

    public void FireBeginInteract(InteractableComponent interactable, InteractItem item)
    {
      Action<IEntity, IInteractableComponent, IInteractItem> beginInteractEvent = BeginInteractEvent;
      if (beginInteractEvent == null)
        return;
      beginInteractEvent(Owner, interactable, item);
    }

    public void FireEndInteract(InteractableComponent interactable, InteractItem item)
    {
      Action<IEntity, IInteractableComponent, IInteractItem> endInteractEvent = EndInteractEvent;
      if (endInteractEvent == null)
        return;
      endInteractEvent(Owner, interactable, item);
    }

    private void AttackerPlayer_HandsHolsteredEvent(WeaponKind weapon)
    {
      Action<WeaponKind, bool> weaponEnableChanged = OnWeaponEnableChanged;
      if (weaponEnableChanged == null)
        return;
      weaponEnableChanged(weapon, false);
    }

    private void AttackerPlayer_HandsUnholsteredEvent(WeaponKind weapon)
    {
      Action<WeaponKind, bool> weaponEnableChanged = OnWeaponEnableChanged;
      if (weaponEnableChanged == null)
        return;
      weaponEnableChanged(weapon, true);
    }

    private bool GenericPlayerMenuListener(GameActionType type, bool down)
    {
      if (!(type == GameActionType.GenericPlayerMenu & down) || !PlayerUtility.IsPlayerCanControlling)
        return false;
      return SimplePlayerWindowSwapper.LastOpenedPlayerWindowType == typeof (IInventoryWindow) ? InventoryListener(type, down) : SimplePlayerWindowSwapper.CallLastPlayerWindow();
    }

    private bool BoundCharactersListener(GameActionType type, bool down)
    {
      if (!down || InputService.Instance.JoystickUsed && JoystickLayoutSwitcher.Instance.CurrentLayout != 0 || !PlayerUtility.IsPlayerCanControlling || ServiceLocator.GetService<InterfaceBlockingService>().BlockBoundsInterface)
        return false;
      ServiceLocator.GetService<UIService>().Push<IBoundCharactersWindow>();
      return true;
    }

    private bool InventoryListener(GameActionType type, bool down)
    {
      if (!down || InputService.Instance.JoystickUsed && type == GameActionType.Inventory && JoystickLayoutSwitcher.Instance.CurrentLayout != 0 || !PlayerUtility.IsPlayerCanControlling || ServiceLocator.GetService<InterfaceBlockingService>().BlockInventoryInterface)
        return false;
      IStorageComponent component = Owner.GetComponent<IStorageComponent>();
      if (component == null)
        return false;
      ServiceLocator.GetService<UIService>().Get<IInventoryWindow>().Actor = component;
      ServiceLocator.GetService<UIService>().Push<IInventoryWindow>();
      return true;
    }

    private bool MindMapListener(GameActionType type, bool down)
    {
      if (!down || InputService.Instance.JoystickUsed && JoystickLayoutSwitcher.Instance.CurrentLayout != 0 || !PlayerUtility.IsPlayerCanControlling || ServiceLocator.GetService<InterfaceBlockingService>().BlockMindMapInterface)
        return false;
      ServiceLocator.GetService<UIService>().Push<IMMWindow>();
      return true;
    }

    private bool MapListener(GameActionType type, bool down)
    {
      if (!down || InputService.Instance.JoystickUsed && JoystickLayoutSwitcher.Instance.CurrentLayout != 0 || !PlayerUtility.IsPlayerCanControlling || ServiceLocator.GetService<InterfaceBlockingService>().BlockMapInterface)
        return false;
      ServiceLocator.GetService<UIService>().Push<IMapWindow>();
      return true;
    }

    private bool UnholsterListener(GameActionType type, bool down)
    {
      if (InputService.Instance.JoystickUsed || !down || !PlayerUtility.IsPlayerCanControlling || attackerPlayer == null)
        return false;
      if (attackerPlayer.IsUnholstered)
        attackerPlayer.HandsHolster();
      else
        attackerPlayer.HandsUnholster();
      return true;
    }

    private bool NextWeaponListener(GameActionType type, bool down)
    {
      if (!down || !PlayerUtility.IsPlayerCanControlling || attackerPlayer == null)
        return false;
      attackerPlayer.NextWeapon();
      return true;
    }

    private bool PrevWeaponListener(GameActionType type, bool down)
    {
      if (!down || !PlayerUtility.IsPlayerCanControlling || attackerPlayer == null)
        return false;
      attackerPlayer.PrevWeapon();
      return true;
    }

    private bool StealthListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling)
          return false;
        IsStelth.Value = true;
        IsRun.Value = false;
        return true;
      }
      IsStelth.Value = false;
      return false;
    }

    private bool CompassListener(GameActionType type, bool down)
    {
      bool flag = down;
      if (!PlayerUtility.IsPlayerCanControlling)
        flag = false;
      if (ServiceLocator.GetService<QuestCompassService>().IsEnabled == flag)
        return false;
      ServiceLocator.GetService<QuestCompassService>().IsEnabled = flag;
      return true;
    }

    private bool RunListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling || IsStelth.Value)
          return false;
        if (JoystickLayoutSwitcher.Instance.CurrentLayout != JoystickLayoutSwitcher.KeyLayouts.Three && IsRun.Value)
        {
          IsRun.Value = false;
          return false;
        }
        IsRun.Value = true;
        return true;
      }
      if (!InputService.Instance.JoystickUsed || JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.Three)
        IsRun.Value = false;
      return false;
    }

    private bool JumpListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling || IsStelth.Value)
          return false;
        IsJump = true;
        return true;
      }
      IsJump = false;
      return false;
    }

    private bool RightListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling)
          return false;
        right = true;
        UpdateWalk();
        return true;
      }
      right = false;
      UpdateWalk();
      return false;
    }

    private bool LeftListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling)
          return false;
        left = true;
        UpdateWalk();
        return true;
      }
      left = false;
      UpdateWalk();
      return false;
    }

    private bool BackwardListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling)
          return false;
        backward = true;
        UpdateWalk();
        return true;
      }
      backward = false;
      UpdateWalk();
      return false;
    }

    private bool ForwardListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling)
          return false;
        forward = true;
        UpdateWalk();
        return true;
      }
      forward = false;
      UpdateWalk();
      return false;
    }

    private void UpdateWalk()
    {
      IsWalk.Value = IsForward || IsBackward || IsRight || IsLeft;
      if (IsWalk.Value || !IsRun.Value)
        return;
      IsRun.Value = false;
    }

    private void UpdatePlayer()
    {
      if (((Entity) Owner).IsAdded && Owner.IsEnabledInHierarchy)
        AddPlayer();
      else
        RemovePlayer();
    }

    private void AddPlayer()
    {
      if (added)
        return;
      added = true;
      ServiceLocator.GetService<Simulation>().AddPlayer(Owner);
    }

    private void RemovePlayer()
    {
      if (!added)
        return;
      added = false;
      ServiceLocator.GetService<Simulation>().RemovePlayer(Owner);
    }

    private void Reset()
    {
      IsStelth.Value = false;
      IsRun.Value = false;
      IsJump = false;
      left = false;
      right = false;
      forward = false;
      backward = false;
      leftValue = 0.0f;
      rightValue = 0.0f;
      forwardValue = 0.0f;
      backwardValue = 0.0f;
      UpdateWalk();
      ServiceLocator.GetService<QuestCompassService>().IsEnabled = false;
    }

    public void ComputeUpdate()
    {
      leftValue = 0.0f;
      rightValue = 0.0f;
      forwardValue = 0.0f;
      backwardValue = 0.0f;
      if (!PlayerUtility.IsPlayerCanControlling)
        return;
      float axis1 = InputService.Instance.GetAxis("LeftStickX");
      if (axis1 > 0.0)
        rightValue = axis1;
      if (axis1 < 0.0)
        leftValue = -axis1;
      float axis2 = InputService.Instance.GetAxis("LeftStickY");
      if (axis2 > 0.0)
        backwardValue = axis2;
      if (axis2 < 0.0)
        forwardValue = -axis2;
      if (stamina != null && stamina.Value == 0.0 && IsRun.Value)
        IsRun.Value = false;
      UpdateWalk();
    }

    public void PlayerActivated()
    {
      if (attackerPlayer != null)
      {
        attackerPlayer.WeaponHolsterStartEvent += AttackerPlayer_HandsHolsteredEvent;
        attackerPlayer.WeaponUnholsterEndEvent += AttackerPlayer_HandsUnholsteredEvent;
      }
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
      AddListeners();
      stamina = Owner.GetComponent<ParametersComponent>().GetByName<float>(ParameterNameEnum.Stamina);
    }

    public void AddListeners()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.BoundCharacters, BoundCharactersListener);
      service.AddListener(GameActionType.Inventory, InventoryListener);
      service.AddListener(GameActionType.MindMap, MindMapListener);
      service.AddListener(GameActionType.Map, MapListener);
      service.AddListener(GameActionType.GenericPlayerMenu, GenericPlayerMenuListener);
      service.AddListener(GameActionType.Unholster, UnholsterListener);
      service.AddListener(GameActionType.Compass, CompassListener);
      service.AddListener(GameActionType.NextWeapon, NextWeaponListener);
      service.AddListener(GameActionType.PrevWeapon, PrevWeaponListener);
      service.AddListener(GameActionType.Stealth, StealthListener);
      service.AddListener(GameActionType.Run, RunListener);
      service.AddListener(GameActionType.Jump, JumpListener);
      service.AddListener(GameActionType.Forward, ForwardListener);
      service.AddListener(GameActionType.Backward, BackwardListener);
      service.AddListener(GameActionType.Left, LeftListener);
      service.AddListener(GameActionType.Right, RightListener);
    }

    public void RemoveListeners()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.BoundCharacters, BoundCharactersListener);
      service.RemoveListener(GameActionType.Inventory, InventoryListener);
      service.RemoveListener(GameActionType.MindMap, MindMapListener);
      service.RemoveListener(GameActionType.Map, MapListener);
      service.RemoveListener(GameActionType.GenericPlayerMenu, GenericPlayerMenuListener);
      service.RemoveListener(GameActionType.Unholster, UnholsterListener);
      service.RemoveListener(GameActionType.Compass, CompassListener);
      service.RemoveListener(GameActionType.NextWeapon, NextWeaponListener);
      service.RemoveListener(GameActionType.PrevWeapon, PrevWeaponListener);
      service.RemoveListener(GameActionType.Stealth, StealthListener);
      service.RemoveListener(GameActionType.Run, RunListener);
      service.RemoveListener(GameActionType.Jump, JumpListener);
      service.RemoveListener(GameActionType.Forward, ForwardListener);
      service.RemoveListener(GameActionType.Backward, BackwardListener);
      service.RemoveListener(GameActionType.Left, LeftListener);
      service.RemoveListener(GameActionType.Right, RightListener);
      Reset();
      ServiceLocator.GetService<QuestCompassService>().IsEnabled = false;
    }

    public void PlayerDeactivated()
    {
      RemoveListeners();
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
      if (attackerPlayer == null)
        return;
      attackerPlayer.WeaponUnholsterEndEvent -= AttackerPlayer_HandsUnholsteredEvent;
      attackerPlayer.WeaponHolsterStartEvent -= AttackerPlayer_HandsHolsteredEvent;
    }
  }
}
