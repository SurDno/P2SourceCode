using Engine.Common.Components.AttackerPlayer;

namespace Engine.Behaviours.Engines.Controllers
{
  public class PlayerKnifeWeaponController : PlayerUppercotWeaponControllerBase
  {
    protected override string Prefix => "Knife";

    protected override void ApplyVisibility()
    {
      this.pivot.HandsGeometryVisible = this.geometryVisible;
      this.pivot.KnifeGeometryVisible = this.geometryVisible && this.weaponVisible;
      this.ApplyLayerWeight(this.geometryVisible ? 1f : 0.0f);
    }

    protected override void ApplyLayerWeight(float layerWeight)
    {
      this.animatorState.KnifeLayerWeight = layerWeight;
      this.animatorState.KnifeReactionLayerWeight = layerWeight;
    }

    protected override WeaponKind WeaponKind => WeaponKind.Knife;

    protected override bool SupportsLowStaminaPunch => true;
  }
}
