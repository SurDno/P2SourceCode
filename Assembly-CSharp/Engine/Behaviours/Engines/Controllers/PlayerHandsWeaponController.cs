using Engine.Common.Components.AttackerPlayer;

namespace Engine.Behaviours.Engines.Controllers
{
  public class PlayerHandsWeaponController : PlayerUppercotWeaponControllerBase
  {
    protected override string Prefix => "Hands";

    protected override void ApplyVisibility()
    {
      pivot.HandsGeometryVisible = geometryVisible;
      ApplyLayerWeight(geometryVisible ? 1f : 0.0f);
    }

    protected override void ApplyLayerWeight(float layerWeight)
    {
      animatorState.HandsLayerWeight = layerWeight;
      animatorState.ReactionLayerWeight = layerWeight;
    }

    protected override WeaponKind WeaponKind => WeaponKind.Hands;

    protected override bool SupportsLowStaminaPunch => true;
  }
}
