using Engine.Common.Components.AttackerPlayer;

namespace Engine.Behaviours.Engines.Controllers
{
  public class PlayerHandsWeaponController : PlayerUppercotWeaponControllerBase
  {
    protected override string Prefix => "Hands";

    protected override void ApplyVisibility()
    {
      this.pivot.HandsGeometryVisible = this.geometryVisible;
      this.ApplyLayerWeight(this.geometryVisible ? 1f : 0.0f);
    }

    protected override void ApplyLayerWeight(float layerWeight)
    {
      this.animatorState.HandsLayerWeight = layerWeight;
      this.animatorState.ReactionLayerWeight = layerWeight;
    }

    protected override WeaponKind WeaponKind => WeaponKind.Hands;

    protected override bool SupportsLowStaminaPunch => true;
  }
}
