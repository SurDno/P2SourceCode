using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Behaviours.Engines.Controllers
{
  public class PlayerRevolverWeaponController : IWeaponController
  {
    private GameObject gameObject;
    private Animator animator;
    private PlayerEnemy playerEnemy;
    protected PlayerAnimatorState animatorState;
    protected PivotPlayer pivot;
    private IEntity entity;
    private DetectableComponent detectable;
    private IParameter<float> stamina;
    private ControllerComponent controllerComponent;
    protected bool geometryVisible;
    protected bool weaponVisible;
    private float timeToLastPunch;
    private bool isShoting;
    private StorageComponent storage;
    private List<StorableComponent> storageAmmo = [];
    private IEntity item;
    private IParameter<int> bullets;
    private IParameter<float> durability;
    private bool isReloading;
    private bool reloadingCancelled;
    private bool reloadingEnded;
    private bool isAiming;
    private int maxAmmo = 6;
    private float smoothedNormalizedSpeed;
    private float layerWeight;
    private bool bulletVisible;
    private bool firstReload;
    private bool listenersAdded;
    private int currentBullet;
    private float angleDiff;

    public event Action WeaponUnholsterEndEvent;

    public event Action WeaponHolsterStartEvent;

    public event Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

    bool IWeaponController.GeometryVisible
    {
      set
      {
        geometryVisible = value;
        ApplyVisibility();
        ApplyLayerWeight(geometryVisible ? 1f : 0.0f);
        if (geometryVisible && item != null)
          CheckAmmo();
        if (!geometryVisible && isReloading)
        {
          if (bullets.Value == 0)
          {
            ReloadAmmo();
          }
          else
          {
            animatorState.RevolverRestore();
            isReloading = false;
          }
        }
        isShoting = false;
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

    private void SetBulletVisible(bool visible)
    {
      bulletVisible = visible;
      ApplyBulletVisibility();
    }

    private void ApplyBulletVisibility()
    {
      pivot?.RevolverBullet?.gameObject.SetActive(bulletVisible && geometryVisible);
    }

    protected void ApplyVisibility()
    {
      pivot.HandsGeometryVisible = geometryVisible;
      pivot.RevolverGeometryVisible = geometryVisible && weaponVisible;
      ApplyBulletVisibility();
      ApplyLayerWeight(geometryVisible ? 1f : 0.0f);
    }

    protected void ApplyLayerWeight(float weight)
    {
      animatorState.RevolverLayerWeight = weight;
      animatorState.RevolverReactionLayerWeight = weight * 0.6f;
    }

    public void Initialise(IEntity entity, GameObject gameObject, Animator animator)
    {
      this.entity = entity;
      pivot = gameObject.GetComponent<PivotPlayer>();
      if (pivot == null)
      {
        Debug.LogErrorFormat("{0} has no {1} unity component", gameObject.name, typeof (PivotPlayer).Name);
      }
      else
      {
        this.gameObject = gameObject;
        this.animator = animator;
        playerEnemy = gameObject.GetComponent<PlayerEnemy>();
        controllerComponent = entity.GetComponent<ControllerComponent>();
        animatorState = PlayerAnimatorState.GetAnimatorState(animator);
        stamina = entity.GetComponent<ParametersComponent>().GetByName<float>(ParameterNameEnum.Stamina);
        if (entity == null)
        {
          Debug.LogWarningFormat("{0} can't map entity", gameObject.name);
        }
        else
        {
          detectable = (DetectableComponent) entity.GetComponent<IDetectableComponent>();
          if (detectable == null)
            Debug.LogWarningFormat("{0} doesn't have {1} engine component", gameObject.name, typeof (IDetectableComponent).Name);
          else
            storage = entity.GetComponent<StorageComponent>();
        }
      }
    }

    public IEntity GetItem() => item;

    public void SetItem(IEntity item)
    {
      this.item = item;
      ParametersComponent component = item.GetComponent<ParametersComponent>();
      if (component != null)
      {
        durability = component.GetByName<float>(ParameterNameEnum.Durability);
        bullets = component.GetByName<int>(ParameterNameEnum.Bullets);
      }
      else
      {
        durability = null;
        bullets = null;
      }
      firstReload = bullets != null && bullets.Value == 0;
      if (firstReload)
        EmptyBarrel();
      if (!geometryVisible)
        return;
      CheckAmmo();
    }

    private int StorageAmmoCount()
    {
      int num = 0;
      foreach (StorableComponent storableComponent in storageAmmo)
        num += storableComponent.Count;
      return num;
    }

    private void RefreshAmmoCount()
    {
      RefreshStorageAmmo();
      if (bullets == null)
        return;
      bullets.Value = Mathf.Min(StorageAmmoCount(), bullets.Value, bullets.MaxValue);
    }

    private void CheckAmmo()
    {
      if (bullets != null && bullets.Value != 0 || StorageAmmoCount() <= 0)
        return;
      BeginReloading();
    }

    private void BeginReloading()
    {
      isReloading = true;
      reloadingCancelled = false;
      reloadingEnded = false;
      isAiming = false;
      animatorState.RevolverAim(false);
      animatorState.RevolverReload(true, bullets == null ? 0 : bullets.Value, firstReload);
      firstReload = false;
      angleDiff = 0.0f;
    }

    private void EndReloading()
    {
      reloadingEnded = true;
      animatorState.RevolverReload(false);
    }

    private void EmptyBarrel()
    {
      int num = bullets.Value;
      for (int index = 0; index < maxAmmo; ++index)
      {
        bool flag = index < num;
        pivot?.RevolverBullets[index].gameObject.SetActive(flag);
      }
      currentBullet = maxAmmo - 1;
    }

    private void ReloadAmmo()
    {
      bullets.Value = Mathf.Min(StorageAmmoCount(), bullets.Value + 1, bullets.MaxValue);
      SetBulletVisible(false);
      if (currentBullet >= 0 && currentBullet < maxAmmo)
        pivot?.RevolverBullets[currentBullet].gameObject.SetActive(true);
      --currentBullet;
    }

    private void TurnBarrel()
    {
      if (reloadingEnded || bullets.Value >= maxAmmo)
        return;
      angleDiff -= 60f;
    }

    private void RemoveAmmo()
    {
      --bullets.Value;
      if (storageAmmo.Count > 0)
      {
        --storageAmmo[0].Count;
        if (storageAmmo[0].Count <= 0)
          storageAmmo[0].Owner.Dispose();
      }
      RefreshStorageAmmo();
    }

    public void OnEnable()
    {
      ApplyVisibility();
      animator.SetTrigger("Triggers/RevolverRestore");
      AddListeners();
    }

    public void OnDisable() => RemoveListeners();

    public void Activate(bool geometryVisible)
    {
      WeaponVisible = true;
      animatorState.RevolverUnholster();
      Action unholsterEndEvent = WeaponUnholsterEndEvent;
      if (unholsterEndEvent != null)
        unholsterEndEvent();
      AddListeners();
      RefreshAmmoCount();
      isReloading = false;
      isShoting = false;
      if (item == null)
        return;
      CheckAmmo();
    }

    private void RefreshStorageAmmo()
    {
      storageAmmo.Clear();
      storageAmmo.AddRange(storage.Items.ToList().FindAll(x => x.Groups.Contains(StorableGroup.Ammo_Revolver) && x.Count > 0).Cast<StorableComponent>());
    }

    public void Shutdown()
    {
      RemoveListeners();
      animatorState.RevolverHolster();
      Action holsterStartEvent = WeaponHolsterStartEvent;
      if (holsterStartEvent != null)
        holsterStartEvent();
      isAiming = false;
      animatorState.RevolverAim(false);
    }

    private void AddListeners()
    {
      if (listenersAdded)
        return;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Fire, PunchListener);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Block, BlockListener);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Push, PushListener);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Reload, ReloadListener);
      listenersAdded = true;
    }

    private void RemoveListeners()
    {
      if (!listenersAdded)
        return;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Fire, PunchListener);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Block, BlockListener);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Push, PushListener);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Reload, ReloadListener);
      listenersAdded = false;
    }

    private bool PunchListener(GameActionType type, bool down)
    {
      RefreshAmmoCount();
      if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return false;
      if (isReloading)
      {
        reloadingCancelled = true;
        return true;
      }
      if (down && timeToLastPunch <= 0.0 && !isShoting)
      {
        if (bullets != null && bullets.Value > 0)
        {
          Shoot();
          return true;
        }
        CheckAmmo();
        if (StorageAmmoCount() == 0)
          Shoot();
      }
      return true;
    }

    private void Shoot()
    {
      RefreshAmmoCount();
      bool gunJam = WeaponUtility.ComputeGunJam(gameObject, durability);
      animatorState.RevolverShot(bullets.Value <= 0, gunJam);
      isShoting = true;
      if (durability != null && durability.Value <= 0.0)
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(item, ShotType.None, ReactionType.None, ShotSubtypeEnum.WeaponBroken);
      }
      if (bullets.Value <= 0 || gunJam)
        return;
      RemoveAmmo();
    }

    private bool PushListener(GameActionType type, bool down)
    {
      if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling || isReloading)
        return false;
      if (!down || timeToLastPunch > 0.0 || isShoting)
        return true;
      animatorState.RevolverPush();
      timeToLastPunch = ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerPunchCooldownTime;
      return true;
    }

    private bool BlockListener(GameActionType type, bool down)
    {
      if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return false;
      if (isReloading)
      {
        reloadingCancelled = true;
        return true;
      }
      isAiming = down;
      animatorState.RevolverAim(down);
      return true;
    }

    private bool ReloadListener(GameActionType type, bool down)
    {
      if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling || isReloading)
        return false;
      if (bullets.Value < bullets.MaxValue && StorageAmmoCount() > bullets.Value)
        BeginReloading();
      return true;
    }

    public void Update(IEntity target)
    {
      if (!PlayerUtility.IsPlayerCanControlling)
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

    public void Reset() => animatorState.ResetAnimator();

    public bool Validate(GameObject gameObject, IEntity item) => true;

    public void LateUpdate(IEntity target)
    {
      pivot.RevolverCylinder.localEulerAngles = new Vector3(0.0f, 0.0f, pivot.RevolverCylinder.localEulerAngles.z + angleDiff);
    }

    public void FixedUpdate(IEntity target)
    {
    }

    public void OnAnimatorEvent(string data)
    {
      if (data.StartsWith("Revolver.EndShot"))
      {
        isShoting = false;
        CheckAmmo();
      }
      if (data.StartsWith("Revolver.Shot"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(item, ShotType.Moderate, ReactionType.Uppercut, ShotSubtypeEnum.None);
        pivot?.RevolverFirePS?.Fire();
        item.GetComponent<StorableComponent>().Use();
      }
      if (data.StartsWith("Revolver.AimedShot"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(item, ShotType.Strong, ReactionType.Uppercut, ShotSubtypeEnum.None);
        pivot?.RevolverFirePS?.Fire();
        item.GetComponent<StorableComponent>().Use();
      }
      if (data.StartsWith("Revolver.Turn"))
        angleDiff += 60f;
      if (data.StartsWith("Revolver.Prepunch"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(item, ShotType.Prepunch, ReactionType.Right, ShotSubtypeEnum.None);
      }
      if (data.StartsWith("Revolver.Push"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(item, ShotType.Push, ReactionType.Right, ShotSubtypeEnum.None);
      }
      if (data.StartsWith("Revolver.EmptyBarrel"))
        EmptyBarrel();
      if (data.StartsWith("Revolver.StartedReloadingAmmo"))
        SetBulletVisible(true);
      if (data.StartsWith("Revolver.ReloadedAmmo"))
      {
        ReloadAmmo();
        RefreshAmmoCount();
        if (reloadingCancelled || bullets.Value >= bullets.MaxValue || StorageAmmoCount() <= bullets.Value)
          EndReloading();
      }
      if (data.StartsWith("Revolver.ReloadedLoopCycleEnded"))
        TurnBarrel();
      if (!data.StartsWith("Revolver.ReloadEnded"))
        return;
      isReloading = false;
      isShoting = false;
    }

    public void Reaction()
    {
      if (!isReloading)
        return;
      animatorState.RevolverCancelReload();
      isReloading = false;
    }
  }
}
