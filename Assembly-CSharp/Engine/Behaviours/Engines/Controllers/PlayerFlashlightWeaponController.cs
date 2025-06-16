// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Controllers.PlayerFlashlightWeaponController
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
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
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
    private float blockStance = 0.0f;
    private float realBlockStance = 0.0f;
    private float realStamina = 0.0f;
    private IParameter<TimeSpan> WorkEndTime;
    private IParameter<TimeSpan> WorkTime;
    private float smoothedNormalizedSpeed = 0.0f;
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
        this.geometryVisible = value;
        this.ApplyVisibility();
        this.ApplyLayerWeight(this.geometryVisible ? 1f : 0.0f);
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

    private void ApplyVisibility()
    {
      this.pivot.HandsGeometryVisible = this.geometryVisible;
      this.pivot.FlashlightGeometryVisible = this.geometryVisible && this.weaponVisible;
      this.ApplyLayerWeight(this.geometryVisible ? 1f : 0.0f);
      if (!this.geometryVisible)
        this.flashlightOn.Value = false;
      this.pivot.FlashlightFire.SetActive(this.geometryVisible);
      if (!this.geometryVisible || !this.weaponVisible)
        return;
      this.TrySwitchOn();
    }

    public void OnEnable()
    {
      this.ApplyVisibility();
      this.animator.SetTrigger("Triggers/FlashlightRestore");
      this.AddListeners();
    }

    public void OnDisable()
    {
      this.SwitchLight(false);
      this.RemoveListeners();
    }

    private void ApplyLayerWeight(float weight)
    {
      this.animatorState.FlashlightLayerWeight = weight;
      this.animatorState.FlashlightReactionLayerWeight = weight;
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
        this.controllerComponent = entity.GetComponent<ControllerComponent>();
        this.animatorState = PlayerAnimatorState.GetAnimatorState(animator);
        if (entity == null)
        {
          Debug.LogWarningFormat("{0} can't map entity", (object) gameObject.name);
        }
        else
        {
          this.lowStamina = entity.GetComponent<ParametersComponent>().GetByName<bool>(ParameterNameEnum.LowStamina);
          this.flashlightOn = entity.GetComponent<ParametersComponent>().GetByName<bool>(ParameterNameEnum.Flashlight);
          this.storage = entity.GetComponent<StorageComponent>();
          this.timeService = ServiceLocator.GetService<TimeService>();
        }
      }
    }

    public IEntity GetItem() => this.item;

    public void SetItem(IEntity item)
    {
      this.item = item;
      this.TrySwitchOn();
    }

    private void TrySwitchOn()
    {
      if (this.item == null)
        return;
      ParametersComponent component = this.item.GetComponent<ParametersComponent>();
      if (component != null)
      {
        this.WorkTime = component.GetByName<TimeSpan>(ParameterNameEnum.WorkTime);
        this.WorkEndTime = component.GetByName<TimeSpan>(ParameterNameEnum.WorkEndTime);
        this.IsOn = this.WorkEndTime.Value > this.timeService.GameTime;
        this.SwitchLight(this.IsOn);
        if (!this.IsOn)
        {
          Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
          if (weaponShootEvent != null)
            weaponShootEvent(this.item, ShotType.None, ReactionType.Right, ShotSubtypeEnum.FireLamp);
        }
      }
    }

    public void Activate(bool geometryVisible)
    {
      this.WeaponVisible = true;
      this.pivot.FlashlightEffect.SetActive(true);
      this.animatorState.FlashlightUnholster();
      Action unholsterEndEvent = this.WeaponUnholsterEndEvent;
      if (unholsterEndEvent != null)
        unholsterEndEvent();
      this.AddListeners();
      this.RefreshStorageAmmo();
      this.flashlightOn.Value = this.IsOn;
    }

    private void RefreshStorageAmmo()
    {
      this.storageAmmo.Clear();
      this.storageAmmo.AddRange(this.storage.Items.ToList<IStorableComponent>().FindAll((Predicate<IStorableComponent>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Fuel_Lamp) && x.Count > 0)).Cast<StorableComponent>());
    }

    private int StorageAmmoCount()
    {
      int num = 0;
      foreach (StorableComponent storableComponent in this.storageAmmo)
        num += storableComponent.Count;
      return num;
    }

    private void RemoveAmmo()
    {
      if (this.storageAmmo.Count > 0)
      {
        --this.storageAmmo[0].Count;
        if (this.storageAmmo[0].Count <= 0)
          this.storageAmmo[0].Owner.Dispose();
      }
      this.RefreshStorageAmmo();
    }

    public void Shutdown()
    {
      this.RemoveListeners();
      this.pivot.FlashlightEffect.SetActive(false);
      this.animatorState.FlashlightHolster();
      Action holsterStartEvent = this.WeaponHolsterStartEvent;
      if (holsterStartEvent != null)
        holsterStartEvent();
      this.blockStance = 0.0f;
    }

    private void AddListeners()
    {
      if (this.listenersAdded)
        return;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Fire, new GameActionHandle(this.PunchListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Block, new GameActionHandle(this.BlockListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Flashlight, new GameActionHandle(this.LightListener));
      this.listenersAdded = true;
    }

    private void RemoveListeners()
    {
      if (!this.listenersAdded)
        return;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Fire, new GameActionHandle(this.PunchListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Block, new GameActionHandle(this.BlockListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Flashlight, new GameActionHandle(this.LightListener));
      this.listenersAdded = false;
    }

    public void Reset() => this.animatorState.ResetAnimator();

    public bool Validate(GameObject gameObject, IEntity item) => true;

    public void Update(IEntity target)
    {
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
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

    public void LateUpdate(IEntity target)
    {
    }

    public void FixedUpdate(IEntity target)
    {
      this.realBlockStance = Mathf.MoveTowards(this.realBlockStance, this.blockStance, Time.fixedDeltaTime / ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerBlockStanceTime);
      this.animatorState.BlockStance = SmoothUtility.Smooth12(this.realBlockStance);
      this.realStamina = Mathf.MoveTowards(this.realStamina, this.lowStamina.Value ? 0.0f : 1f, Time.fixedDeltaTime / ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerBlockStanceTime);
      this.animatorState.Stamina = this.realStamina;
      if ((UnityEngine.Object) this.playerEnemy != (UnityEngine.Object) null)
        this.playerEnemy.BlockStance = (double) this.realBlockStance >= 0.5;
      bool isOn = this.WorkEndTime.Value > this.timeService.GameTime;
      if (isOn || !this.IsOn)
        return;
      this.SwitchLight(isOn);
    }

    private bool LightListener(GameActionType type, bool down)
    {
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling || this.IsOn)
        return false;
      if (down)
      {
        this.RefreshStorageAmmo();
        if (this.StorageAmmoCount() <= 0)
        {
          Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
          if (weaponShootEvent != null)
            weaponShootEvent(this.item, ShotType.None, ReactionType.Right, ShotSubtypeEnum.NoMatches);
          return false;
        }
        this.FireLamp();
      }
      return true;
    }

    private void FireLamp()
    {
      if (this.StorageAmmoCount() <= 0)
        return;
      TimeSpan timeSpan = this.WorkTime.MinValue;
      double totalSeconds1 = timeSpan.TotalSeconds;
      timeSpan = this.WorkTime.MaxValue;
      double totalSeconds2 = timeSpan.TotalSeconds;
      this.WorkEndTime.Value = this.timeService.GameTime + TimeSpan.FromSeconds((double) UnityEngine.Random.Range((float) totalSeconds1, (float) totalSeconds2));
      this.SwitchLight(true);
      this.RemoveAmmo();
      Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent != null)
        weaponShootEvent(this.item, ShotType.Light, ReactionType.None, ShotSubtypeEnum.None);
    }

    private void SwitchLight(bool isOn)
    {
      this.IsOn = isOn;
      ParticleSystem componentInChildren = this.pivot.FlashlightEffect.GetComponentInChildren<ParticleSystem>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      {
        if (isOn)
          componentInChildren.Play();
        else
          componentInChildren.Stop();
      }
      this.pivot.FlashlightLight.SetActive(isOn);
      this.flashlightOn.Value = this.IsOn;
    }

    private bool PunchListener(GameActionType type, bool down)
    {
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return false;
      if (down && (double) this.timeToLastPunch <= 0.0)
      {
        this.blockStance = 0.0f;
        if (this.lowStamina.Value)
          this.animatorState.FlashlightPunchLowStamina();
        else
          this.animatorState.FlashlightPunch();
        this.timeToLastPunch = ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerPunchCooldownTime;
        return true;
      }
      if (!down && (double) this.timeToLastPunch <= 0.0)
      {
        this.blockStance = 0.0f;
        this.realBlockStance = 0.0f;
        this.animatorState.BlockStance = SmoothUtility.Smooth12(this.realBlockStance);
      }
      return true;
    }

    private bool BlockListener(GameActionType type, bool down)
    {
      if (this.entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
        return false;
      this.blockStance = down ? 1f : 0.0f;
      return true;
    }

    public void OnAnimatorEvent(string data)
    {
      if (data.StartsWith("Flashlight.Punch"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(this.item, ShotType.Moderate, ReactionType.Right, ShotSubtypeEnum.None);
      }
      else if (data.StartsWith("Flashlight.LowStamina"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(this.item, ShotType.LowStamina, ReactionType.Right, ShotSubtypeEnum.None);
      }
      else if (data.StartsWith("Flashlight.PrePunch"))
      {
        Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent == null)
          return;
        weaponShootEvent(this.item, ShotType.Prepunch, ReactionType.Right, ShotSubtypeEnum.None);
      }
      else
      {
        if (!data.StartsWith("Flashlight.Light"))
          return;
        this.FireLamp();
      }
    }

    public void Reaction()
    {
    }
  }
}
