// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Services.PlayerWeaponServiceNew
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Engines.Controllers;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Engines.Services
{
  public class PlayerWeaponServiceNew : MonoBehaviour, IEntityAttachable
  {
    private IEntity target;
    private Animator animator;
    private PlayerAnimatorState animatorState;
    private bool isAttached = false;
    private bool isPaused;
    private List<PlayerWeaponSwitchCommand> switchCommandsList = new List<PlayerWeaponSwitchCommand>();
    private bool enabled = false;
    private WeaponKind kind = WeaponKind.Unknown;
    private IWeaponController currentWeaponController;
    private Dictionary<WeaponKind, IWeaponController> controllers = new Dictionary<WeaponKind, IWeaponController>()
    {
      {
        WeaponKind.Unknown,
        (IWeaponController) new EmptyWeaponController()
      },
      {
        WeaponKind.Hands,
        (IWeaponController) new PlayerHandsWeaponController()
      },
      {
        WeaponKind.Knife,
        (IWeaponController) new PlayerKnifeWeaponController()
      },
      {
        WeaponKind.Revolver,
        (IWeaponController) new PlayerRevolverWeaponController()
      },
      {
        WeaponKind.Rifle,
        (IWeaponController) new PlayerRifleWeaponController()
      },
      {
        WeaponKind.Visir,
        (IWeaponController) new PlayerVisirWeaponController()
      },
      {
        WeaponKind.Flashlight,
        (IWeaponController) new PlayerFlashlightWeaponController()
      },
      {
        WeaponKind.Lockpick,
        (IWeaponController) new PlayerLockpickWeaponController()
      },
      {
        WeaponKind.Scalpel,
        (IWeaponController) new PlayerScalpelWeaponController()
      },
      {
        WeaponKind.Shotgun,
        (IWeaponController) new PlayerShotgunWeaponController()
      }
    };

    public event Action<WeaponKind> WeaponHolsterStartEvent;

    public event Action<WeaponKind> WeaponUnholsterEndEvent;

    public event Action<WeaponKind, IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

    public float ReactionLayerWeight { get; set; }

    public bool IsPaused
    {
      get => this.isPaused;
      set
      {
        this.isPaused = value;
        if (!((UnityEngine.Object) this.animator != (UnityEngine.Object) null))
          return;
        if (this.isPaused)
          this.animator.SetFloat("Mecanim.Speed", 0.0f);
        else
          this.animator.SetFloat("Mecanim.Speed", 1f);
      }
    }

    private void Awake()
    {
      this.animator = this.gameObject.GetComponent<Animator>();
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("{0} has no animator", (object) this.gameObject));
      }
      else
      {
        this.animatorState = PlayerAnimatorState.GetAnimatorState(this.animator);
        this.IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
      }
    }

    void IEntityAttachable.Attach(IEntity owner)
    {
      this.isAttached = true;
      this.Initialize(owner, this.animator, true);
    }

    void IEntityAttachable.Detach() => this.isAttached = false;

    [Inspected(Mutable = true)]
    public bool GeometryVisible
    {
      set
      {
        if (!value)
          this.switchCommandsList.RemoveAll((Predicate<PlayerWeaponSwitchCommand>) (x => !x.SwitchOn));
        foreach (KeyValuePair<WeaponKind, IWeaponController> controller in this.controllers)
          controller.Value.GeometryVisible = false;
        if (this.currentWeaponController != null)
          this.currentWeaponController.GeometryVisible = value;
        if (!value)
          return;
        this.switchCommandsList.Clear();
      }
    }

    [Inspected(Mutable = true)]
    public WeaponKind KindBase => this.kind;

    public void SetWeapon(WeaponKind kind, IEntity item)
    {
      if (this.kind == kind && this.currentWeaponController != null && this.currentWeaponController.GetItem() == item)
        return;
      if (!this.enabled)
      {
        this.currentWeaponController?.Shutdown();
        this.currentWeaponController = this.controllers[kind];
        this.currentWeaponController.SetItem(item);
        this.currentWeaponController.GeometryVisible = true;
        this.currentWeaponController.Activate(true);
        this.kind = kind;
      }
      else
      {
        if (this.switchCommandsList.Exists((Predicate<PlayerWeaponSwitchCommand>) (x => x.SwitchOn && x.IsActive && kind == WeaponKind.Unknown)))
          return;
        this.switchCommandsList.RemoveAll((Predicate<PlayerWeaponSwitchCommand>) (x => !x.IsActive));
        if (this.kind != WeaponKind.Unknown && this.currentWeaponController.GeometryVisible)
          this.AddCommand(this.kind, false);
        if (kind == 0)
          return;
        this.AddCommand(kind, true, item);
      }
    }

    private void AddCommand(WeaponKind weapon, bool switchOn, IEntity item = null)
    {
      PlayerWeaponSwitchCommand command = new PlayerWeaponSwitchCommand();
      command.WeaponKind = weapon;
      command.SwitchOn = switchOn;
      command.item = item;
      this.switchCommandsList.Add(command);
      if (this.switchCommandsList.Exists((Predicate<PlayerWeaponSwitchCommand>) (x => x.IsActive)))
        return;
      this.ExecuteCommand(command);
    }

    public void OnWeaponSwitch(WeaponKind kind, bool switchOn)
    {
      PlayerWeaponSwitchCommand weaponSwitchCommand = this.switchCommandsList.Find((Predicate<PlayerWeaponSwitchCommand>) (x => x.WeaponKind == kind && x.SwitchOn == switchOn));
      if (weaponSwitchCommand == null)
        return;
      weaponSwitchCommand.IsActive = false;
      IWeaponController controller = this.controllers[weaponSwitchCommand.WeaponKind];
      this.switchCommandsList.Remove(weaponSwitchCommand);
      if (!switchOn)
      {
        controller.GeometryVisible = false;
        kind = WeaponKind.Unknown;
        this.controllers[kind].GeometryVisible = true;
      }
      if (this.switchCommandsList.Count <= 0)
        return;
      this.ExecuteCommand(this.switchCommandsList[0]);
    }

    private void ExecuteCommand(PlayerWeaponSwitchCommand command)
    {
      this.kind = WeaponKind.Unknown;
      command.IsActive = true;
      IWeaponController controller1 = this.controllers[command.WeaponKind];
      if (command.SwitchOn)
      {
        this.currentWeaponController?.Shutdown();
        foreach (KeyValuePair<WeaponKind, IWeaponController> controller2 in this.controllers)
          controller2.Value.GeometryVisible = false;
        controller1.GeometryVisible = true;
        controller1.Activate(true);
        controller1.SetItem(command.item);
        this.kind = command.WeaponKind;
        this.currentWeaponController = controller1;
      }
      else
      {
        controller1.Shutdown();
        this.currentWeaponController = this.controllers[WeaponKind.Unknown];
      }
    }

    public void OnEnable()
    {
      this.enabled = true;
      this.switchCommandsList.Clear();
      this.currentWeaponController?.OnEnable();
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= new Action(this.WorldPauseHandler);
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += new Action(this.WorldPauseHandler);
      this.IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
    }

    public void OnDisable()
    {
      this.enabled = false;
      this.currentWeaponController?.OnDisable();
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= new Action(this.WorldPauseHandler);
      this.IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
    }

    private void OnDestroy() => this.currentWeaponController?.Shutdown();

    [Inspected]
    public IEntity Target
    {
      get => this.target;
      set
      {
        if (this.target == value)
          return;
        this.target = value;
      }
    }

    public void Initialize(IEntity entity, Animator animator, bool geometryVisible)
    {
      this.animator = animator;
      foreach (KeyValuePair<WeaponKind, IWeaponController> controller1 in this.controllers)
      {
        KeyValuePair<WeaponKind, IWeaponController> controller = controller1;
        controller.Value.WeaponHolsterStartEvent += (Action) (() =>
        {
          Action<WeaponKind> holsterStartEvent = this.WeaponHolsterStartEvent;
          if (holsterStartEvent == null)
            return;
          holsterStartEvent(controller.Key);
        });
        controller.Value.WeaponUnholsterEndEvent += (Action) (() =>
        {
          Action<WeaponKind> unholsterEndEvent = this.WeaponUnholsterEndEvent;
          if (unholsterEndEvent == null)
            return;
          unholsterEndEvent(controller.Key);
        });
        controller.Value.WeaponShootEvent += (Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum>) ((weapon, shotType, reactionType, shotSubtype) =>
        {
          Action<WeaponKind, IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
          if (weaponShootEvent == null)
            return;
          weaponShootEvent(controller.Key, weapon, shotType, reactionType, shotSubtype);
        });
        controller.Value.Initialise(entity, this.gameObject, this.animator);
      }
      this.currentWeaponController = this.controllers[WeaponKind.Unknown];
      this.currentWeaponController.GeometryVisible = true;
      this.GeometryVisible = geometryVisible;
      this.switchCommandsList = new List<PlayerWeaponSwitchCommand>();
    }

    public void ResetWeapon()
    {
      if (!this.isAttached)
        return;
      this.currentWeaponController.Reset();
    }

    public void Update()
    {
      if (!this.isAttached)
        return;
      this.currentWeaponController?.Update(this.Target);
    }

    public void LateUpdate()
    {
      if (!this.isAttached)
        return;
      this.currentWeaponController?.LateUpdate(this.Target);
    }

    public void FixedUpdate()
    {
      if (!this.isAttached)
        return;
      this.currentWeaponController?.FixedUpdate(this.Target);
    }

    public void AnimatorEvent(string data)
    {
      if (!this.isAttached)
        return;
      this.currentWeaponController.OnAnimatorEvent(data);
    }

    private void WorldPauseHandler()
    {
      this.IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
    }

    public IWeaponController GetCurrentWeaponController() => this.currentWeaponController;

    public bool IsWeaponOn() => !(this.currentWeaponController is EmptyWeaponController);

    public void Reaction() => this.currentWeaponController.Reaction();
  }
}
