// Decompiled with JetBrains decompiler
// Type: NPCRifleWeaponController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Engines.Controllers;
using UnityEngine;

#nullable disable
public class NPCRifleWeaponController : NPCWeaponControllerBase
{
  private NPCEnemy owner;
  private IKController ikController;
  private RifleObject rifleObject;

  public override void Initialise(NPCWeaponService service)
  {
    base.Initialise(service);
    if ((Object) service.Rifle != (Object) null)
    {
      this.rifleObject = service.Rifle.GetComponent<RifleObject>();
      this.IndoorChanged();
    }
    this.owner = service.gameObject.GetComponent<NPCEnemy>();
    this.ikController = service.gameObject.GetComponent<IKController>();
  }

  public override void IndoorChanged()
  {
    if (!((Object) this.rifleObject != (Object) null) || !((Object) this.service != (Object) null))
      return;
    this.rifleObject.SetIndoor(this.service.IsIndoor);
  }

  protected override void GetLayersIndices()
  {
    if (!(bool) (Object) this.animator)
      return;
    this.walkLayerIndex = this.animator.GetLayerIndex("Fight Gun Walk Layer");
    this.attackLayerIndex = this.animator.GetLayerIndex("Fight Rifle Attack Layer");
    this.reactionLayerIndex = this.animator.GetLayerIndex("Fight Empty Reaction Layer");
  }

  protected override void SetLayers(float weight, bool immediate = false)
  {
    if ((Object) this.service == (Object) null)
      return;
    if (this.walkLayerIndex != -1)
      this.service.AddNeededLayer(this.walkLayerIndex, weight);
    if (this.attackLayerIndex != -1)
      this.service.AddNeededLayer(this.attackLayerIndex, weight);
    if (this.reactionLayerIndex != -1)
      this.service.AddNeededLayer(this.reactionLayerIndex, weight);
    if (!immediate)
      return;
    this.service.ForceUpdateLayers();
  }

  public override void Update()
  {
    this.layersWeight = Mathf.MoveTowards(this.layersWeight, 1f, Time.deltaTime);
    this.SetRiflePose();
  }

  public override void UpdateSilent()
  {
    base.UpdateSilent();
    this.SetRiflePose();
  }

  public override void Activate() => this.SetLayers(1f, false);

  public override bool IsChangingWeapon() => false;

  private void SetRiflePose()
  {
    if (!((Object) this.animator != (Object) null))
      return;
    this.animator.SetFloat("Fight.RifleHold", this.layersWeight);
  }

  public override void OnAnimatorEvent(string data)
  {
    if (data.StartsWith("Rifle.Fire"))
    {
      if ((Object) this.rifleObject != (Object) null)
        this.rifleObject.Shoot();
      this.owner.SetAiming(false);
    }
    if (data.StartsWith("Rifle.Reload") && (Object) this.rifleObject != (Object) null)
      this.rifleObject.Reload();
    base.OnAnimatorEvent(data);
  }
}
