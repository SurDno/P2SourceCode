using System;
using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using UnityEngine;

public class NPCWeaponControllerBase : INPCWeaponController {
	protected Animator animator;
	protected AnimatorState45 animatorState;
	private FightAnimatorBehavior.AnimatorState fightAnimatorState;
	protected float layersWeight;
	protected int walkLayerIndex;
	protected int attackLayerIndex;
	protected int reactionLayerIndex;
	protected NPCWeaponService service;
	private bool droped;
	protected IEntity item;
	protected bool weaponIsShown;

	public event Action<IEntity, ShotType, ReactionType> WeaponShootEvent;

	public virtual void Initialise(NPCWeaponService service) {
		this.service = service;
		animator = service.gameObject.GetComponent<Pivot>().GetAnimator();
		animatorState = AnimatorState45.GetAnimatorState(animator);
		fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(animator);
		GetLayersIndices();
		layersWeight = 0.0f;
		SetLayers(0.0f);
	}

	public virtual void IndoorChanged() { }

	protected virtual void SetLayers(float weight, bool immediate = false) {
		if (service == null)
			return;
		if (walkLayerIndex != -1)
			service.AddNeededLayer(walkLayerIndex, weight);
		if (attackLayerIndex != -1)
			service.AddNeededLayer(attackLayerIndex, weight);
		if (reactionLayerIndex != -1)
			service.AddNeededLayer(reactionLayerIndex, weight);
		if (!immediate)
			return;
		service.ForceUpdateLayers();
	}

	protected virtual void GetLayersIndices() { }

	protected virtual void ShowWeapon(bool show) {
		weaponIsShown = show;
	}

	public virtual void Activate() {
		animatorState.SetTrigger("Fight.Triggers/WeaponPrepare");
		SetLayers(1f);
	}

	public virtual void Shutdown() {
		SetLayers(0.0f);
	}

	public virtual bool IsChangingWeapon() {
		return layersWeight < 1.0 || !weaponIsShown;
	}

