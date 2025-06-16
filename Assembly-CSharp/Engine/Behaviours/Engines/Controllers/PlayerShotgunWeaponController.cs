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
  public class PlayerShotgunWeaponController : IWeaponController
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
    private List<StorableComponent> storageAmmo = new List<StorableComponent>();
    private IEntity item;
    private IParameter<int> bullets;
    private IParameter<float> durability;
    private bool isReloading;
    private bool reloadingCancelled;
    private bool isAiming;
    private int maxAmmo = 2;
    private float smoothedNormalizedSpeed;
    private float layerWeight;
    private bool bulletVisible;
    private bool[] bulletsInserted;
    private bool listenersAdded;

    public event Action WeaponUnholsterEndEvent;

    public event Action WeaponHolsterStartEvent;

    public event Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

    bool IWeaponController.GeometryVisible
    {
      set
      {
        geometryVisible = value;
        ApplyVisibility();
        ApplyBulletsVisibility();
        if (!geometryVisible && isReloading)
        {
          animatorState.ShotgunRestore();
          isReloading = false;
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
        ApplyBulletsVisibility();
      }
    }

    private void SetBulletVisible(bool visible)
    {
      bulletVisible = visible;
      ApplyBulletVisibility();
    }

    private void ApplyBulletVisibility()
    {
      pivot?.ShotgunAmmo?.SetActive(bulletVisible && geometryVisible);
    }

    private void ApplyBulletsVisibility()
    {
      Transform[] shotgunAmmos = pivot?.ShotgunAmmos;
      for (int index = 0; index < shotgunAmmos.Length; ++index)
        shotgunAmmos[index]?.gameObject?.SetActive(weaponVisible && geometryVisible && bulletsInserted[index]);
    }

    protected void ApplyVisibility()
    {
      pivot.HandsGeometryVisible = geometryVisible;
      pivot.ShotgunGeometryVisible = geometryVisible && weaponVisible;
      ApplyBulletVisibility();
      ApplyLayerWeight(geometryVisible ? 1f : 0.0f);
    }

    protected void ApplyLayerWeight(float weight)
    {
      animatorState.ShotgunLayerWeight = weight;
      animatorState.ShotgunReactionLayerWeight = weight;
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
        bulletsInserted = new bool[maxAmmo];
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
      if (bullets == null || bullets.Value == 0)
      {
        if (StorageAmmoCount() <= 0)
          return;
        BeginReloading();
      }
      else
      {
        for (int index = 0; index < bullets.Value; ++index)
        {
          if (index < bulletsInserted.Length)
            bulletsInserted[index] = true;
        }
      }
    }

    private void BeginReloading()
    {
      isReloading = true;
      reloadingCancelled = false;
      isAiming = false;
      animatorState.ShotgunAim(false);
      bool flag = durability != null && durability.Value < 0.30000001192092896;
      animatorState.ShotgunReload(true, bullets.Value);
      SetBulletVisible(true);
    }

    private void EndReloading() => animatorState.ShotgunReload(false);

    private void ReloadAmmo()
    {
      bullets.Value = Mathf.Min(StorageAmmoCount(), bullets.Value + 1, bullets.MaxValue);
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
      animator.SetTrigger("Triggers/ShotgunRestore");
      AddListeners();
    }

    public void OnDisable() => RemoveListeners();

    public void Activate(bool geometryVisible)
    {
      WeaponVisible = true;
      animatorState.ShotgunUnholster();
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
      storageAmmo.AddRange(storage.Items.ToList().FindAll(x => x.Groups.Contains(StorableGroup.Ammo_Shotgun) && x.Count > 0).Cast<StorableComponent>());
    }

    public void Shutdown()
    {
      RemoveListeners();
      animatorState.ShotgunHolster();
      Action holsterStartEvent = WeaponHolsterStartEvent;
      if (holsterStartEvent != null)
        holsterStartEvent();
      isAiming = false;
      animatorState.ShotgunAim(false);
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
      animatorState.ShotgunShot(bullets.Value <= 0, gunJam);
      isShoting = true;
      if (durability != null && durability.Value <= 0.0)
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(item, ShotType.None, ReactionType.None, ShotSubtypeEnum.WeaponBroken);
      }
      if (gunJam)
        return;
      RemoveAmmo();
    }

    private bool PushListener(GameActionType type, bool down)
    {
      if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling || isReloading)
        return false;
      if (!down || timeToLastPunch > 0.0 || isShoting)
        return true;
      animatorState.ShotgunPush();
      timeToLastPunch = ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerPunchCooldownTime;
      return true;
    }

    private bool BlockListener(GameActionType type, bool down)
    {
      if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return false;
      isAiming = down;
      animatorState.ShotgunAim(down);
      return true;
    }

    private bool ReloadListener(GameActionType type, bool down)
    {
      if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling || isReloading || isShoting)
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
    }

    public void FixedUpdate(IEntity target)
    {
    }

    public void OnAnimatorEvent(string data)
    {
      if (data.StartsWith("Shotgun.EndShot"))
      {
        isShoting = false;
        CheckAmmo();
      }
      if (data.StartsWith("Shotgun.Shot"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(item, ShotType.Moderate, ReactionType.Uppercut, ShotSubtypeEnum.None);
        pivot?.ShotgunShot?.Fire();
        item.GetComponent<StorableComponent>().Use();
      }
      if (data.StartsWith("Shotgun.AimedShot"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(item, ShotType.Strong, ReactionType.Uppercut, ShotSubtypeEnum.None);
        pivot?.ShotgunShot?.Fire();
        item.GetComponent<StorableComponent>().Use();
      }
      if (data.StartsWith("Shotgun.Push"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(item, ShotType.Push, ReactionType.None, ShotSubtypeEnum.None);
      }
      if (data.StartsWith("Shotgun.ReloadedAmmo"))
      {
        ReloadAmmo();
        RefreshAmmoCount();
        ShowAmmo(bullets.Value - 1, true);
        SetBulletVisible(false);
        if (reloadingCancelled || bullets.Value >= bullets.MaxValue || StorageAmmoCount() <= bullets.Value)
          EndReloading();
      }
      if (data.StartsWith("Shotgun.ShowInsertingAmmo"))
        SetBulletVisible(true);
      if (data.StartsWith("Shotgun.HideAmmo0"))
        ShowAmmo(0, false);
      if (data.StartsWith("Shotgun.HideAmmo1"))
        ShowAmmo(1, false);
      if (!data.StartsWith("Shotgun.ReloadEnded"))
        return;
      isReloading = false;
      isShoting = false;
      SetBulletVisible(false);
    }

    private void ShowAmmo(int index, bool show)
    {
      if (geometryVisible || !show)
        bulletsInserted[index] = show;
      ApplyBulletsVisibility();
    }

    public void Reaction()
    {
      if (!isReloading)
        return;
      animatorState.ShotgunCancelReload();
      reloadingCancelled = true;
      isReloading = false;
    }
  }
}
