using UnityEngine;

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
