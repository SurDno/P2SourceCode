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

namespace Engine.Behaviours.Engines.Controllers;

public class PlayerRifleWeaponController : IWeaponController {
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
	private List<StorableComponent> storageAmmo = new();
	private IEntity item;
	private IParameter<int> bullets;
	private IParameter<float> durability;
	private bool isReloading;
	private bool reloadingCancelled;
	private bool isAiming;
	private float smoothedNormalizedSpeed;
	private float layerWeight;
	private bool bulletVisible;
	private bool listenersAdded;

	public event Action WeaponUnholsterEndEvent;

	public event Action WeaponHolsterStartEvent;

	public event Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

	bool IWeaponController.GeometryVisible {
		set {
			geometryVisible = value;
			ApplyVisibility();
			ApplyLayerWeight(geometryVisible ? 1f : 0.0f);
			if (!geometryVisible && isReloading) {
				animatorState.RifleRestore();
				isReloading = false;
			}

			isShoting = false;
		}
		get => geometryVisible;
	}

	private bool WeaponVisible {
		set {
			weaponVisible = value;
			ApplyVisibility();
		}
	}

	private void SetBulletVisible(bool visible) {
		bulletVisible = visible;
		ApplyBulletVisibility();
	}

	private void ApplyBulletVisibility() {
		pivot?.RifleAmmo?.SetActive(bulletVisible && geometryVisible);
	}

	protected void ApplyVisibility() {
		pivot.HandsGeometryVisible = geometryVisible;
		pivot.RifleGeometryVisible = geometryVisible && weaponVisible;
		ApplyBulletVisibility();
		ApplyLayerWeight(geometryVisible ? 1f : 0.0f);
	}

	protected void ApplyLayerWeight(float weight) {
		animatorState.RifleLayerWeight = weight;
		animatorState.RifleReactionLayerWeight = weight;
	}

	public void Initialise(IEntity entity, GameObject gameObject, Animator animator) {
		this.entity = entity;
		pivot = gameObject.GetComponent<PivotPlayer>();
		if (pivot == null)
			Debug.LogErrorFormat("{0} has no {1} unity component", gameObject.name, typeof(PivotPlayer).Name);
		else {
			this.gameObject = gameObject;
			this.animator = animator;
			playerEnemy = gameObject.GetComponent<PlayerEnemy>();
			controllerComponent = entity.GetComponent<ControllerComponent>();
			animatorState = PlayerAnimatorState.GetAnimatorState(animator);
			stamina = entity.GetComponent<ParametersComponent>().GetByName<float>(ParameterNameEnum.Stamina);
			if (entity == null)
				Debug.LogWarningFormat("{0} can't map entity", gameObject.name);
			else {
				detectable = (DetectableComponent)entity.GetComponent<IDetectableComponent>();
				if (detectable == null)
					Debug.LogWarningFormat("{0} doesn't have {1} engine component", gameObject.name,
						typeof(IDetectableComponent).Name);
				else
					storage = entity.GetComponent<StorageComponent>();
			}
		}
	}

	public IEntity GetItem() {
		return item;
	}

	public void SetItem(IEntity item) {
		this.item = item;
		var component = item.GetComponent<ParametersComponent>();
		if (component != null) {
			durability = component.GetByName<float>(ParameterNameEnum.Durability);
			bullets = component.GetByName<int>(ParameterNameEnum.Bullets);
		} else {
			durability = null;
			bullets = null;
		}
	}

	private int StorageAmmoCount() {
		var num = 0;
		foreach (var storableComponent in storageAmmo)
			num += storableComponent.Count;
		return num;
	}

	private void RefreshAmmoCount() {
		RefreshStorageAmmo();
		if (bullets == null)
			return;
		bullets.Value = Mathf.Min(StorageAmmoCount(), bullets.Value, bullets.MaxValue);
	}

	private void CheckAmmo() {
		if ((bullets != null && bullets.Value != 0) || StorageAmmoCount() <= 0)
			return;
		BeginReloading();
	}

	private void BeginReloading() {
		isReloading = true;
		reloadingCancelled = false;
		isAiming = false;
		animatorState.RifleAim(false);
		animatorState.RifleReload(true, durability != null && durability.Value < 0.30000001192092896);
		SetBulletVisible(true);
	}

	private void EndReloading() {
		animatorState.RifleReload(false);
	}

	private void ReloadAmmo() {
		bullets.Value = Mathf.Min(StorageAmmoCount(), bullets.Value + 1, bullets.MaxValue);
	}

	private void RemoveAmmo() {
		--bullets.Value;
		if (storageAmmo.Count > 0) {
			--storageAmmo[0].Count;
			if (storageAmmo[0].Count <= 0)
				storageAmmo[0].Owner.Dispose();
		}

		RefreshStorageAmmo();
	}

	public void OnEnable() {
		ApplyVisibility();
		animator.SetTrigger("Triggers/RifleRestore");
		AddListeners();
	}

	public void OnDisable() {
		RemoveListeners();
	}

	public void Activate(bool geometryVisible) {
		WeaponVisible = true;
		animatorState.RifleUnholster();
		var unholsterEndEvent = WeaponUnholsterEndEvent;
		if (unholsterEndEvent != null)
			unholsterEndEvent();
		AddListeners();
		RefreshAmmoCount();
		isReloading = false;
		isShoting = false;
		CheckAmmo();
	}

