using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using System;
using UnityEngine;

namespace Engine.Behaviours.Engines.Controllers
{
  public abstract class PlayerUppercotWeaponControllerBase : IWeaponController
  {
    private GameObject gameObject;
    private Animator animator;
    private PlayerEnemy playerEnemy;
    protected PlayerAnimatorState animatorState;
    protected PivotPlayer pivot;
    private IEntity entity;
    private DetectableComponent detectable;
    private IParameter<bool> lowStamina;
    private IParameter<bool> stealth;
    private IParameter<float> durability;
    private ControllerComponent controllerComponent;
    protected bool geometryVisible;
    protected bool weaponVisible;
    private float timeToLastPunch = 0.0f;
    private float blockStance = 0.0f;
    private float realBlockStance = 0.0f;
    private float realStamina = 0.0f;
    private bool lastPunchWasByLeftHand = false;
    private CharacterController characterController;
    protected IEntity item;
    private string WeaponPunchString;
    private string WeaponPunchLowStaminaString;
    private string WeaponPrepunchString;
    private string WeaponPushString;
    private string WeaponUppercutString;
    private string WeaponBackstabString;
    private string WeaponSpecialPrepunchString;
    private float fireDownTime;
    private bool fireButtonDown;
    private bool isBlocking = false;
    private bool listenersAdded;
    private float smoothedNormalizedSpeed = 0.0f;
    private float layerWeight;

    public event Action WeaponUnholsterEndEvent;

    public event Action WeaponHolsterStartEvent;

    public event Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

    protected abstract void ApplyVisibility();

    protected abstract void ApplyLayerWeight(float weight);

    protected abstract WeaponKind WeaponKind { get; }

    protected abstract string Prefix { get; }

    protected abstract bool SupportsLowStaminaPunch { get; }

    bool IWeaponController.GeometryVisible
    {
      set
      {
        this.geometryVisible = value;
        this.ApplyVisibility();
      }
      get => this.geometryVisible;
    }

    private bool WeaponVisible
    {
      set
      {
        this.weaponVisible = value;
        this.ApplyVisibility();
      }
    }

    public void Initialise(IEntity entity, GameObject gameObject, Animator animator)
    {
      this.entity = entity;
      this.pivot = gameObject.GetComponent<PivotPlayer>();
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("{0} has no {1} unity component", (object) gameObject.name, (object) typeof (PivotPlayer).Name);
      }
      else
      {
        this.gameObject = gameObject;
        this.animator = animator;
        this.playerEnemy = gameObject.GetComponent<PlayerEnemy>();
        this.characterController = gameObject.GetComponent<CharacterController>();
        this.controllerComponent = entity.GetComponent<ControllerComponent>();
        this.animatorState = PlayerAnimatorState.GetAnimatorState(animator);
        this.lowStamina = entity.GetComponent<ParametersComponent>().GetByName<bool>(ParameterNameEnum.LowStamina);
        this.stealth = entity.GetComponent<ParametersComponent>().GetByName<bool>(ParameterNameEnum.Stealth);
        if (entity == null)
        {
          Debug.LogWarningFormat("{0} can't map entity", (object) gameObject.name);
        }
        else
        {
          this.detectable = (DetectableComponent) entity.GetComponent<IDetectableComponent>();
          if (this.detectable == null)
          {
            Debug.LogWarningFormat("{0} doesn't have {1} engine component", (object) gameObject.name, (object) typeof (IDetectableComponent).Name);
          }
          else
          {
            this.WeaponPunchString = this.Prefix + ".Punch";
            this.WeaponPunchLowStaminaString = this.Prefix + ".PunchLowStamina";
            this.WeaponPrepunchString = this.Prefix + ".Prepunch";
            this.WeaponPushString = this.Prefix + ".Push";
            this.WeaponUppercutString = this.Prefix + ".Uppercut";
            this.WeaponBackstabString = this.Prefix + ".Backstab";
            this.WeaponSpecialPrepunchString = this.Prefix + ".SpecialPrepunch";
          }
        }
      }
    }

    public IEntity GetItem() => this.item;

