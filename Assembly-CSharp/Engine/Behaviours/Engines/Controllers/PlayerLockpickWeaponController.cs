using Engine.Common.Components.AttackerPlayer;

namespace Engine.Behaviours.Engines.Controllers;

public class PlayerLockpickWeaponController : PlayerUppercotWeaponControllerBase {
	protected override string Prefix => "Lockpick";

	protected override void ApplyVisibility() {
		pivot.HandsGeometryVisible = geometryVisible;
		pivot.LockpickGeometryVisible = geometryVisible && weaponVisible;
		ApplyLayerWeight(geometryVisible ? 1f : 0.0f);
	}

	protected override void ApplyLayerWeight(float layerWeight) {
		animatorState.LockpickLayerWeight = layerWeight;
		animatorState.KnifeReactionLayerWeight = layerWeight;
	}

	protected override WeaponKind WeaponKind => WeaponKind.Lockpick;

	protected override bool SupportsLowStaminaPunch => true;
}