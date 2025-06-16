// Decompiled with JetBrains decompiler
// Type: NPCKnifeWeaponController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class NPCKnifeWeaponController : NPCWeaponControllerBase
{
  private GameObject knife;

  public override void Initialise(NPCWeaponService service)
  {
    if ((Object) service.KnifeParent != (Object) null)
    {
      this.knife = Object.Instantiate<GameObject>(service.KnifePrefab, service.KnifeParent);
      if ((Object) this.knife != (Object) null)
        this.knife.SetActive(false);
    }
    base.Initialise(service);
  }

  protected override void ShowWeapon(bool show)
  {
    base.ShowWeapon(show);
    if ((Object) this.knife == (Object) null && (Object) this.service.KnifeParent != (Object) null)
      this.knife = Object.Instantiate<GameObject>(this.service.KnifePrefab, this.service.KnifeParent);
    if (!((Object) this.knife != (Object) null))
      return;
    this.knife.SetActive(show);
  }

  public override void Shutdown()
  {
    if ((Object) this.knife != (Object) null)
      Object.Destroy((Object) this.knife);
    base.Shutdown();
  }

  protected override void GetLayersIndices()
  {
    if (!((Object) this.animator != (Object) null))
      return;
    this.walkLayerIndex = this.animator.GetLayerIndex("Fight Knife Walk Layer");
    this.attackLayerIndex = this.animator.GetLayerIndex("Fight Knife Attack Layer");
    this.reactionLayerIndex = this.animator.GetLayerIndex("Fight Knife Reaction Layer");
  }

  public override void OnAnimatorEvent(string data)
  {
    if (data.StartsWith("Knife.WeaponOn"))
      this.ShowWeapon(true);
    base.OnAnimatorEvent(data);
  }
}
