using Engine.Behaviours.Engines.Controllers;

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
      rifleObject = service.Rifle.GetComponent<RifleObject>();
      IndoorChanged();
    }
    owner = service.gameObject.GetComponent<NPCEnemy>();
    ikController = service.gameObject.GetComponent<IKController>();
  }

  public override void IndoorChanged()
  {
    if (!((Object) rifleObject != (Object) null) || !((Object) service != (Object) null))
      return;
    rifleObject.SetIndoor(service.IsIndoor);
  }

  protected override void GetLayersIndices()
  {
    if (!(bool) (Object) animator)
      return;
    walkLayerIndex = animator.GetLayerIndex("Fight Gun Walk Layer");
    attackLayerIndex = animator.GetLayerIndex("Fight Rifle Attack Layer");
    reactionLayerIndex = animator.GetLayerIndex("Fight Empty Reaction Layer");
  }

  protected override void SetLayers(float weight, bool immediate = false)
  {
    if ((Object) service == (Object) null)
      return;
    if (walkLayerIndex != -1)
      service.AddNeededLayer(walkLayerIndex, weight);
    if (attackLayerIndex != -1)
      service.AddNeededLayer(attackLayerIndex, weight);
    if (reactionLayerIndex != -1)
      service.AddNeededLayer(reactionLayerIndex, weight);
    if (!immediate)
      return;
    service.ForceUpdateLayers();
  }

  public override void Update()
  {
    layersWeight = Mathf.MoveTowards(layersWeight, 1f, Time.deltaTime);
    SetRiflePose();
  }

  public override void UpdateSilent()
  {
    base.UpdateSilent();
    SetRiflePose();
  }

  public override void Activate() => SetLayers(1f);

  public override bool IsChangingWeapon() => false;

  private void SetRiflePose()
  {
    if (!((Object) animator != (Object) null))
      return;
    animator.SetFloat("Fight.RifleHold", layersWeight);
  }

  public override void OnAnimatorEvent(string data)
  {
    if (data.StartsWith("Rifle.Fire"))
    {
      if ((Object) rifleObject != (Object) null)
        rifleObject.Shoot();
      owner.SetAiming(false);
    }
    if (data.StartsWith("Rifle.Reload") && (Object) rifleObject != (Object) null)
      rifleObject.Reload();
    base.OnAnimatorEvent(data);
  }
}
