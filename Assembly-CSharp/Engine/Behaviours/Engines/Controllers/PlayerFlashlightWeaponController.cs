using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;

namespace Engine.Behaviours.Engines.Controllers
{
  public class PlayerFlashlightWeaponController : IWeaponController
  {
    private const float punchCooldownTime = 1f;
    private GameObject gameObject;
    private Animator animator;
    private PlayerEnemy playerEnemy;
    private PlayerAnimatorState animatorState;
    private PivotPlayer pivot;
    private TimeService timeService;
    private IEntity entity;
    private DetectableComponent detectable;
    private ControllerComponent controllerComponent;
    private bool geometryVisible;
    private bool weaponVisible;
    private float timeToLastPunch;
    private StorageComponent storage;
    private List<StorableComponent> storageAmmo = new List<StorableComponent>();
    private IParameter<bool> lowStamina;
    private IParameter<bool> flashlightOn;
    private float blockStance;
    private float realBlockStance;
    private float realStamina;
    private IParameter<TimeSpan> WorkEndTime;
    private IParameter<TimeSpan> WorkTime;
    private float smoothedNormalizedSpeed;
    private float layerWeight;
    private bool listenersAdded;
    private IEntity item;

    public event Action WeaponUnholsterEndEvent;

    public event Action WeaponHolsterStartEvent;

    public event Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

    public bool IsOn { get; private set; }

    bool IWeaponController.GeometryVisible
    {
      set
      {
        geometryVisible = value;
        ApplyVisibility();
        ApplyLayerWeight(geometryVisible ? 1f : 0.0f);
      }
      get => geometryVisible;
    }

    private bool WeaponVisible
    {
      set
      {
        weaponVisible = value;
        ApplyVisibility();
      }
    }

    private void ApplyVisibility()
    {
      pivot.HandsGeometryVisible = geometryVisible;
      pivot.FlashlightGeometryVisible = geometryVisible && weaponVisible;
      ApplyLayerWeight(geometryVisible ? 1f : 0.0f);
      if (!geometryVisible)
        flashlightOn.Value = false;
      pivot.FlashlightFire.SetActive(geometryVisible);
      if (!geometryVisible || !weaponVisible)
        return;
      TrySwitchOn();
    }

    public void OnEnable()
    {
      ApplyVisibility();
      animator.SetTrigger("Triggers/FlashlightRestore");
      AddListeners();
    }

    public void OnDisable()
    {
      SwitchLight(false);
      RemoveListeners();
    }

    private void ApplyLayerWeight(float weight)
    {
      animatorState.FlashlightLayerWeight = weight;
      animatorState.FlashlightReactionLayerWeight = weight;
    }

    public void Initialise(IEntity entity, GameObject gameObject, Animator animator)
    {
      this.entity = entity;
      pivot = gameObject.GetComponent<PivotPlayer>();
      if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("{0} has no {1} unity component", (object) gameObject.name, (object) typeof (PivotPlayer).Name);
      }
      else
      {
        this.gameObject = gameObject;
        this.animator = animator;
        playerEnemy = gameObject.GetComponent<PlayerEnemy>();
        controllerComponent = entity.GetComponent<ControllerComponent>();
        animatorState = PlayerAnimatorState.GetAnimatorState(animator);
        if (entity == null)
        {
          Debug.LogWarningFormat("{0} can't map entity", (object) gameObject.name);
        }
        else
        {
          lowStamina = entity.GetComponent<ParametersComponent>().GetByName<bool>(ParameterNameEnum.LowStamina);
          flashlightOn = entity.GetComponent<ParametersComponent>().GetByName<bool>(ParameterNameEnum.Flashlight);
          storage = entity.GetComponent<StorageComponent>();
          timeService = ServiceLocator.GetService<TimeService>();
        }
      }
    }

    public IEntity GetItem() => item;

    public void SetItem(IEntity item)
    {
      this.item = item;
      TrySwitchOn();
    }

    private void TrySwitchOn()
    {
      if (item == null)
        return;
      ParametersComponent component = item.GetComponent<ParametersComponent>();
      if (component != null)
      {
        WorkTime = component.GetByName<TimeSpan>(ParameterNameEnum.WorkTime);
        WorkEndTime = component.GetByName<TimeSpan>(ParameterNameEnum.WorkEndTime);
        IsOn = WorkEndTime.Value > timeService.GameTime;
        SwitchLight(IsOn);
        if (!IsOn)
        {
          Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
          if (weaponShootEvent != null)
            weaponShootEvent(item, ShotType.None, ReactionType.Right, ShotSubtypeEnum.FireLamp);
        }
      }
    }

    public void Activate(bool geometryVisible)
    {
      WeaponVisible = true;
      pivot.FlashlightEffect.SetActive(true);
      animatorState.FlashlightUnholster();
      Action unholsterEndEvent = WeaponUnholsterEndEvent;
      if (unholsterEndEvent != null)
        unholsterEndEvent();
      AddListeners();
      RefreshStorageAmmo();
      flashlightOn.Value = IsOn;
    }

    private void RefreshStorageAmmo()
    {
      storageAmmo.Clear();
      storageAmmo.AddRange(storage.Items.ToList().FindAll(x => x.Groups.Contains(StorableGroup.Fuel_Lamp) && x.Count > 0).Cast<StorableComponent>());
    }

    private int StorageAmmoCount()
    {
      int num = 0;
      foreach (StorableComponent storableComponent in storageAmmo)
        num += storableComponent.Count;
      return num;
    }

    private void RemoveAmmo()
    {
      if (storageAmmo.Count > 0)
      {
        --storageAmmo[0].Count;
        if (storageAmmo[0].Count <= 0)
          storageAmmo[0].Owner.Dispose();
      }
      RefreshStorageAmmo();
    }

