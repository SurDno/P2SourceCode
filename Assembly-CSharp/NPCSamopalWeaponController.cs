using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common.Components.AttackerPlayer;

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
      if ((Object) samopal == (Object) null)
        samopal = Object.Instantiate<GameObject>(service.SamopalPrefab, service.SamopalParent);
      if ((Object) samopal != (Object) null)
      {
        samopal.SetActive(false);
        samopalObject = samopal.GetComponentInChildren<SamopalObject>();
        IndoorChanged();
        service.pivot.AimTransform = samopalObject.AimPoint;
        service.pivot.ShootStart = samopal;
      }
    }
    owner = service.gameObject.GetComponent<NPCEnemy>();
    ikController = service.gameObject.GetComponent<IKController>();
  }

  public override void IndoorChanged()
  {
    if (!((Object) samopalObject != (Object) null) || !((Object) service != (Object) null))
      return;
    samopalObject.SetIndoor(service.IsIndoor);
  }

  public override void Shutdown()
  {
    if ((Object) samopal != (Object) null)
      samopal.SetActive(false);
    Drop();
    base.Shutdown();
  }

  protected override void ShowWeapon(bool show)
  {
    base.ShowWeapon(show);
    if (show && (Object) service.SamopalPrefab == (Object) null)
    {
      Debug.LogError((object) (owner.gameObject.ToString() + " doesn`t support samopal!"));
    }
    else
    {
      if (show && (Object) samopal == (Object) null)
        samopal = Object.Instantiate<GameObject>(service.SamopalPrefab, service.SamopalParent);
      if (!((Object) samopal != (Object) null))
        return;
      samopal.SetActive(show);
    }
  }

  protected override void GetLayersIndices()
  {
    if (!((Object) animator != (Object) null))
      return;
    walkLayerIndex = animator.GetLayerIndex("Fight Gun Walk Layer");
    attackLayerIndex = animator.GetLayerIndex("Fight Gun Attack Layer");
    reactionLayerIndex = animator.GetLayerIndex("Fight Empty Reaction Layer");
  }

  public override void OnAnimatorEvent(string data)
  {
    if (data.StartsWith("Samopal.StartAiming"))
    {
      if ((Object) ikController != (Object) null && (Object) owner.Enemy != (Object) null)
      {
        ikController.WeaponTarget = owner.Enemy.transform;
        ikController.WeaponAimTo = Pivot.AimWeaponType.Chest;
      }
      owner.SetAiming(true);
    }
    if (data.StartsWith("Samopal.Fire"))
    {
      if ((Object) samopal != (Object) null)
        samopal.SetActive(true);
      if ((Object) samopalObject != (Object) null)
        samopalObject.Shoot();
    }
    if (data.StartsWith("Samopal.Hit"))
    {
      if ((Object) ikController != (Object) null)
        ikController.WeaponTarget = (Transform) null;
      owner.SetAiming(false);
    }
    if (data.StartsWith("Samopal.Drop"))
      Shutdown();
    if (data.StartsWith("Samopal.WeaponOn"))
      ShowWeapon(true);
    base.OnAnimatorEvent(data);
  }

  public override void PunchReaction(ReactionType reactionType)
  {
    if ((Object) ikController != (Object) null)
      ikController.WeaponTarget = (Transform) null;
    owner.SetAiming(false);
    TriggerAction(WeaponActionEnum.ForcedSamopalDrop);
    Shutdown();
  }
}
