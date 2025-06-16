using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;

namespace Engine.Behaviours.Engines.Controllers
{
  public class PlayerScalpelWeaponController : PlayerUppercotWeaponControllerBase
  {
    protected override string Prefix => "Scalpel";

    protected override void ApplyVisibility()
    {
      pivot.HandsGeometryVisible = geometryVisible;
      pivot.ScalpelGeometryVisible = geometryVisible && weaponVisible;
      ApplyLayerWeight(geometryVisible ? 1f : 0.0f);
    }

    protected override void ApplyLayerWeight(float layerWeight)
    {
      animatorState.ScalpelLayerWeight = layerWeight;
      animatorState.ScalpelReactionLayerWeight = layerWeight;
    }

    public override void SetItem(IEntity item)
    {
      base.SetItem(item);
      this.item = item;
      int kind = 0;
      ParametersComponent component = item.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<int> byName = component.GetByName<int>(ParameterNameEnum.Customization);
        if (byName != null)
          kind = byName.Value;
      }
      pivot.SetScalpelCustomGeometryVisible(kind);
    }

    protected override WeaponKind WeaponKind => WeaponKind.Scalpel;

    protected override bool SupportsLowStaminaPunch => true;
  }
}
