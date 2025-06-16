using System;
using System.Collections.Generic;
using Engine.Behaviours.Engines.Controllers;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

namespace Engine.Behaviours.Engines.Services
{
  public class PlayerWeaponServiceNew : MonoBehaviour, IEntityAttachable
  {
    private IEntity target;
    private Animator animator;
    private PlayerAnimatorState animatorState;
    private bool isAttached;
    private bool isPaused;
    private List<PlayerWeaponSwitchCommand> switchCommandsList = new List<PlayerWeaponSwitchCommand>();
    private bool enabled;
    private WeaponKind kind = WeaponKind.Unknown;
    private IWeaponController currentWeaponController;
    private Dictionary<WeaponKind, IWeaponController> controllers = new Dictionary<WeaponKind, IWeaponController> {
      {
        WeaponKind.Unknown,
        new EmptyWeaponController()
      },
      {
        WeaponKind.Hands,
        new PlayerHandsWeaponController()
      },
      {
        WeaponKind.Knife,
        new PlayerKnifeWeaponController()
      },
      {
        WeaponKind.Revolver,
        new PlayerRevolverWeaponController()
      },
      {
        WeaponKind.Rifle,
        new PlayerRifleWeaponController()
      },
      {
        WeaponKind.Visir,
        new PlayerVisirWeaponController()
      },
      {
        WeaponKind.Flashlight,
        new PlayerFlashlightWeaponController()
      },
      {
        WeaponKind.Lockpick,
        new PlayerLockpickWeaponController()
      },
      {
        WeaponKind.Scalpel,
        new PlayerScalpelWeaponController()
      },
      {
        WeaponKind.Shotgun,
        new PlayerShotgunWeaponController()
      }
    };

    public event Action<WeaponKind> WeaponHolsterStartEvent;

    public event Action<WeaponKind> WeaponUnholsterEndEvent;

    public event Action<WeaponKind, IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

    public float ReactionLayerWeight { get; set; }

    public bool IsPaused
    {
      get => isPaused;
      set
      {
        isPaused = value;
        if (!(animator != null))
          return;
        if (isPaused)
          animator.SetFloat("Mecanim.Speed", 0.0f);
        else
          animator.SetFloat("Mecanim.Speed", 1f);
      }
    }

    private void Awake()
    {
      animator = gameObject.GetComponent<Animator>();
      if (animator == null)
      {
        Debug.LogError(string.Format("{0} has no animator", gameObject));
      }
      else
      {
        animatorState = PlayerAnimatorState.GetAnimatorState(animator);
        IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
      }
    }

    void IEntityAttachable.Attach(IEntity owner)
    {
      isAttached = true;
      Initialize(owner, animator, true);
    }

    void IEntityAttachable.Detach() => isAttached = false;

    [Inspected(Mutable = true)]
    public bool GeometryVisible
    {
      set
      {
        if (!value)
          switchCommandsList.RemoveAll(x => !x.SwitchOn);
        foreach (KeyValuePair<WeaponKind, IWeaponController> controller in controllers)
          controller.Value.GeometryVisible = false;
        if (currentWeaponController != null)
          currentWeaponController.GeometryVisible = value;
        if (!value)
          return;
        switchCommandsList.Clear();
      }
    }

    [Inspected(Mutable = true)]
    public WeaponKind KindBase => kind;

    public void SetWeapon(WeaponKind kind, IEntity item)
    {
      if (this.kind == kind && currentWeaponController != null && currentWeaponController.GetItem() == item)
        return;
      if (!enabled)
      {
        currentWeaponController?.Shutdown();
        currentWeaponController = controllers[kind];
        currentWeaponController.SetItem(item);
        currentWeaponController.GeometryVisible = true;
        currentWeaponController.Activate(true);
        this.kind = kind;
      }
      else
      {
        if (switchCommandsList.Exists(x => x.SwitchOn && x.IsActive && kind == WeaponKind.Unknown))
          return;
        switchCommandsList.RemoveAll(x => !x.IsActive);
        if (this.kind != WeaponKind.Unknown && currentWeaponController.GeometryVisible)
          AddCommand(this.kind, false);
        if (kind == 0)
          return;
        AddCommand(kind, true, item);
      }
    }

    private void AddCommand(WeaponKind weapon, bool switchOn, IEntity item = null)
    {
      PlayerWeaponSwitchCommand command = new PlayerWeaponSwitchCommand();
      command.WeaponKind = weapon;
      command.SwitchOn = switchOn;
      command.item = item;
      switchCommandsList.Add(command);
      if (switchCommandsList.Exists(x => x.IsActive))
        return;
      ExecuteCommand(command);
    }

