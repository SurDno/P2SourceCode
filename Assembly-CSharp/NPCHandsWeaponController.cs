// Decompiled with JetBrains decompiler
// Type: NPCHandsWeaponController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class NPCHandsWeaponController : NPCWeaponControllerBase
{
  protected override void GetLayersIndices()
  {
    if (!((Object) this.animator != (Object) null))
      return;
    this.walkLayerIndex = this.animator.GetLayerIndex("Fight Walk Layer");
    this.attackLayerIndex = this.animator.GetLayerIndex("Fight Attack Layer");
    this.reactionLayerIndex = this.animator.GetLayerIndex("Fight Reaction Layer");
  }

  public override void Activate() => this.SetLayers(1f);

  public override void Update()
  {
    this.layersWeight = Mathf.MoveTowards(this.layersWeight, 1f, Time.deltaTime * 5f);
  }

  public override bool IsChangingWeapon() => (double) this.layersWeight < 0.5;
}