    public virtual void SetItem(IEntity item)
    {
      this.item = item;
      ParametersComponent component = item.GetComponent<ParametersComponent>();
      if (component == null)
        return;
      this.durability = component.GetByName<float>(ParameterNameEnum.Durability);
    }

    public void OnEnable()
    {
      this.ApplyVisibility();
      this.animator.SetTrigger("Triggers/Restore");
      this.AddListeners();
    }

    public void OnDisable()
    {
      this.blockStance = 0.0f;
      this.RemoveListeners();
    }

    public void Activate(bool geometryVisible)
    {
      this.WeaponVisible = true;
      this.animatorState.Unholster();
      Action unholsterEndEvent = this.WeaponUnholsterEndEvent;
      if (unholsterEndEvent != null)
        unholsterEndEvent();
      this.AddListeners();
    }

    public void Shutdown()
    {
      Action holsterStartEvent = this.WeaponHolsterStartEvent;
      if (holsterStartEvent != null)
        holsterStartEvent();
      this.RemoveListeners();
      this.animatorState.Holster();
      this.blockStance = 0.0f;
      this.playerEnemy.BlockStance = false;
      this.isBlocking = false;
    }

    private void AddListeners()
    {
      if (this.listenersAdded)
        return;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Fire, new GameActionHandle(this.PunchListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Block, new GameActionHandle(this.BlockListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Push, new GameActionHandle(this.PushListener));
      this.listenersAdded = true;
    }

    private void RemoveListeners()
    {
      if (!this.listenersAdded)
        return;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Fire, new GameActionHandle(this.PunchListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Block, new GameActionHandle(this.BlockListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Push, new GameActionHandle(this.PushListener));
      this.listenersAdded = false;
    }

    private bool PunchListener(GameActionType type, bool down)
    {
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling || this.playerEnemy.IsStagger)
        return false;
      this.fireButtonDown = down;
      if (down && (double) this.timeToLastPunch <= 0.0)
      {
        if (this.SupportsLowStaminaPunch && this.lowStamina.Value)
        {
          this.animatorState.PunchLowStamina();
          this.timeToLastPunch = 1.5f;
        }
        else
        {
          this.blockStance = 0.0f;
          if (this.stealth.Value)
          {
            this.animatorState.PunchBackstab();
            this.timeToLastPunch = 1.2f;
          }
          else
            this.animatorState.PunchUppercut();
        }
        if (this.durability != null && (double) this.durability.Value <= 0.0)
        {
          Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
          if (weaponShootEvent != null)
            weaponShootEvent(this.item, ShotType.None, ReactionType.None, ShotSubtypeEnum.WeaponBroken);
        }
        return true;
      }
      if (down || (double) this.fireDownTime >= 0.40000000596046448 || (double) this.timeToLastPunch > 0.0)
        return true;
      this.blockStance = 0.0f;
      this.realBlockStance = 0.0f;
      this.animatorState.BlockStance = SmoothUtility.Smooth12(this.realBlockStance);
      if (this.SupportsLowStaminaPunch && this.lowStamina.Value)
      {
        this.animatorState.PunchLowStamina();
        this.timeToLastPunch = 1.5f;
      }
      else
      {
        if (this.lastPunchWasByLeftHand)
          this.animatorState.PunchRight();
        else
          this.animatorState.PunchLeft();
        this.lastPunchWasByLeftHand = !this.lastPunchWasByLeftHand;
        this.timeToLastPunch = 0.8f;
      }
      if (this.durability != null && (double) this.durability.Value <= 0.0)
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(this.item, ShotType.None, ReactionType.None, ShotSubtypeEnum.WeaponBroken);
      }
      return true;
    }