    public void OnWeaponSwitch(WeaponKind kind, bool switchOn)
    {
      PlayerWeaponSwitchCommand weaponSwitchCommand = switchCommandsList.Find(x => x.WeaponKind == kind && x.SwitchOn == switchOn);
      if (weaponSwitchCommand == null)
        return;
      weaponSwitchCommand.IsActive = false;
      IWeaponController controller = controllers[weaponSwitchCommand.WeaponKind];
      switchCommandsList.Remove(weaponSwitchCommand);
      if (!switchOn)
      {
        controller.GeometryVisible = false;
        kind = WeaponKind.Unknown;
        controllers[kind].GeometryVisible = true;
      }
      if (switchCommandsList.Count <= 0)
        return;
      ExecuteCommand(switchCommandsList[0]);
    }

    private void ExecuteCommand(PlayerWeaponSwitchCommand command)
    {
      kind = WeaponKind.Unknown;
      command.IsActive = true;
      IWeaponController controller1 = controllers[command.WeaponKind];
      if (command.SwitchOn)
      {
        currentWeaponController?.Shutdown();
        foreach (KeyValuePair<WeaponKind, IWeaponController> controller2 in controllers)
          controller2.Value.GeometryVisible = false;
        controller1.GeometryVisible = true;
        controller1.Activate(true);
        controller1.SetItem(command.item);
        kind = command.WeaponKind;
        currentWeaponController = controller1;
      }
      else
      {
        controller1.Shutdown();
        currentWeaponController = controllers[WeaponKind.Unknown];
      }
    }

    public void OnEnable()
    {
      enabled = true;
      switchCommandsList.Clear();
      currentWeaponController?.OnEnable();
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= WorldPauseHandler;
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += WorldPauseHandler;
      IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
    }

    public void OnDisable()
    {
      enabled = false;
      currentWeaponController?.OnDisable();
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= WorldPauseHandler;
      IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
    }

    private void OnDestroy() => currentWeaponController?.Shutdown();

    [Inspected]
    public IEntity Target
    {
      get => target;
      set
      {
        if (target == value)
          return;
        target = value;
      }
    }

    public void Initialize(IEntity entity, Animator animator, bool geometryVisible)
    {
      this.animator = animator;
      foreach (KeyValuePair<WeaponKind, IWeaponController> controller1 in controllers)
      {
        KeyValuePair<WeaponKind, IWeaponController> controller = controller1;
        controller.Value.WeaponHolsterStartEvent += (Action) (() =>
        {
          Action<WeaponKind> holsterStartEvent = WeaponHolsterStartEvent;
          if (holsterStartEvent == null)
            return;
          holsterStartEvent(controller.Key);
        });
        controller.Value.WeaponUnholsterEndEvent += (Action) (() =>
        {
          Action<WeaponKind> unholsterEndEvent = WeaponUnholsterEndEvent;
          if (unholsterEndEvent == null)
            return;
          unholsterEndEvent(controller.Key);
        });
        controller.Value.WeaponShootEvent += (weapon, shotType, reactionType, shotSubtype) =>
        {
          Action<WeaponKind, IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
          if (weaponShootEvent == null)
            return;
          weaponShootEvent(controller.Key, weapon, shotType, reactionType, shotSubtype);
        };
        controller.Value.Initialise(entity, gameObject, this.animator);
      }
      currentWeaponController = controllers[WeaponKind.Unknown];
      currentWeaponController.GeometryVisible = true;
      GeometryVisible = geometryVisible;
      switchCommandsList = new List<PlayerWeaponSwitchCommand>();
    }

    public void ResetWeapon()
    {
      if (!isAttached)
        return;
      currentWeaponController.Reset();
    }

    public void Update()
    {
      if (!isAttached)
        return;
      currentWeaponController?.Update(Target);
    }

    public void LateUpdate()
    {
      if (!isAttached)
        return;
      currentWeaponController?.LateUpdate(Target);
    }

    public void FixedUpdate()
    {
      if (!isAttached)
        return;
      currentWeaponController?.FixedUpdate(Target);
    }

    public void AnimatorEvent(string data)
    {
      if (!isAttached)
        return;
      currentWeaponController.OnAnimatorEvent(data);
    }

    private void WorldPauseHandler()
    {
      IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
    }

    public IWeaponController GetCurrentWeaponController() => currentWeaponController;

    public bool IsWeaponOn() => !(currentWeaponController is EmptyWeaponController);

    public void Reaction() => currentWeaponController.Reaction();
  }
}
