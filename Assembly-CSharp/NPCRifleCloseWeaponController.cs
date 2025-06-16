using Engine.Behaviours.Engines.Controllers;
using UnityEngine;

public class NPCRifleCloseWeaponController : NPCWeaponControllerBase {
	private NPCEnemy owner;
	private IKController ikController;
	private RifleObject rifleObject;

	public override void Initialise(NPCWeaponService service) {
		if (service.Rifle != null)
			rifleObject = service.Rifle.GetComponent<RifleObject>();
		base.Initialise(service);
		owner = service.gameObject.GetComponent<NPCEnemy>();
		ikController = service.gameObject.GetComponent<IKController>();
	}

	public override void Update() {
		layersWeight = Mathf.MoveTowards(layersWeight, 1f, Time.deltaTime * 5f);
	}

	protected override void GetLayersIndices() {
		if (!(animator != null))
			return;
		walkLayerIndex = animator.GetLayerIndex("Fight Gun Walk Layer");
		attackLayerIndex = animator.GetLayerIndex("Fight Attack Layer");
		reactionLayerIndex = animator.GetLayerIndex("Fight Empty Reaction Layer");
	}

	public override void Activate() {
		SetLayers(1f);
	}

	public override bool IsChangingWeapon() {
		return false;
	}

	public override void OnAnimatorEvent(string data) {
		base.OnAnimatorEvent(data);
	}
}