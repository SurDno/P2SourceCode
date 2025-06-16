using Engine.Common.Components.AttackerPlayer;

namespace Engine.Behaviours.Engines.Controllers
{
  public class PlayerKnifeWeaponController : PlayerUppercotWeaponControllerBase
  {
    protected override string Prefix => "Knife";

    protected override void ApplyVisibility()
    {
      pivot.HandsGeometryVisible = geometryVisible;
      pivot.KnifeGeometryVisible = geometryVisible && weaponVisible;
      ApplyLayerWeight(geometryVisible ? 1f : 0.0f);
    }

    protected override void ApplyLayerWeight(float layerWeight)
    {
      animatorState.KnifeLayerWeight = layerWeight;
      animatorState.KnifeReactionLayerWeight = layerWeight;
    }

    protected override WeaponKind WeaponKind => WeaponKind.Knife;

    protected override bool SupportsLowStaminaPunch => true;
  }
}
