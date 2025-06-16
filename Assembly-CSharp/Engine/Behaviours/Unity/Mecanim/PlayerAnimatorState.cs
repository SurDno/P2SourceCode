using System.Collections.Generic;
using UnityEngine;

namespace Engine.Behaviours.Unity.Mecanim;

public class PlayerAnimatorState {
	private static Dictionary<Animator, PlayerAnimatorState> playerAnimatorStates = new();
	public Animator Animator;

	public static void Clear() {
		playerAnimatorStates.Clear();
	}

	public static PlayerAnimatorState GetAnimatorState(Animator animator) {
		PlayerAnimatorState animatorState;
		if (!playerAnimatorStates.TryGetValue(animator, out animatorState)) {
			animatorState = new PlayerAnimatorState();
			animatorState.Animator = animator;
			playerAnimatorStates[animator] = animatorState;
		}

		return animatorState;
	}

	public float HandsLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Hands Layer"), Mathf.Clamp01(value));
	}

	public float KnifeLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Knife Layer"), Mathf.Clamp01(value));
	}

	public float LockpickLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Lockpick Layer"), Mathf.Clamp01(value));
	}

	public float ScalpelLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Scalpel Layer"), Mathf.Clamp01(value));
	}

	public float RevolverLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Revolver Layer"), Mathf.Clamp01(value));
	}

	public float RifleLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Rifle Layer"), Mathf.Clamp01(value));
	}

	public float ShotgunLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Shotgun Layer"), Mathf.Clamp01(value));
	}

	public float FlashlightLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Flashlight Layer"), Mathf.Clamp01(value));
	}

	public float VisirLightLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Visir Layer"), Mathf.Clamp01(value));
	}

	public float ReactionLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Reaction Layer"), Mathf.Clamp01(value));
	}

	public float FlashlightReactionLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Flashlight Reaction Layer"), Mathf.Clamp01(value));
	}

	public float ScalpelReactionLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Scalpel Reaction Layer"), Mathf.Clamp01(value));
	}

	public float KnifeReactionLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Knife Reaction Layer"), Mathf.Clamp01(value));
	}

	public float RifleReactionLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Rifle Reaction Layer"), Mathf.Clamp01(value));
	}

	public float ShotgunReactionLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Shotgun Reaction Layer"), Mathf.Clamp01(value));
	}

	public float RevolverReactionLayerWeight {
		set => Animator.SetLayerWeight(Animator.GetLayerIndex("Revolver Reaction Layer"), Mathf.Clamp01(value));
	}

	public float WalkSpeed {
		set => Animator.SetFloat("Speed", value);
	}

	public void ResetAnimator() {
		Animator.SetInteger("Mecanim.State.Control", 0);
		Animator.SetInteger("AttackerPlayer.Weapon.State.Control", 0);
		Animator.SetTrigger("Reset");
	}

	public bool Fire {
		set => Animator.SetBool("AttackerPlayer.Weapon.Revolver.Fire", value);
	}

	public bool Reload {
		get => Animator.GetBool("AttackerPlayer.Weapon.Revolver.Reload");
		set => Animator.SetBool("AttackerPlayer.Weapon.Revolver.Reload", value);
	}

	public bool Empty {
		get => Animator.GetBool("AttackerPlayer.Weapon.Revolver.Empty");
		set => Animator.SetBool("AttackerPlayer.Weapon.Revolver.Empty", value);
	}

	public bool AlternativeFire {
		set => Animator.SetBool("AttackerPlayer.Weapon.Revolver.Alternative.Fire", value);
	}

	public int Rotate {
		set => Animator.SetInteger("Movable.Stance.Rotate", value);
	}

	public void Unholster() {
		Animator.SetTrigger("Triggers/Unholster");
	}

	public void Holster() {
		Animator.SetTrigger("Triggers/Holster");
	}

	public void PunchLowStamina() {
		Animator.SetTrigger("Triggers/CancelUppercut");
		Animator.SetTrigger("Triggers/PunchLowStamina");
	}

	public void PunchLeft() {
		Animator.SetTrigger("Triggers/CancelUppercut");
		Animator.SetTrigger("Triggers/PunchLeft");
	}

	public void PunchRight() {
		Animator.SetTrigger("Triggers/CancelUppercut");
		Animator.SetTrigger("Triggers/PunchRight");
	}

	public void PunchUppercut() {
		Animator.ResetTrigger("Triggers/CancelUppercut");
		Animator.SetTrigger("Triggers/PunchUppercut");
	}

	public void PunchBackstab() {
		Animator.ResetTrigger("Triggers/CancelUppercut");
		Animator.SetTrigger("Triggers/PunchBackstab");
	}

	public void Push() {
		Animator.SetTrigger("Triggers/Push");
	}

	public void RiflePush() {
		Animator.SetTrigger("Triggers/RiflePush");
	}

	public void RifleShot(int bullets, bool jam = false) {
		if (jam)
			Animator.SetTrigger("Triggers/RifleJamShot");
		else {
			Animator.SetTrigger("Triggers/RifleShot");
			Animator.SetInteger("RifleBullets", bullets);
		}
	}

	public void RifleShotCancel() {
		Animator.SetTrigger("Triggers/RifleShotCancel");
	}

	public void RifleAim(bool aim) {
		Animator.SetBool("RifleIsAiming", aim);
	}

	public void RifleReload(bool reload, bool hard = false) {
		Animator.ResetTrigger("Triggers/RifleRestore");
		if (reload)
			Animator.SetTrigger("Triggers/RifleReload");
		Animator.SetBool("RifleIsReloading", reload);
		Animator.SetBool("RifleIsHard", hard);
		Animator.ResetTrigger("Triggers/RifleCancelReload");
	}

	public void RifleCancelReload() {
		Animator.SetTrigger("Triggers/RifleCancelReload");
		Animator.ResetTrigger("Triggers/RifleReload");
	}

	public void RifleUnholster() {
		Animator.SetTrigger("Triggers/RifleUnholster");
	}

	public void RifleHolster() {
		Animator.SetTrigger("Triggers/RifleHolster");
	}

	public void RifleRestore() {
		Animator.ResetTrigger("Triggers/RifleReload");
		Animator.ResetTrigger("Triggers/RifleCancelReload");
		Animator.SetTrigger("Triggers/RifleRestore");
	}

	public void ShotgunPush() {
		Animator.SetTrigger("Triggers/ShotgunPush");
	}

	public void ShotgunShot(bool empty = false, bool jam = false) {
		if (empty)
			Animator.SetTrigger("Triggers/ShotgunEmptyShot");
		else if (jam)
			Animator.SetTrigger("Triggers/ShotgunJamShot");
		else
			Animator.SetTrigger("Triggers/ShotgunShot");
	}

	public void ShotgunShotCancel() {
		Animator.SetTrigger("Triggers/ShotgunShotCancel");
	}

	public void ShotgunAim(bool aim) {
		Animator.SetBool("ShotgunIsAiming", aim);
	}

	public void ShotgunReload(bool reload, int startAmmo = 0) {
		Animator.ResetTrigger("Triggers/ShotgunRestore");
		if (reload)
			Animator.SetTrigger("Triggers/ShotgunReload");
		Animator.SetBool("ShotgunIsReloading", reload);
		Animator.SetInteger("ShotgunReloadStart", startAmmo);
		Animator.ResetTrigger("Triggers/ShotgunCancelReload");
	}

	public void ShotgunCancelReload() {
		Animator.SetTrigger("Triggers/ShotgunCancelReload");
		Animator.ResetTrigger("Triggers/ShotgunReload");
	}

	public void ShotgunUnholster() {
		Animator.SetTrigger("Triggers/ShotgunUnholster");
	}

	public void ShotgunHolster() {
		Animator.SetTrigger("Triggers/ShotgunHolster");
	}

	public void ShotgunRestore() {
		Animator.ResetTrigger("Triggers/ShotgunReload");
		Animator.ResetTrigger("Triggers/ShotgunCancelReload");
		Animator.SetTrigger("Triggers/ShotgunRestore");
	}

	public void RevolverPush() {
		Animator.SetTrigger("Triggers/RevolverPush");
	}

	public void RevolverShot(bool empty, bool jam) {
		if (empty)
			Animator.SetTrigger("Triggers/RevolverEmptyShot");
		else if (jam)
			Animator.SetTrigger("Triggers/RevolverJamShot");
		else
			Animator.SetTrigger("Triggers/RevolverShot");
	}

	public void RevolverShotCancel() {
		Animator.SetTrigger("Triggers/RevolverShotCancel");
	}

	public void RevolverAim(bool aim) {
		Animator.SetBool("RevolverIsAiming", aim);
	}

	public void RevolverReload(bool reload, int bulletsStart = 0, bool firstReload = false) {
		Animator.ResetTrigger("Triggers/RevolverRestore");
		if (reload)
			Animator.SetTrigger("Triggers/RevolverReload");
		Animator.SetInteger("RevolverReloadStart", bulletsStart);
		Animator.SetBool("RevolverIsReloading", reload);
		Animator.SetBool("RevolverFirstReload", firstReload);
		Animator.ResetTrigger("Triggers/RevolverCancelReload");
	}

	public void RevolverCancelReload() {
		Animator.SetTrigger("Triggers/RevolverCancelReload");
		Animator.ResetTrigger("Triggers/RevolverReload");
	}

	public void RevolverUnholster() {
		Animator.SetTrigger("Triggers/RevolverUnholster");
	}

	public void RevolverHolster() {
		Animator.SetTrigger("Triggers/RevolverHolster");
	}

	public void RevolverRestore() {
		Animator.ResetTrigger("Triggers/RevolverReload");
		Animator.ResetTrigger("Triggers/RevolverCancelReload");
		Animator.SetTrigger("Triggers/RevolverRestore");
	}

	public void FlashlightPunch() {
		Animator.SetTrigger("Triggers/FlashlightPunch");
	}

	public void FlashlightPunchLowStamina() {
		Animator.SetTrigger("Triggers/FlashlightPunchLowStamina");
	}

	public void FlashlightFire() {
		Animator.SetTrigger("Triggers/FlashlightFire");
	}

	public void FlashlightUnholster() {
		Animator.SetTrigger("Triggers/FlashlightUnholster");
	}

	public void FlashlightHolster() {
		Animator.SetTrigger("Triggers/FlashlightHolster");
	}

	public float BlockStance {
		set => Animator.SetFloat(nameof(BlockStance), value);
	}

	public float Stamina {
		set => Animator.SetFloat(nameof(Stamina), value);
	}
}