using UnityEngine;

public class NPCHandsWeaponController : NPCWeaponControllerBase {
	protected override void GetLayersIndices() {
		if (!(animator != null))
			return;
		walkLayerIndex = animator.GetLayerIndex("Fight Walk Layer");
		attackLayerIndex = animator.GetLayerIndex("Fight Attack Layer");
		reactionLayerIndex = animator.GetLayerIndex("Fight Reaction Layer");
	}

	public override void Activate() {
		SetLayers(1f);
	}

	public override void Update() {
		layersWeight = Mathf.MoveTowards(layersWeight, 1f, Time.deltaTime * 5f);
	}

	public override bool IsChangingWeapon() {
		return layersWeight < 0.5;
	}
}