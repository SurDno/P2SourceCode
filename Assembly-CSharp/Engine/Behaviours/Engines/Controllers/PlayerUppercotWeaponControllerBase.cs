using System;
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
using UnityEngine;

namespace Engine.Behaviours.Engines.Controllers;

public abstract class PlayerUppercotWeaponControllerBase : IWeaponController {
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
	private float timeToLastPunch;
	private float blockStance;
	private float realBlockStance;
	private float realStamina;
	private bool lastPunchWasByLeftHand;
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
	private bool isBlocking;
	private bool listenersAdded;
	private float smoothedNormalizedSpeed;
	private float layerWeight;

	public event Action WeaponUnholsterEndEvent;

	public event Action WeaponHolsterStartEvent;

	public event Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

	protected abstract void ApplyVisibility();

	protected abstract void ApplyLayerWeight(float weight);

	protected abstract WeaponKind WeaponKind { get; }

	protected abstract string Prefix { get; }

	protected abstract bool SupportsLowStaminaPunch { get; }

	bool IWeaponController.GeometryVisible {
		set {
			geometryVisible = value;
			ApplyVisibility();
		}
		get => geometryVisible;
	}

	private bool WeaponVisible {
		set {
			weaponVisible = value;
			ApplyVisibility();
		}
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
			characterController = gameObject.GetComponent<CharacterController>();
			controllerComponent = entity.GetComponent<ControllerComponent>();
			animatorState = PlayerAnimatorState.GetAnimatorState(animator);
			lowStamina = entity.GetComponent<ParametersComponent>().GetByName<bool>(ParameterNameEnum.LowStamina);
			stealth = entity.GetComponent<ParametersComponent>().GetByName<bool>(ParameterNameEnum.Stealth);
			if (entity == null)
				Debug.LogWarningFormat("{0} can't map entity", gameObject.name);
			else {
				detectable = (DetectableComponent)entity.GetComponent<IDetectableComponent>();
				if (detectable == null)
					Debug.LogWarningFormat("{0} doesn't have {1} engine component", gameObject.name,
						typeof(IDetectableComponent).Name);
				else {
					WeaponPunchString = Prefix + ".Punch";
					WeaponPunchLowStaminaString = Prefix + ".PunchLowStamina";
					WeaponPrepunchString = Prefix + ".Prepunch";
					WeaponPushString = Prefix + ".Push";
					WeaponUppercutString = Prefix + ".Uppercut";
					WeaponBackstabString = Prefix + ".Backstab";
					WeaponSpecialPrepunchString = Prefix + ".SpecialPrepunch";
				}
			}
		}
	}

	public IEntity GetItem() {
		return item;
	}

	public virtual void SetItem(IEntity item) {
		this.item = item;
		var component = item.GetComponent<ParametersComponent>();
		if (component == null)
			return;
		durability = component.GetByName<float>(ParameterNameEnum.Durability);
	}

	public void OnEnable() {
		ApplyVisibility();
		animator.SetTrigger("Triggers/Restore");
		AddListeners();
	}

	public void OnDisable() {
		blockStance = 0.0f;
		RemoveListeners();
	}

	public void Activate(bool geometryVisible) {
		WeaponVisible = true;
		animatorState.Unholster();
		var unholsterEndEvent = WeaponUnholsterEndEvent;
		if (unholsterEndEvent != null)
			unholsterEndEvent();
		AddListeners();
	}

	public void Shutdown() {
		var holsterStartEvent = WeaponHolsterStartEvent;
		if (holsterStartEvent != null)
			holsterStartEvent();
		RemoveListeners();
		animatorState.Holster();
		blockStance = 0.0f;
		playerEnemy.BlockStance = false;
		isBlocking = false;
	}

	private void AddListeners() {
		if (listenersAdded)
			return;
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Fire, PunchListener);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Block, BlockListener);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Push, PushListener);
		listenersAdded = true;
	}

	private void RemoveListeners() {
		if (!listenersAdded)
			return;
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Fire, PunchListener);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Block, BlockListener);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Push, PushListener);
		listenersAdded = false;
	}

	private bool PunchListener(GameActionType type, bool down) {
		if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling ||
		    playerEnemy.IsStagger)
			return false;
		fireButtonDown = down;
		if (down && timeToLastPunch <= 0.0) {
			if (SupportsLowStaminaPunch && lowStamina.Value) {
				animatorState.PunchLowStamina();
				timeToLastPunch = 1.5f;
			} else {
				blockStance = 0.0f;
				if (stealth.Value) {
					animatorState.PunchBackstab();
					timeToLastPunch = 1.2f;
				} else
					animatorState.PunchUppercut();
			}

			if (durability != null && durability.Value <= 0.0) {
				var weaponShootEvent = WeaponShootEvent;
				if (weaponShootEvent != null)
					weaponShootEvent(item, ShotType.None, ReactionType.None, ShotSubtypeEnum.WeaponBroken);
			}

			return true;
		}

		if (down || fireDownTime >= 0.40000000596046448 || timeToLastPunch > 0.0)
			return true;
		blockStance = 0.0f;
		realBlockStance = 0.0f;
		animatorState.BlockStance = SmoothUtility.Smooth12(realBlockStance);
		if (SupportsLowStaminaPunch && lowStamina.Value) {
			animatorState.PunchLowStamina();
			timeToLastPunch = 1.5f;
		} else {
			if (lastPunchWasByLeftHand)
				animatorState.PunchRight();
			else
				animatorState.PunchLeft();
			lastPunchWasByLeftHand = !lastPunchWasByLeftHand;
			timeToLastPunch = 0.8f;
		}

		if (durability != null && durability.Value <= 0.0) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent != null)
				weaponShootEvent(item, ShotType.None, ReactionType.None, ShotSubtypeEnum.WeaponBroken);
		}

		return true;
	}

	private bool PushListener(GameActionType type, bool down) {
		if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling ||
		    lowStamina.Value)
			return false;
		if (!down || timeToLastPunch > 0.0)
			return true;
		animatorState.Push();
		timeToLastPunch = ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerPunchCooldownTime;
		return true;
	}

	private bool BlockListener(GameActionType type, bool down) {
		if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
			return false;
		isBlocking = down;
		return true;
	}

	public void Update(IEntity target) {
		if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
			return;
		var target1 = 0.0f;
		var flag = controllerComponent != null && controllerComponent.IsRun.Value;
		if (controllerComponent.IsWalk.Value)
			target1 = (flag ? 1f : 0.5f) * controllerComponent.WalkModifier.Value;
		if (fireButtonDown)
			fireDownTime += Time.deltaTime;
		else
			fireDownTime = 0.0f;
		if (timeToLastPunch > 0.0)
			timeToLastPunch -= Time.deltaTime;
		smoothedNormalizedSpeed = Mathf.MoveTowards(smoothedNormalizedSpeed, target1, Time.deltaTime / 1f);
		animatorState.WalkSpeed = smoothedNormalizedSpeed;
		if (entity != ServiceLocator.GetService<ISimulation>().Player || !PlayerUtility.IsPlayerCanControlling)
			return;
		blockStance = !isBlocking || timeToLastPunch > 0.0 ? 0.0f : 1f;
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

	public void FixedUpdate(IEntity target) {
		realBlockStance = Mathf.MoveTowards(realBlockStance, blockStance,
			Time.fixedDeltaTime /
			ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerBlockStanceTime);
		animatorState.BlockStance = SmoothUtility.Smooth12(realBlockStance);
		realStamina = Mathf.MoveTowards(realStamina, lowStamina.Value ? 0.0f : 1f,
			Time.fixedDeltaTime /
			ScriptableObjectInstance<FightSettingsData>.Instance.Description.PlayerBlockStanceTime);
		animatorState.Stamina = realStamina;
		if (!(playerEnemy != null))
			return;
		playerEnemy.BlockStance = realBlockStance >= 0.5;
	}

	public void OnAnimatorEvent(string data) {
		if (data.StartsWith(WeaponPunchLowStaminaString)) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.LowStamina, lastPunchWasByLeftHand ? ReactionType.Left : ReactionType.Right,
				ShotSubtypeEnum.None);
		} else if (data.StartsWith(WeaponPunchString)) {
			var shotType = !SupportsLowStaminaPunch
				? !lowStamina.Value ? ShotType.Moderate : ShotType.LowStamina
				: ShotType.Moderate;
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, shotType, lastPunchWasByLeftHand ? ReactionType.Left : ReactionType.Right,
				ShotSubtypeEnum.None);
		} else if (data.StartsWith(WeaponPrepunchString)) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.Prepunch, lastPunchWasByLeftHand ? ReactionType.Left : ReactionType.Right,
				ShotSubtypeEnum.None);
		} else if (data.StartsWith(WeaponPushString)) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.Push, ReactionType.None, ShotSubtypeEnum.None);
		} else if (data.StartsWith(WeaponUppercutString)) {
			var shotType = !SupportsLowStaminaPunch
				? !lowStamina.Value ? ShotType.Uppercut : ShotType.LowStamina
				: ShotType.Uppercut;
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, shotType, ReactionType.Uppercut, ShotSubtypeEnum.None);
		} else if (data.StartsWith(WeaponBackstabString)) {
			var shotType = !SupportsLowStaminaPunch
				? !lowStamina.Value ? ShotType.Backstab : ShotType.LowStamina
				: ShotType.Backstab;
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, shotType, ReactionType.Backstab, ShotSubtypeEnum.None);
		} else {
			if (!data.StartsWith(WeaponSpecialPrepunchString))
				return;
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent != null)
				weaponShootEvent(item, ShotType.SpecialPrepunch, ReactionType.Uppercut, ShotSubtypeEnum.None);
		}
	}

	public void Reaction() { }
}