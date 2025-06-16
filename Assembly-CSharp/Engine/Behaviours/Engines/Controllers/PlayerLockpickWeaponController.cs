using Engine.Common.Components.AttackerPlayer;

namespace Engine.Behaviours.Engines.Controllers
{
  public class PlayerLockpickWeaponController : PlayerUppercotWeaponControllerBase
  {
    protected override string Prefix => "Lockpick";

    protected override void ApplyVisibility()
    {
      this.pivot.HandsGeometryVisible = this.geometryVisible;
      this.pivot.LockpickGeometryVisible = this.geometryVisible && this.weaponVisible;
      this.ApplyLayerWeight(this.geometryVisible ? 1f : 0.0f);
    }

    protected override void ApplyLayerWeight(float layerWeight)
    {
      this.animatorState.LockpickLayerWeight = layerWeight;
      this.animatorState.KnifeReactionLayerWeight = layerWeight;
    }

    protected override WeaponKind WeaponKind => WeaponKind.Lockpick;

    protected override bool SupportsLowStaminaPunch => true;
  }
}
