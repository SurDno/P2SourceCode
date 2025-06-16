using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common.Components.AttackerPlayer;
using UnityEngine;

public class NPCSamopalWeaponController : NPCWeaponControllerBase
{
  private GameObject samopal;
  private NPCEnemy owner;
  private IKController ikController;
  private SamopalObject samopalObject;

  public override void Initialise(NPCWeaponService service)
  {
    base.Initialise(service);
    if ((Object) service.SamopalParent != (Object) null)
    {
      if ((Object) this.samopal == (Object) null)
        this.samopal = Object.Instantiate<GameObject>(service.SamopalPrefab, service.SamopalParent);
      if ((Object) this.samopal != (Object) null)
      {
        this.samopal.SetActive(false);
        this.samopalObject = this.samopal.GetComponentInChildren<SamopalObject>();
        this.IndoorChanged();
        service.pivot.AimTransform = this.samopalObject.AimPoint;
        service.pivot.ShootStart = this.samopal;
      }
    }
    this.owner = service.gameObject.GetComponent<NPCEnemy>();
    this.ikController = service.gameObject.GetComponent<IKController>();
  }

  public override void IndoorChanged()
  {
    if (!((Object) this.samopalObject != (Object) null) || !((Object) this.service != (Object) null))
      return;
    this.samopalObject.SetIndoor(this.service.IsIndoor);
  }

  public override void Shutdown()
  {
    if ((Object) this.samopal != (Object) null)
      this.samopal.SetActive(false);
    this.Drop();
    base.Shutdown();
  }

  protected override void ShowWeapon(bool show)
  {
    base.ShowWeapon(show);
    if (show && (Object) this.service.SamopalPrefab == (Object) null)
    {
      Debug.LogError((object) (this.owner.gameObject.ToString() + " doesn`t support samopal!"));
    }
    else
    {
      if (show && (Object) this.samopal == (Object) null)
        this.samopal = Object.Instantiate<GameObject>(this.service.SamopalPrefab, this.service.SamopalParent);
      if (!((Object) this.samopal != (Object) null))
        return;
      this.samopal.SetActive(show);
    }
  }

  protected override void GetLayersIndices()
  {
    if (!((Object) this.animator != (Object) null))
      return;
    this.walkLayerIndex = this.animator.GetLayerIndex("Fight Gun Walk Layer");
    this.attackLayerIndex = this.animator.GetLayerIndex("Fight Gun Attack Layer");
    this.reactionLayerIndex = this.animator.GetLayerIndex("Fight Empty Reaction Layer");
  }

  public override void OnAnimatorEvent(string data)
  {
    if (data.StartsWith("Samopal.StartAiming"))
    {
      if ((Object) this.ikController != (Object) null && (Object) this.owner.Enemy != (Object) null)
      {
        this.ikController.WeaponTarget = this.owner.Enemy.transform;
        this.ikController.WeaponAimTo = Pivot.AimWeaponType.Chest;
      }
      this.owner.SetAiming(true);
    }
    if (data.StartsWith("Samopal.Fire"))
    {
      if ((Object) this.samopal != (Object) null)
        this.samopal.SetActive(true);
      if ((Object) this.samopalObject != (Object) null)
        this.samopalObject.Shoot();
    }
    if (data.StartsWith("Samopal.Hit"))
    {
      if ((Object) this.ikController != (Object) null)
        this.ikController.WeaponTarget = (Transform) null;
      this.owner.SetAiming(false);
    }
    if (data.StartsWith("Samopal.Drop"))
      this.Shutdown();
    if (data.StartsWith("Samopal.WeaponOn"))
      this.ShowWeapon(true);
    base.OnAnimatorEvent(data);
  }

  public override void PunchReaction(ReactionType reactionType)
  {
    if ((Object) this.ikController != (Object) null)
      this.ikController.WeaponTarget = (Transform) null;
    this.owner.SetAiming(false);
    this.TriggerAction(WeaponActionEnum.ForcedSamopalDrop);
    this.Shutdown();
  }
}
