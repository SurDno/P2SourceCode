// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Controllers.PlayerHandsWeaponController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.AttackerPlayer;

#nullable disable
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