	private void AddListeners() {
		if (listenersAdded)
			return;
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Fire, PunchListener);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Block, BlockListener);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Push, PushListener);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Reload, ReloadListener);
		listenersAdded = true;
	}

	private void RemoveListeners() {
		if (!listenersAdded)
			return;
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Fire, PunchListener);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Block, BlockListener);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Push, PushListener);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Reload, ReloadListener);
		listenersAdded = false;
	}

	private void RefreshStorageAmmo() {
		storageAmmo.Clear();
		storageAmmo.AddRange(storage.Items.ToList()
			.FindAll(x => x.Groups.Contains(StorableGroup.Ammo_Rifle) && x.Count > 0).Cast<StorableComponent>());
	}

	public void Shutdown() {
		RemoveListeners();
		animatorState.RifleHolster();
		var holsterStartEvent = WeaponHolsterStartEvent;
		if (holsterStartEvent != null)
			holsterStartEvent();
		isAiming = false;
		animatorState.RifleAim(false);
	}

	private bool PunchListener(GameActionType type, bool down) {
		RefreshAmmoCount();
		if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
			return false;
		if (isReloading) {
			reloadingCancelled = true;
			return true;
		}

		if (down && timeToLastPunch <= 0.0 && !isShoting) {
			if (bullets != null) {
				Shoot();
				return true;
			}

			CheckAmmo();
		}

		return true;
	}

	private void Shoot() {
		RefreshAmmoCount();
		var gunJam = WeaponUtility.ComputeGunJam(gameObject, durability);
		animatorState.RifleShot(bullets.Value, gunJam);
		isShoting = true;
		if (durability != null && durability.Value <= 0.0) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent != null)
				weaponShootEvent(item, ShotType.None, ReactionType.None, ShotSubtypeEnum.WeaponBroken);
		}

		if (gunJam)
			return;
		RemoveAmmo();
		SetBulletVisible(true);
	}

	private bool PushListener(GameActionType type, bool down) {
		if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling ||
		    isReloading)
			return false;
		if (!down || timeToLastPunch > 0.0 || isShoting)
			return true;
		animatorState.RiflePush();
		timeToLastPunch = ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerPunchCooldownTime;
		return true;
	}

	private bool BlockListener(GameActionType type, bool down) {
		if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
			return false;
		if (isReloading) {
			reloadingCancelled = true;
			return false;
		}

		isAiming = down;
		animatorState.RifleAim(down);
		return true;
	}

	private bool ReloadListener(GameActionType type, bool down) {
		if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling ||
		    isReloading)
			return false;
		if (bullets.Value < bullets.MaxValue && StorageAmmoCount() > bullets.Value)
			BeginReloading();
		return true;
	}

	public void Update(IEntity target) {
		if (!PlayerUtility.IsPlayerCanControlling)
			return;
		var target1 = 0.0f;
		var flag = controllerComponent != null && controllerComponent.IsRun.Value;
		if (controllerComponent.IsWalk.Value)
			target1 = (flag ? 1f : 0.5f) * controllerComponent.WalkModifier.Value;
		smoothedNormalizedSpeed = Mathf.MoveTowards(smoothedNormalizedSpeed, target1, Time.deltaTime / 1f);
		animatorState.WalkSpeed = smoothedNormalizedSpeed;
		if (timeToLastPunch <= 0.0)
			return;
		timeToLastPunch -= Time.deltaTime;
	}

	public void UpdateSilent(IEntity target) {
		if (!InstanceByRequest<EngineApplication>.Instance.IsPaused)
			;
	}

	public void Reset() {
		animatorState.ResetAnimator();
	}

	public bool Validate(GameObject gameObject, IEntity item) {
		return true;
	}

	public void LateUpdate(IEntity target) { }

	public void FixedUpdate(IEntity target) { }

	public void OnAnimatorEvent(string data) {
		if (data.StartsWith("Rifle.EndShot")) {
			isShoting = false;
			CheckAmmo();
		}

		if (data.StartsWith("Rifle.Shot")) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent != null)
				weaponShootEvent(item, ShotType.Moderate, ReactionType.Uppercut, ShotSubtypeEnum.None);
			pivot?.RifleShot?.Fire();
			item.GetComponent<StorableComponent>().Use();
		}

		if (data.StartsWith("Rifle.AimedShot")) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent != null)
				weaponShootEvent(item, ShotType.Strong, ReactionType.Uppercut, ShotSubtypeEnum.None);
			pivot?.RifleShot?.Fire();
			item.GetComponent<StorableComponent>().Use();
		}

		if (data.StartsWith("Rifle.Push")) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent != null)
				weaponShootEvent(item, ShotType.Push, ReactionType.None, ShotSubtypeEnum.None);
		}

		if (data.StartsWith("Rifle.ReloadedAmmo")) {
			ReloadAmmo();
			RefreshAmmoCount();
			if (reloadingCancelled || bullets.Value >= bullets.MaxValue || StorageAmmoCount() <= bullets.Value)
				EndReloading();
		}

		if (!data.StartsWith("Rifle.ReloadEnded"))
			return;
		isReloading = false;
		isShoting = false;
		SetBulletVisible(false);
	}

	public void Reaction() {
		if (!isReloading)
			return;
		animatorState.RifleCancelReload();
		reloadingCancelled = true;
		isReloading = false;
	}
}