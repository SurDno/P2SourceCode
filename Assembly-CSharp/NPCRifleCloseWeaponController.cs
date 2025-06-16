// Decompiled with JetBrains decompiler
// Type: NPCRifleCloseWeaponController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Engines.Controllers;
using UnityEngine;

#nullable disable
public class NPCRifleCloseWeaponController : NPCWeaponControllerBase
{
  private NPCEnemy owner;
  private IKController ikController;
  private RifleObject rifleObject;

  public override void Initialise(NPCWeaponService service)
  {
    if ((Object) service.Rifle != (Object) null)
      this.rifleObject = service.Rifle.GetComponent<RifleObject>();
    base.Initialise(service);
    this.owner = service.gameObject.GetComponent<NPCEnemy>();
    this.ikController = service.gameObject.GetComponent<IKController>();
  }

  public override void Update()
  {
    this.layersWeight = Mathf.MoveTowards(this.layersWeight, 1f, Time.deltaTime * 5f);
  }

  protected override void GetLayersIndices()
  {
    if (!((Object) this.animator != (Object) null))
      return;
    this.walkLayerIndex = this.animator.GetLayerIndex("Fight Gun Walk Layer");
    this.attackLayerIndex = this.animator.GetLayerIndex("Fight Attack Layer");
    this.reactionLayerIndex = this.animator.GetLayerIndex("Fight Empty Reaction Layer");
  }

  public override void Activate() => this.SetLayers(1f);

  public override bool IsChangingWeapon() => false;

  public override void OnAnimatorEvent(string data) => base.OnAnimatorEvent(data);
}