	protected void Drop() {
		if (!droped) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent != null)
				weaponShootEvent(item, ShotType.Drop, ReactionType.None);
		}

		droped = true;
	}

	protected void BombHit() {
		var weaponShootEvent = WeaponShootEvent;
		if (weaponShootEvent == null)
			return;
		weaponShootEvent(item, ShotType.Drop, ReactionType.None);
	}

	public void ActivateImmediate() {
		animatorState.SetTrigger("Fight.Triggers/CancelWeaponPrepare");
		layersWeight = 1f;
		SetLayers(1f, true);
		ShowWeapon(true);
	}

	public void ShutdownImmediate() {
		layersWeight = 0.0f;
		SetLayers(0.0f, true);
		ShowWeapon(false);
	}

	public bool Validate(GameObject gameObject) {
		throw new NotImplementedException();
	}

	public virtual void Update() {
		var num = Mathf.MoveTowards(layersWeight, 1f, Time.deltaTime * 5f);
		if (layersWeight == 0.0 && num > 0.0)
			animatorState.SetTrigger("Fight.Triggers/WeaponPrepare");
		if (layersWeight < 1.0 && num >= 1.0)
			animatorState.SetTrigger("Fight.Triggers/WeaponOn");
		layersWeight = num;
	}

	public virtual void UpdateSilent() {
		var num = Mathf.MoveTowards(layersWeight, 0.0f, Time.deltaTime / 0.5f);
		if (layersWeight > 0.5 && num <= 0.5)
			ShowWeapon(false);
		layersWeight = num;
	}

	public void SetItem(IEntity item) {
		this.item = item;
		droped = false;
	}

	public void TriggerAction(WeaponActionEnum weaponAction) {
		switch (weaponAction) {
			case WeaponActionEnum.Uppercut:
				animatorState.SetTrigger("Fight.Triggers/Attack");
				animator.SetInteger("Fight.AttackType", 8);
				break;
			case WeaponActionEnum.JabAttack:
				animatorState.SetTrigger("Fight.Triggers/Attack");
				animator.SetInteger("Fight.AttackType", 0);
				break;
			case WeaponActionEnum.StepAttack:
				animatorState.SetTrigger("Fight.Triggers/Attack");
				animator.SetInteger("Fight.AttackType", 1);
				break;
			case WeaponActionEnum.TelegraphAttack:
				animatorState.SetTrigger("Fight.Triggers/Attack");
				animator.SetInteger("Fight.AttackType", 2);
				break;
			case WeaponActionEnum.RunAttack:
				animatorState.SetTrigger("Fight.Triggers/RunPunch");
				break;
			case WeaponActionEnum.Push:
				animatorState.SetTrigger("Fight.Triggers/Push");
				break;
			case WeaponActionEnum.KnockDown:
				var weaponShootEvent = WeaponShootEvent;
				if (weaponShootEvent == null)
					break;
				weaponShootEvent(item, ShotType.KnockDown, ReactionType.None);
				break;
			case WeaponActionEnum.SamopalAim:
				animatorState.SetTrigger("Fight.Triggers/AimSamopal");
				break;
			case WeaponActionEnum.SamopalFire:
				animatorState.SetTrigger("Fight.Triggers/FireSamopal");
				break;
			case WeaponActionEnum.RifleAim:
				animatorState.SetTrigger("Fight.Triggers/AimRifle");
				break;
			case WeaponActionEnum.RifleFire:
				animatorState.SetTrigger("Fight.Triggers/FireRifle");
				break;
			case WeaponActionEnum.ForcedSamopalDrop:
				animatorState.SetTrigger("Fight.Triggers/DropSamopal");
				break;
		}
	}

	public virtual void OnAnimatorEvent(string data) {
		if (layersWeight < 0.5)
			return;
		var reactionType = !data.EndsWith(" Left")
			? !data.EndsWith(" Right")
				? !data.EndsWith(" Front")
					? !data.EndsWith(" Uppercut") ? ReactionType.Front : ReactionType.Uppercut
					: ReactionType.Front
				: ReactionType.Right
			: ReactionType.Left;
		if (data.StartsWith("Hands.Punch.UltraLight")) {
			if (IsReactingToHit())
				return;
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.UltraLight, reactionType);
		} else if (data.StartsWith("Hands.Punch.Light")) {
			if (IsReactingToHit())
				return;
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.Light, reactionType);
		} else if (data.StartsWith("Hands.Punch.Moderate")) {
			if (IsReactingToHit())
				return;
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.Moderate, reactionType);
		} else if (data.StartsWith("Hands.Punch.Strong")) {
			if (IsReactingToHit())
				return;
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.Strong, reactionType);
		} else if (data.StartsWith("Hands.Uppercut")) {
			if (IsReactingToHit())
				return;
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.Uppercut, ReactionType.Uppercut);
		} else if (data.StartsWith("Hands.Push")) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.Push, ReactionType.None);
		} else if (data.StartsWith("Hands.Prepunch")) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.Prepunch, ReactionType.None);
		} else if (data.StartsWith("Bomb.Throw")) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.Throw, ReactionType.None);
		} else if (data.StartsWith("Samopal.Hit")) {
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.Fire, ReactionType.None);
		} else if (data.StartsWith("Samopal.Drop"))
			Drop();
		else if (data.StartsWith("Rifle.Hit")) {
			if (IsReactingToHit())
				return;
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent == null)
				return;
			weaponShootEvent(item, ShotType.Fire, ReactionType.None);
		} else {
			if (!data.StartsWith("Rifle.Punch"))
				return;
			var weaponShootEvent = WeaponShootEvent;
			if (weaponShootEvent != null)
				weaponShootEvent(item, ShotType.Moderate, ReactionType.Front);
		}
	}

	private bool IsReactingToHit() {
		return fightAnimatorState != null && (fightAnimatorState.IsReaction || fightAnimatorState.IsStagger);
	}

	public virtual void PunchReaction(ReactionType reactionType) { }
}