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
using System;
using UnityEngine;

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
    public IParameterValue<bool> IsFighting { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsStelth { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsRun { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsWalk { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsFlashlight { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<float> WalkModifier { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<bool> WalkBlock { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<float> RunModifier { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public bool IsJump { get; set; }

    [Inspected]
    public bool IsForward => (double) this.ForwardValue != 0.0;

    [Inspected]
    public bool IsBackward => (double) this.BackwardValue != 0.0;

    [Inspected]
    public bool IsLeft => (double) this.LeftValue != 0.0;

    [Inspected]
    public bool IsRight => (double) this.RightValue != 0.0;

    [Inspected]
    public float ForwardValue => this.forward ? 1f : this.forwardValue;

    [Inspected]
    public float BackwardValue => this.backward ? 1f : this.backwardValue;

    [Inspected]
    public float LeftValue => this.left ? 1f : this.leftValue;

    [Inspected]
    public float RightValue => this.right ? 1f : this.rightValue;

    [Inspected]
    public Vector3 PushVelocity { get; set; }

    public override void OnAdded()
    {
      base.OnAdded();
      this.IsFighting.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.IsFighting));
      this.IsStelth.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Stealth));
      this.IsRun.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Run));
      this.IsWalk.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Walk));
      this.IsFlashlight.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Flashlight));
      this.WalkModifier.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.WalkSpeedModifier));
      this.WalkBlock.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.MovementControlBlock));
      this.RunModifier.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.RunSpeedModifier));
      this.UpdatePlayer();
    }

    public override void OnRemoved()
    {
      this.RemovePlayer();
      this.IsFighting.Set<bool>((IParameter<bool>) null);
      this.IsStelth.Set<bool>((IParameter<bool>) null);
      this.IsRun.Set<bool>((IParameter<bool>) null);
      this.IsWalk.Set<bool>((IParameter<bool>) null);
      this.IsFlashlight.Set<bool>((IParameter<bool>) null);
      this.WalkModifier.Set<float>((IParameter<float>) null);
      this.WalkBlock.Set<bool>((IParameter<bool>) null);
      this.RunModifier.Set<float>((IParameter<float>) null);
      base.OnRemoved();
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      this.UpdatePlayer();
    }

    public void FireBeginInteract(InteractableComponent interactable, InteractItem item)
    {
      Action<IEntity, IInteractableComponent, IInteractItem> beginInteractEvent = this.BeginInteractEvent;
      if (beginInteractEvent == null)
        return;
      beginInteractEvent(this.Owner, (IInteractableComponent) interactable, (IInteractItem) item);
    }

    public void FireEndInteract(InteractableComponent interactable, InteractItem item)
    {
      Action<IEntity, IInteractableComponent, IInteractItem> endInteractEvent = this.EndInteractEvent;
      if (endInteractEvent == null)
        return;
      endInteractEvent(this.Owner, (IInteractableComponent) interactable, (IInteractItem) item);
    }

    private void AttackerPlayer_HandsHolsteredEvent(WeaponKind weapon)
    {
      Action<WeaponKind, bool> weaponEnableChanged = this.OnWeaponEnableChanged;
      if (weaponEnableChanged == null)
        return;
      weaponEnableChanged(weapon, false);
    }

    private void AttackerPlayer_HandsUnholsteredEvent(WeaponKind weapon)
    {
      Action<WeaponKind, bool> weaponEnableChanged = this.OnWeaponEnableChanged;
      if (weaponEnableChanged == null)
        return;
      weaponEnableChanged(weapon, true);
    }

    private bool GenericPlayerMenuListener(GameActionType type, bool down)
    {
      if (!(type == GameActionType.GenericPlayerMenu & down) || !PlayerUtility.IsPlayerCanControlling)
        return false;
      return SimplePlayerWindowSwapper.LastOpenedPlayerWindowType == typeof (IInventoryWindow) ? this.InventoryListener(type, down) : SimplePlayerWindowSwapper.CallLastPlayerWindow();
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
      IStorageComponent component = this.Owner.GetComponent<IStorageComponent>();
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
      if (InputService.Instance.JoystickUsed || !down || !PlayerUtility.IsPlayerCanControlling || this.attackerPlayer == null)
        return false;
      if (this.attackerPlayer.IsUnholstered)
        this.attackerPlayer.HandsHolster();
      else
        this.attackerPlayer.HandsUnholster();
      return true;
    }

    private bool NextWeaponListener(GameActionType type, bool down)
    {
      if (!down || !PlayerUtility.IsPlayerCanControlling || this.attackerPlayer == null)
        return false;
      this.attackerPlayer.NextWeapon();
      return true;
    }

    private bool PrevWeaponListener(GameActionType type, bool down)
    {
      if (!down || !PlayerUtility.IsPlayerCanControlling || this.attackerPlayer == null)
        return false;
      this.attackerPlayer.PrevWeapon();
      return true;
    }

    private bool StealthListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling)
          return false;
        this.IsStelth.Value = true;
        this.IsRun.Value = false;
        return true;
      }
      this.IsStelth.Value = false;
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
        if (!PlayerUtility.IsPlayerCanControlling || this.IsStelth.Value)
          return false;
        if (JoystickLayoutSwitcher.Instance.CurrentLayout != JoystickLayoutSwitcher.KeyLayouts.Three && this.IsRun.Value)
        {
          this.IsRun.Value = false;
          return false;
        }
        this.IsRun.Value = true;
        return true;
      }
      if (!InputService.Instance.JoystickUsed || JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.Three)
        this.IsRun.Value = false;
      return false;
    }

    private bool JumpListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling || this.IsStelth.Value)
          return false;
        this.IsJump = true;
        return true;
      }
      this.IsJump = false;
      return false;
    }

    private bool RightListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling)
          return false;
        this.right = true;
        this.UpdateWalk();
        return true;
      }
      this.right = false;
      this.UpdateWalk();
      return false;
    }

    private bool LeftListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling)
          return false;
        this.left = true;
        this.UpdateWalk();
        return true;
      }
      this.left = false;
      this.UpdateWalk();
      return false;
    }

    private bool BackwardListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling)
          return false;
        this.backward = true;
        this.UpdateWalk();
        return true;
      }
      this.backward = false;
      this.UpdateWalk();
      return false;
    }

    private bool ForwardListener(GameActionType type, bool down)
    {
      if (down)
      {
        if (!PlayerUtility.IsPlayerCanControlling)
          return false;
        this.forward = true;
        this.UpdateWalk();
        return true;
      }
      this.forward = false;
      this.UpdateWalk();
      return false;
    }

    private void UpdateWalk()
    {
      this.IsWalk.Value = this.IsForward || this.IsBackward || this.IsRight || this.IsLeft;
      if (this.IsWalk.Value || !this.IsRun.Value)
        return;
      this.IsRun.Value = false;
    }

    private void UpdatePlayer()
    {
      if (((Entity) this.Owner).IsAdded && this.Owner.IsEnabledInHierarchy)
        this.AddPlayer();
      else
        this.RemovePlayer();
    }

    private void AddPlayer()
    {
      if (this.added)
        return;
      this.added = true;
      ServiceLocator.GetService<Simulation>().AddPlayer(this.Owner);
    }

    private void RemovePlayer()
    {
      if (!this.added)
        return;
      this.added = false;
      ServiceLocator.GetService<Simulation>().RemovePlayer(this.Owner);
    }

    private void Reset()
    {
      this.IsStelth.Value = false;
      this.IsRun.Value = false;
      this.IsJump = false;
      this.left = false;
      this.right = false;
      this.forward = false;
      this.backward = false;
      this.leftValue = 0.0f;
      this.rightValue = 0.0f;
      this.forwardValue = 0.0f;
      this.backwardValue = 0.0f;
      this.UpdateWalk();
      ServiceLocator.GetService<QuestCompassService>().IsEnabled = false;
    }

    public void ComputeUpdate()
    {
      this.leftValue = 0.0f;
      this.rightValue = 0.0f;
      this.forwardValue = 0.0f;
      this.backwardValue = 0.0f;
      if (!PlayerUtility.IsPlayerCanControlling)
        return;
      float axis1 = InputService.Instance.GetAxis("LeftStickX");
      if ((double) axis1 > 0.0)
        this.rightValue = axis1;
      if ((double) axis1 < 0.0)
        this.leftValue = -axis1;
      float axis2 = InputService.Instance.GetAxis("LeftStickY");
      if ((double) axis2 > 0.0)
        this.backwardValue = axis2;
      if ((double) axis2 < 0.0)
        this.forwardValue = -axis2;
      if (this.stamina != null && (double) this.stamina.Value == 0.0 && this.IsRun.Value)
        this.IsRun.Value = false;
      this.UpdateWalk();
    }

    public void PlayerActivated()
    {
      if (this.attackerPlayer != null)
      {
        this.attackerPlayer.WeaponHolsterStartEvent += new Action<WeaponKind>(this.AttackerPlayer_HandsHolsteredEvent);
        this.attackerPlayer.WeaponUnholsterEndEvent += new Action<WeaponKind>(this.AttackerPlayer_HandsUnholsteredEvent);
      }
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
      this.AddListeners();
      this.stamina = this.Owner.GetComponent<ParametersComponent>().GetByName<float>(ParameterNameEnum.Stamina);
    }

    public void AddListeners()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.BoundCharacters, new GameActionHandle(this.BoundCharactersListener));
      service.AddListener(GameActionType.Inventory, new GameActionHandle(this.InventoryListener));
      service.AddListener(GameActionType.MindMap, new GameActionHandle(this.MindMapListener));
      service.AddListener(GameActionType.Map, new GameActionHandle(this.MapListener));
      service.AddListener(GameActionType.GenericPlayerMenu, new GameActionHandle(this.GenericPlayerMenuListener));
      service.AddListener(GameActionType.Unholster, new GameActionHandle(this.UnholsterListener));
      service.AddListener(GameActionType.Compass, new GameActionHandle(this.CompassListener));
      service.AddListener(GameActionType.NextWeapon, new GameActionHandle(this.NextWeaponListener));
      service.AddListener(GameActionType.PrevWeapon, new GameActionHandle(this.PrevWeaponListener));
      service.AddListener(GameActionType.Stealth, new GameActionHandle(this.StealthListener));
      service.AddListener(GameActionType.Run, new GameActionHandle(this.RunListener));
      service.AddListener(GameActionType.Jump, new GameActionHandle(this.JumpListener));
      service.AddListener(GameActionType.Forward, new GameActionHandle(this.ForwardListener));
      service.AddListener(GameActionType.Backward, new GameActionHandle(this.BackwardListener));
      service.AddListener(GameActionType.Left, new GameActionHandle(this.LeftListener));
      service.AddListener(GameActionType.Right, new GameActionHandle(this.RightListener));
    }

    public void RemoveListeners()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.BoundCharacters, new GameActionHandle(this.BoundCharactersListener));
      service.RemoveListener(GameActionType.Inventory, new GameActionHandle(this.InventoryListener));
      service.RemoveListener(GameActionType.MindMap, new GameActionHandle(this.MindMapListener));
      service.RemoveListener(GameActionType.Map, new GameActionHandle(this.MapListener));
      service.RemoveListener(GameActionType.GenericPlayerMenu, new GameActionHandle(this.GenericPlayerMenuListener));
      service.RemoveListener(GameActionType.Unholster, new GameActionHandle(this.UnholsterListener));
      service.RemoveListener(GameActionType.Compass, new GameActionHandle(this.CompassListener));
      service.RemoveListener(GameActionType.NextWeapon, new GameActionHandle(this.NextWeaponListener));
      service.RemoveListener(GameActionType.PrevWeapon, new GameActionHandle(this.PrevWeaponListener));
      service.RemoveListener(GameActionType.Stealth, new GameActionHandle(this.StealthListener));
      service.RemoveListener(GameActionType.Run, new GameActionHandle(this.RunListener));
      service.RemoveListener(GameActionType.Jump, new GameActionHandle(this.JumpListener));
      service.RemoveListener(GameActionType.Forward, new GameActionHandle(this.ForwardListener));
      service.RemoveListener(GameActionType.Backward, new GameActionHandle(this.BackwardListener));
      service.RemoveListener(GameActionType.Left, new GameActionHandle(this.LeftListener));
      service.RemoveListener(GameActionType.Right, new GameActionHandle(this.RightListener));
      this.Reset();
      ServiceLocator.GetService<QuestCompassService>().IsEnabled = false;
    }

    public void PlayerDeactivated()
    {
      this.RemoveListeners();
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      if (this.attackerPlayer == null)
        return;
      this.attackerPlayer.WeaponUnholsterEndEvent -= new Action<WeaponKind>(this.AttackerPlayer_HandsUnholsteredEvent);
      this.attackerPlayer.WeaponHolsterStartEvent -= new Action<WeaponKind>(this.AttackerPlayer_HandsHolsteredEvent);
    }
  }
}