    public void Shutdown()
    {
      RemoveListeners();
      pivot.FlashlightEffect.SetActive(false);
      animatorState.FlashlightHolster();
      Action holsterStartEvent = WeaponHolsterStartEvent;
      if (holsterStartEvent != null)
        holsterStartEvent();
      blockStance = 0.0f;
    }

    private void AddListeners()
    {
      if (listenersAdded)
        return;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Fire, PunchListener);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Block, BlockListener);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Flashlight, LightListener);
      listenersAdded = true;
    }

    private void RemoveListeners()
    {
      if (!listenersAdded)
        return;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Fire, PunchListener);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Block, BlockListener);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Flashlight, LightListener);
      listenersAdded = false;
    }

    public void Reset() => animatorState.ResetAnimator();

    public bool Validate(GameObject gameObject, IEntity item) => true;

    public void Update(IEntity target)
    {
      if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return;
      float target1 = 0.0f;
      bool flag = controllerComponent != null && controllerComponent.IsRun.Value;
      if (controllerComponent.IsWalk.Value)
        target1 = (flag ? 1f : 0.5f) * controllerComponent.WalkModifier.Value;
      smoothedNormalizedSpeed = Mathf.MoveTowards(smoothedNormalizedSpeed, target1, Time.deltaTime / 1f);
      animatorState.WalkSpeed = smoothedNormalizedSpeed;
      if (timeToLastPunch <= 0.0)
        return;
      timeToLastPunch -= Time.deltaTime;
    }

    public void UpdateSilent(IEntity target)
    {
      if (!InstanceByRequest<EngineApplication>.Instance.IsPaused)
        ;
    }

    public void LateUpdate(IEntity target)
    {
    }

    public void FixedUpdate(IEntity target)
    {
      realBlockStance = Mathf.MoveTowards(realBlockStance, blockStance, Time.fixedDeltaTime / ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerBlockStanceTime);
      animatorState.BlockStance = SmoothUtility.Smooth12(realBlockStance);
      realStamina = Mathf.MoveTowards(realStamina, lowStamina.Value ? 0.0f : 1f, Time.fixedDeltaTime / ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerBlockStanceTime);
      animatorState.Stamina = realStamina;
      if ((UnityEngine.Object) playerEnemy != (UnityEngine.Object) null)
        playerEnemy.BlockStance = realBlockStance >= 0.5;
      bool isOn = WorkEndTime.Value > timeService.GameTime;
      if (isOn || !IsOn)
        return;
      SwitchLight(isOn);
    }

    private bool LightListener(GameActionType type, bool down)
    {
      if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling || IsOn)
        return false;
      if (down)
      {
        RefreshStorageAmmo();
        if (StorageAmmoCount() <= 0)
        {
          Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
          if (weaponShootEvent != null)
            weaponShootEvent(item, ShotType.None, ReactionType.Right, ShotSubtypeEnum.NoMatches);
          return false;
        }
        FireLamp();
      }
      return true;
    }

    private void FireLamp()
    {
      if (StorageAmmoCount() <= 0)
        return;
      TimeSpan timeSpan = WorkTime.MinValue;
      double totalSeconds1 = timeSpan.TotalSeconds;
      timeSpan = WorkTime.MaxValue;
      double totalSeconds2 = timeSpan.TotalSeconds;
      WorkEndTime.Value = timeService.GameTime + TimeSpan.FromSeconds((double) UnityEngine.Random.Range((float) totalSeconds1, (float) totalSeconds2));
      SwitchLight(true);
      RemoveAmmo();
      Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
      if (weaponShootEvent != null)
        weaponShootEvent(item, ShotType.Light, ReactionType.None, ShotSubtypeEnum.None);
    }

    private void SwitchLight(bool isOn)
    {
      IsOn = isOn;
      ParticleSystem componentInChildren = pivot.FlashlightEffect.GetComponentInChildren<ParticleSystem>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      {
        if (isOn)
          componentInChildren.Play();
        else
          componentInChildren.Stop();
      }
      pivot.FlashlightLight.SetActive(isOn);
      flashlightOn.Value = IsOn;
    }

    private bool PunchListener(GameActionType type, bool down)
    {
      if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return false;
      if (down && timeToLastPunch <= 0.0)
      {
        blockStance = 0.0f;
        if (lowStamina.Value)
          animatorState.FlashlightPunchLowStamina();
        else
          animatorState.FlashlightPunch();
        timeToLastPunch = ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerPunchCooldownTime;
        return true;
      }
      if (!down && timeToLastPunch <= 0.0)
      {
        blockStance = 0.0f;
        realBlockStance = 0.0f;
        animatorState.BlockStance = SmoothUtility.Smooth12(realBlockStance);
      }
      return true;
    }

    private bool BlockListener(GameActionType type, bool down)
    {
      if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return false;
      blockStance = down ? 1f : 0.0f;
      return true;
    }

    public void OnAnimatorEvent(string data)
    {
      if (data.StartsWith("Flashlight.Punch"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(item, ShotType.Moderate, ReactionType.Right, ShotSubtypeEnum.None);
      }
      else if (data.StartsWith("Flashlight.LowStamina"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(item, ShotType.LowStamina, ReactionType.Right, ShotSubtypeEnum.None);
      }
      else if (data.StartsWith("Flashlight.PrePunch"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(item, ShotType.Prepunch, ReactionType.Right, ShotSubtypeEnum.None);
      }
      else
      {
        if (!data.StartsWith("Flashlight.Light"))
          return;
        FireLamp();
      }
    }

    public void Reaction()
    {
    }
  }
}
