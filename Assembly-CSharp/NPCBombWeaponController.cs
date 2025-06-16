public class NPCBombWeaponController : NPCWeaponControllerBase
{
  private GameObject bomb;
  private ProjectileObject projectile;
  private NPCEnemy npcEnemy;
  private float throwPower = 20f;

  public override void Initialise(NPCWeaponService service)
  {
    npcEnemy = service.GetComponent<NPCEnemy>();
    if ((UnityEngine.Object) service.BombParent != (UnityEngine.Object) null)
    {
      bomb = UnityEngine.Object.Instantiate<GameObject>(service.BombPrefab, service.BombParent);
      bomb?.SetActive(false);
    }
    base.Initialise(service);
  }

  protected override void ShowWeapon(bool show)
  {
    base.ShowWeapon(show);
    if (show)
      return;
    bomb?.SetActive(show);
  }

  protected override void GetLayersIndices()
  {
    if (!((UnityEngine.Object) animator != (UnityEngine.Object) null))
      return;
    walkLayerIndex = animator.GetLayerIndex("Fight Bomb Walk Layer");
    attackLayerIndex = animator.GetLayerIndex("Fight Bomb Attack Layer");
    reactionLayerIndex = animator.GetLayerIndex("Fight Bomb Reaction Layer");
  }

  public override void OnAnimatorEvent(string data)
  {
    if (data.StartsWith("Bomb.Show"))
    {
      if ((UnityEngine.Object) bomb == (UnityEngine.Object) null && (UnityEngine.Object) service.BombParent != (UnityEngine.Object) null)
        bomb = UnityEngine.Object.Instantiate<GameObject>(service.BombPrefab, service.BombParent);
      ShowWeapon(true);
      bomb?.SetActive(true);
    }
    else if (data.StartsWith("Bomb.Throw"))
    {
      bomb?.SetActive(false);
      CreateBomb();
    }
    base.OnAnimatorEvent(data);
  }

  private void CreateBomb()
  {
    EnemyBase enemy = npcEnemy.Enemy;
    if ((UnityEngine.Object) enemy == (UnityEngine.Object) null)
      return;
    Vector3 vector3 = npcEnemy.LastThrowV * ((enemy.transform.position - npcEnemy.transform.position).normalized * Mathf.Cos(npcEnemy.LastThrowAngle) + Vector3.up * Mathf.Sin(npcEnemy.LastThrowAngle));
    Transform transform = service.BombParent.transform;
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ScriptableObjectInstance<ResourceFromCodeData>.Instance.BottleBomb, transform.position, transform.rotation);
    Rigidbody component = gameObject.GetComponent<Rigidbody>();
    component.velocity = vector3;
    component.angularVelocity = UnityEngine.Random.insideUnitSphere * throwPower;
    component.useGravity = true;
    projectile = gameObject.GetComponent<ProjectileObject>();
    projectile.SetOwner(npcEnemy);
    projectile.OnProjectileHit -= OnProjectileHit;
    projectile.OnProjectileHit += OnProjectileHit;
  }

  private void OnProjectileHit(Transform projectileTransform)
  {
    service.ProjectileHitPosition = projectileTransform.position;
    service.ProjectileHitRotation = projectileTransform.rotation;
    BombHit();
    UnityEngine.Object.Destroy((UnityEngine.Object) projectile.gameObject);
  }
}
