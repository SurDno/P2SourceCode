using UnityEngine;

public class NPCKnifeWeaponController : NPCWeaponControllerBase {
	private GameObject knife;

	public override void Initialise(NPCWeaponService service) {
		if (service.KnifeParent != null) {
			knife = Object.Instantiate(service.KnifePrefab, service.KnifeParent);
			if (knife != null)
				knife.SetActive(false);
		}

		base.Initialise(service);
	}

	protected override void ShowWeapon(bool show) {
		base.ShowWeapon(show);
		if (knife == null && service.KnifeParent != null)
			knife = Object.Instantiate(service.KnifePrefab, service.KnifeParent);
		if (!(knife != null))
			return;
		knife.SetActive(show);
	}

	public override void Shutdown() {
		if (knife != null)
			Object.Destroy(knife);
		base.Shutdown();
	}

	protected override void GetLayersIndices() {
		if (!(animator != null))
			return;
		walkLayerIndex = animator.GetLayerIndex("Fight Knife Walk Layer");
		attackLayerIndex = animator.GetLayerIndex("Fight Knife Attack Layer");
		reactionLayerIndex = animator.GetLayerIndex("Fight Knife Reaction Layer");
	}

	public override void OnAnimatorEvent(string data) {
		if (data.StartsWith("Knife.WeaponOn"))
			ShowWeapon(true);
		base.OnAnimatorEvent(data);
	}
}