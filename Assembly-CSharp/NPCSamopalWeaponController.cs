using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common.Components.AttackerPlayer;
using UnityEngine;

public class NPCSamopalWeaponController : NPCWeaponControllerBase {
	private GameObject samopal;
	private NPCEnemy owner;
	private IKController ikController;
	private SamopalObject samopalObject;

	public override void Initialise(NPCWeaponService service) {
		base.Initialise(service);
		if (service.SamopalParent != null) {
			if (samopal == null)
				samopal = Object.Instantiate(service.SamopalPrefab, service.SamopalParent);
			if (samopal != null) {
				samopal.SetActive(false);
				samopalObject = samopal.GetComponentInChildren<SamopalObject>();
				IndoorChanged();
				service.pivot.AimTransform = samopalObject.AimPoint;
				service.pivot.ShootStart = samopal;
			}
		}

		owner = service.gameObject.GetComponent<NPCEnemy>();
		ikController = service.gameObject.GetComponent<IKController>();
	}

	public override void IndoorChanged() {
		if (!(samopalObject != null) || !(service != null))
			return;
		samopalObject.SetIndoor(service.IsIndoor);
	}

	public override void Shutdown() {
		if (samopal != null)
			samopal.SetActive(false);
		Drop();
		base.Shutdown();
	}

	protected override void ShowWeapon(bool show) {
		base.ShowWeapon(show);
		if (show && service.SamopalPrefab == null)
			Debug.LogError(owner.gameObject + " doesn`t support samopal!");
		else {
			if (show && samopal == null)
				samopal = Object.Instantiate(service.SamopalPrefab, service.SamopalParent);
			if (!(samopal != null))
				return;
			samopal.SetActive(show);
		}
	}

	protected override void GetLayersIndices() {
		if (!(animator != null))
			return;
		walkLayerIndex = animator.GetLayerIndex("Fight Gun Walk Layer");
		attackLayerIndex = animator.GetLayerIndex("Fight Gun Attack Layer");
		reactionLayerIndex = animator.GetLayerIndex("Fight Empty Reaction Layer");
	}

	public override void OnAnimatorEvent(string data) {
		if (data.StartsWith("Samopal.StartAiming")) {
			if (ikController != null && owner.Enemy != null) {
				ikController.WeaponTarget = owner.Enemy.transform;
				ikController.WeaponAimTo = Pivot.AimWeaponType.Chest;
			}

			owner.SetAiming(true);
		}

		if (data.StartsWith("Samopal.Fire")) {
			if (samopal != null)
				samopal.SetActive(true);
			if (samopalObject != null)
				samopalObject.Shoot();
		}

		if (data.StartsWith("Samopal.Hit")) {
			if (ikController != null)
				ikController.WeaponTarget = null;
			owner.SetAiming(false);
		}

		if (data.StartsWith("Samopal.Drop"))
			Shutdown();
		if (data.StartsWith("Samopal.WeaponOn"))
			ShowWeapon(true);
		base.OnAnimatorEvent(data);
	}

	public override void PunchReaction(ReactionType reactionType) {
		if (ikController != null)
			ikController.WeaponTarget = null;
		owner.SetAiming(false);
		TriggerAction(WeaponActionEnum.ForcedSamopalDrop);
		Shutdown();
	}
}