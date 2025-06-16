// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Controllers.PlayerShotgunWeaponController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
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
    private float timeToLastPunch = 0.0f;
    private bool isShoting;
    private StorageComponent storage;
    private List<StorableComponent> storageAmmo = new List<StorableComponent>();
    private IEntity item;
    private IParameter<int> bullets;
    private IParameter<float> durability;
    private bool isReloading = false;
    private bool reloadingCancelled = false;
    private bool isAiming = false;
    private int maxAmmo = 2;
    private float smoothedNormalizedSpeed = 0.0f;
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
        this.geometryVisible = value;
        this.ApplyVisibility();
        this.ApplyBulletsVisibility();
        if (!this.geometryVisible && this.isReloading)
        {
          this.animatorState.ShotgunRestore();
          this.isReloading = false;
        }
        this.isShoting = false;
      }
      get => this.geometryVisible;
    }

    private bool WeaponVisible
    {
      set
      {
        this.weaponVisible = value;
        this.ApplyVisibility();
        this.ApplyBulletsVisibility();
      }
    }

    private void SetBulletVisible(bool visible)
    {
      this.bulletVisible = visible;
      this.ApplyBulletVisibility();
    }

    private void ApplyBulletVisibility()
    {
      this.pivot?.ShotgunAmmo?.SetActive(this.bulletVisible && this.geometryVisible);
    }

    private void ApplyBulletsVisibility()
    {
      Transform[] shotgunAmmos = this.pivot?.ShotgunAmmos;
      for (int index = 0; index < shotgunAmmos.Length; ++index)
        shotgunAmmos[index]?.gameObject?.SetActive(this.weaponVisible && this.geometryVisible && this.bulletsInserted[index]);
    }

    protected void ApplyVisibility()
    {
      this.pivot.HandsGeometryVisible = this.geometryVisible;
      this.pivot.ShotgunGeometryVisible = this.geometryVisible && this.weaponVisible;
      this.ApplyBulletVisibility();
      this.ApplyLayerWeight(this.geometryVisible ? 1f : 0.0f);
    }

    protected void ApplyLayerWeight(float weight)
    {
      this.animatorState.ShotgunLayerWeight = weight;
      this.animatorState.ShotgunReactionLayerWeight = weight;
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
        this.bulletsInserted = new bool[this.maxAmmo];
        this.playerEnemy = gameObject.GetComponent<PlayerEnemy>();
        this.controllerComponent = entity.GetComponent<ControllerComponent>();
        this.animatorState = PlayerAnimatorState.GetAnimatorState(animator);
        this.stamina = entity.GetComponent<ParametersComponent>().GetByName<float>(ParameterNameEnum.Stamina);
        if (entity == null)
        {
          Debug.LogWarningFormat("{0} can't map entity", (object) gameObject.name);
        }
        else
        {
          this.detectable = (DetectableComponent) entity.GetComponent<IDetectableComponent>();
          if (this.detectable == null)
            Debug.LogWarningFormat("{0} doesn't have {1} engine component", (object) gameObject.name, (object) typeof (IDetectableComponent).Name);
          else
            this.storage = entity.GetComponent<StorageComponent>();
        }
      }
    }

    public IEntity GetItem() => this.item;

    public void SetItem(IEntity item)
    {
      this.item = item;
      ParametersComponent component = item.GetComponent<ParametersComponent>();
      if (component != null)
      {
        this.durability = component.GetByName<float>(ParameterNameEnum.Durability);
        this.bullets = component.GetByName<int>(ParameterNameEnum.Bullets);
      }
      else
      {
        this.durability = (IParameter<float>) null;
        this.bullets = (IParameter<int>) null;
      }
      if (!this.geometryVisible)
        return;
      this.CheckAmmo();
    }

    private int StorageAmmoCount()
    {
      int num = 0;
      foreach (StorableComponent storableComponent in this.storageAmmo)
        num += storableComponent.Count;
      return num;
    }

    private void RefreshAmmoCount()
    {
      this.RefreshStorageAmmo();
      if (this.bullets == null)
        return;
      this.bullets.Value = Mathf.Min(new int[3]
      {
        this.StorageAmmoCount(),
        this.bullets.Value,
        this.bullets.MaxValue
      });
    }

    private void CheckAmmo()
    {
      if (this.bullets == null || this.bullets.Value == 0)
      {
        if (this.StorageAmmoCount() <= 0)
          return;
        this.BeginReloading();
      }
      else
      {
        for (int index = 0; index < this.bullets.Value; ++index)
        {
          if (index < this.bulletsInserted.Length)
            this.bulletsInserted[index] = true;
        }
      }
    }

    private void BeginReloading()
    {
      this.isReloading = true;
      this.reloadingCancelled = false;
      this.isAiming = false;
      this.animatorState.ShotgunAim(false);
      bool flag = this.durability != null && (double) this.durability.Value < 0.30000001192092896;
      this.animatorState.ShotgunReload(true, this.bullets.Value);
      this.SetBulletVisible(true);
    }

    private void EndReloading() => this.animatorState.ShotgunReload(false);

    private void ReloadAmmo()
    {
      this.bullets.Value = Mathf.Min(new int[3]
      {
        this.StorageAmmoCount(),
        this.bullets.Value + 1,
        this.bullets.MaxValue
      });
    }

    private void RemoveAmmo()
    {
      --this.bullets.Value;
      if (this.storageAmmo.Count > 0)
      {
        --this.storageAmmo[0].Count;
        if (this.storageAmmo[0].Count <= 0)
          this.storageAmmo[0].Owner.Dispose();
      }
      this.RefreshStorageAmmo();
    }

    public void OnEnable()
    {
      this.ApplyVisibility();
      this.animator.SetTrigger("Triggers/ShotgunRestore");
      this.AddListeners();
    }

    public void OnDisable() => this.RemoveListeners();

    public void Activate(bool geometryVisible)
    {
      this.WeaponVisible = true;
      this.animatorState.ShotgunUnholster();
      Action unholsterEndEvent = this.WeaponUnholsterEndEvent;
      if (unholsterEndEvent != null)
        unholsterEndEvent();
      this.AddListeners();
      this.RefreshAmmoCount();
      this.isReloading = false;
      this.isShoting = false;
      if (this.item == null)
        return;
      this.CheckAmmo();
    }

    private void RefreshStorageAmmo()
    {
      this.storageAmmo.Clear();
      this.storageAmmo.AddRange(this.storage.Items.ToList<IStorableComponent>().FindAll((Predicate<IStorableComponent>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Ammo_Shotgun) && x.Count > 0)).Cast<StorableComponent>());
    }

    public void Shutdown()
    {
      this.RemoveListeners();
      this.animatorState.ShotgunHolster();
      Action holsterStartEvent = this.WeaponHolsterStartEvent;
      if (holsterStartEvent != null)
        holsterStartEvent();
      this.isAiming = false;
      this.animatorState.ShotgunAim(false);
    }

    private void AddListeners()
    {
      if (this.listenersAdded)
        return;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Fire, new GameActionHandle(this.PunchListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Block, new GameActionHandle(this.BlockListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Push, new GameActionHandle(this.PushListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Reload, new GameActionHandle(this.ReloadListener));
      this.listenersAdded = true;
    }

    private void RemoveListeners()
    {
      if (!this.listenersAdded)
        return;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Fire, new GameActionHandle(this.PunchListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Block, new GameActionHandle(this.BlockListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Push, new GameActionHandle(this.PushListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Reload, new GameActionHandle(this.ReloadListener));
      this.listenersAdded = false;
    }

    private bool PunchListener(GameActionType type, bool down)
    {
      this.RefreshAmmoCount();
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return false;
      if (this.isReloading)
      {
        this.reloadingCancelled = true;
        return true;
      }
      if (down && (double) this.timeToLastPunch <= 0.0 && !this.isShoting)
      {
        if (this.bullets != null && this.bullets.Value > 0)
        {
          this.Shoot();
          return true;
        }
        this.CheckAmmo();
        if (this.StorageAmmoCount() == 0)
          this.Shoot();
      }
      return true;
    }

    private void Shoot()
    {
      this.RefreshAmmoCount();
      bool gunJam = WeaponUtility.ComputeGunJam(this.gameObject, this.durability);
      this.animatorState.ShotgunShot(this.bullets.Value <= 0, gunJam);
      this.isShoting = true;
      if (this.durability != null && (double) this.durability.Value <= 0.0)
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(this.item, ShotType.None, ReactionType.None, ShotSubtypeEnum.WeaponBroken);
      }
      if (gunJam)
        return;
      this.RemoveAmmo();
    }

    private bool PushListener(GameActionType type, bool down)
    {
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling || this.isReloading)
        return false;
      if (!down || (double) this.timeToLastPunch > 0.0 || this.isShoting)
        return true;
      this.animatorState.ShotgunPush();
      this.timeToLastPunch = ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerPunchCooldownTime;
      return true;
    }

    private bool BlockListener(GameActionType type, bool down)
    {
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return false;
      this.isAiming = down;
      this.animatorState.ShotgunAim(down);
      return true;
    }

    private bool ReloadListener(GameActionType type, bool down)
    {
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling || this.isReloading || this.isShoting)
        return false;
      if (this.bullets.Value < this.bullets.MaxValue && this.StorageAmmoCount() > this.bullets.Value)
        this.BeginReloading();
      return true;
    }

    public void Update(IEntity target)
    {
      if (!PlayerUtility.IsPlayerCanControlling)
        return;
      float target1 = 0.0f;
      bool flag = this.controllerComponent != null && this.controllerComponent.IsRun.Value;
      if (this.controllerComponent.IsWalk.Value)
        target1 = (flag ? 1f : 0.5f) * this.controllerComponent.WalkModifier.Value;
      this.smoothedNormalizedSpeed = Mathf.MoveTowards(this.smoothedNormalizedSpeed, target1, Time.deltaTime / 1f);
      this.animatorState.WalkSpeed = this.smoothedNormalizedSpeed;
      if ((double) this.timeToLastPunch <= 0.0)
        return;
      this.timeToLastPunch -= Time.deltaTime;
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
    }

    public void OnAnimatorEvent(string data)
    {
      if (data.StartsWith("Shotgun.EndShot"))
      {
        this.isShoting = false;
        this.CheckAmmo();
      }
      if (data.StartsWith("Shotgun.Shot"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(this.item, ShotType.Moderate, ReactionType.Uppercut, ShotSubtypeEnum.None);
        this.pivot?.ShotgunShot?.Fire();
        this.item.GetComponent<StorableComponent>().Use();
      }
      if (data.StartsWith("Shotgun.AimedShot"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(this.item, ShotType.Strong, ReactionType.Uppercut, ShotSubtypeEnum.None);
        this.pivot?.ShotgunShot?.Fire();
        this.item.GetComponent<StorableComponent>().Use();
      }
      if (data.StartsWith("Shotgun.Push"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent != null)
          weaponShootEvent(this.item, ShotType.Push, ReactionType.None, ShotSubtypeEnum.None);
      }
      if (data.StartsWith("Shotgun.ReloadedAmmo"))
      {
        this.ReloadAmmo();
        this.RefreshAmmoCount();
        this.ShowAmmo(this.bullets.Value - 1, true);
        this.SetBulletVisible(false);
        if (this.reloadingCancelled || this.bullets.Value >= this.bullets.MaxValue || this.StorageAmmoCount() <= this.bullets.Value)
          this.EndReloading();
      }
      if (data.StartsWith("Shotgun.ShowInsertingAmmo"))
        this.SetBulletVisible(true);
      if (data.StartsWith("Shotgun.HideAmmo0"))
        this.ShowAmmo(0, false);
      if (data.StartsWith("Shotgun.HideAmmo1"))
        this.ShowAmmo(1, false);
      if (!data.StartsWith("Shotgun.ReloadEnded"))
        return;
      this.isReloading = false;
      this.isShoting = false;
      this.SetBulletVisible(false);
    }

    private void ShowAmmo(int index, bool show)
    {
      if (this.geometryVisible || !show)
        this.bulletsInserted[index] = show;
      this.ApplyBulletsVisibility();
    }

    public void Reaction()
    {
      if (!this.isReloading)
        return;
      this.animatorState.ShotgunCancelReload();
      this.reloadingCancelled = true;
      this.isReloading = false;
    }
  }
}
