// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Controllers.PlayerKnifeWeaponController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.AttackerPlayer;

#nullable disable
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
