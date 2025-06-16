public class NPCKnifeWeaponController : NPCWeaponControllerBase
{
  private GameObject knife;

  public override void Initialise(NPCWeaponService service)
  {
    if ((Object) service.KnifeParent != (Object) null)
    {
      knife = Object.Instantiate<GameObject>(service.KnifePrefab, service.KnifeParent);
      if ((Object) knife != (Object) null)
        knife.SetActive(false);
    }
    base.Initialise(service);
  }

  protected override void ShowWeapon(bool show)
  {
    base.ShowWeapon(show);
    if ((Object) knife == (Object) null && (Object) service.KnifeParent != (Object) null)
      knife = Object.Instantiate<GameObject>(service.KnifePrefab, service.KnifeParent);
    if (!((Object) knife != (Object) null))
      return;
    knife.SetActive(show);
  }

  public override void Shutdown()
  {
    if ((Object) knife != (Object) null)
      Object.Destroy((Object) knife);
    base.Shutdown();
  }

  protected override void GetLayersIndices()
  {
    if (!((Object) animator != (Object) null))
      return;
    walkLayerIndex = animator.GetLayerIndex("Fight Knife Walk Layer");
    attackLayerIndex = animator.GetLayerIndex("Fight Knife Attack Layer");
    reactionLayerIndex = animator.GetLayerIndex("Fight Knife Reaction Layer");
  }

  public override void OnAnimatorEvent(string data)
  {
    if (data.StartsWith("Knife.WeaponOn"))
      ShowWeapon(true);
    base.OnAnimatorEvent(data);
  }
}
