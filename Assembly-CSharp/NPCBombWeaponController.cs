using UnityEngine;

public class NPCBombWeaponController : NPCWeaponControllerBase
{
  private GameObject bomb;
  private ProjectileObject projectile;
  private NPCEnemy npcEnemy;
  private float throwPower = 20f;

  public override void Initialise(NPCWeaponService service)
  {
    npcEnemy = service.GetComponent<NPCEnemy>();
    if (service.BombParent != null)
    {
      bomb = Object.Instantiate(service.BombPrefab, service.BombParent);
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
    if (!(animator != null))
      return;
    walkLayerIndex = animator.GetLayerIndex("Fight Bomb Walk Layer");
    attackLayerIndex = animator.GetLayerIndex("Fight Bomb Attack Layer");
    reactionLayerIndex = animator.GetLayerIndex("Fight Bomb Reaction Layer");
  }

  public override void OnAnimatorEvent(string data)
  {
    if (data.StartsWith("Bomb.Show"))
    {
      if (bomb == null && service.BombParent != null)
        bomb = Object.Instantiate(service.BombPrefab, service.BombParent);
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
    if (enemy == null)
      return;
    Vector3 vector3 = npcEnemy.LastThrowV * ((enemy.transform.position - npcEnemy.transform.position).normalized * Mathf.Cos(npcEnemy.LastThrowAngle) + Vector3.up * Mathf.Sin(npcEnemy.LastThrowAngle));
    Transform transform = service.BombParent.transform;
    GameObject gameObject = Object.Instantiate(ScriptableObjectInstance<ResourceFromCodeData>.Instance.BottleBomb, transform.position, transform.rotation);
    Rigidbody component = gameObject.GetComponent<Rigidbody>();
    component.velocity = vector3;
    component.angularVelocity = Random.insideUnitSphere * throwPower;
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
    Object.Destroy(projectile.gameObject);
  }
}