    private bool PushListener(GameActionType type, bool down)
    {
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling || this.lowStamina.Value)
        return false;
      if (!down || (double) this.timeToLastPunch > 0.0)
        return true;
      this.animatorState.Push();
      this.timeToLastPunch = ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerPunchCooldownTime;
      return true;
    }

    private bool BlockListener(GameActionType type, bool down)
    {
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return false;
      this.isBlocking = down;
      return true;
    }

    public void Update(IEntity target)
    {
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return;
      float target1 = 0.0f;
      bool flag = this.controllerComponent != null && this.controllerComponent.IsRun.Value;
      if (this.controllerComponent.IsWalk.Value)
        target1 = (flag ? 1f : 0.5f) * this.controllerComponent.WalkModifier.Value;
      if (this.fireButtonDown)
        this.fireDownTime += Time.deltaTime;
      else
        this.fireDownTime = 0.0f;
      if ((double) this.timeToLastPunch > 0.0)
        this.timeToLastPunch -= Time.deltaTime;
      this.smoothedNormalizedSpeed = Mathf.MoveTowards(this.smoothedNormalizedSpeed, target1, Time.deltaTime / 1f);
      this.animatorState.WalkSpeed = this.smoothedNormalizedSpeed;
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return;
      this.blockStance = !this.isBlocking || (double) this.timeToLastPunch > 0.0 ? 0.0f : 1f;
    }

    public void UpdateSilent(IEntity target)
    {
      if (!InstanceByRequest<EngineApplication>.Instance.IsPaused)
        ;
    }

    public void Reset() => this.animatorState.ResetAnimator();

    public bool Validate(GameObject gameObject, IEntity item) => true;

    public void LateUpdate(IEntity target)
    {
    }

    public void FixedUpdate(IEntity target)
    {
      this.realBlockStance = Mathf.MoveTowards(this.realBlockStance, this.blockStance, Time.fixedDeltaTime / ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerBlockStanceTime);
      this.animatorState.BlockStance = SmoothUtility.Smooth12(this.realBlockStance);
      this.realStamina = Mathf.MoveTowards(this.realStamina, this.lowStamina.Value ? 0.0f : 1f, Time.fixedDeltaTime / ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerBlockStanceTime);
      this.animatorState.Stamina = this.realStamina;
      if (!((UnityEngine.Object) this.playerEnemy != (UnityEngine.Object) null))
        return;
      this.playerEnemy.BlockStance = (double) this.realBlockStance >= 0.5;
    }

    public void OnAnimatorEvent(string data)
    {
      if (data.StartsWith(this.WeaponPunchLowStaminaString))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(this.item, ShotType.LowStamina, this.lastPunchWasByLeftHand ? ReactionType.Left : ReactionType.Right, ShotSubtypeEnum.None);
      }
      else if (data.StartsWith(this.WeaponPunchString))
      {
        ShotType shotType = !this.SupportsLowStaminaPunch ? (!this.lowStamina.Value ? ShotType.Moderate : ShotType.LowStamina) : ShotType.Moderate;
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(this.item, shotType, this.lastPunchWasByLeftHand ? ReactionType.Left : ReactionType.Right, ShotSubtypeEnum.None);
      }
      else if (data.StartsWith(this.WeaponPrepunchString))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(this.item, ShotType.Prepunch, this.lastPunchWasByLeftHand ? ReactionType.Left : ReactionType.Right, ShotSubtypeEnum.None);
      }
      else if (data.StartsWith(this.WeaponPushString))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(this.item, ShotType.Push, ReactionType.None, ShotSubtypeEnum.None);
      }
      else if (data.StartsWith(this.WeaponUppercutString))
      {
        ShotType shotType = !this.SupportsLowStaminaPunch ? (!this.lowStamina.Value ? ShotType.Uppercut : ShotType.LowStamina) : ShotType.Uppercut;
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(this.item, shotType, ReactionType.Uppercut, ShotSubtypeEnum.None);
      }
      else if (data.StartsWith(this.WeaponBackstabString))
      {
        ShotType shotType = !this.SupportsLowStaminaPunch ? (!this.lowStamina.Value ? ShotType.Backstab : ShotType.LowStamina) : ShotType.Backstab;
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(this.item, shotType, ReactionType.Backstab, ShotSubtypeEnum.None);
      }
      else
      {
        if (!data.StartsWith(this.WeaponSpecialPrepunchString))
          return;
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(this.item, ShotType.SpecialPrepunch, ReactionType.Uppercut, ShotSubtypeEnum.None);
      }
    }

    public void Reaction()
    {
    }
  }
}
