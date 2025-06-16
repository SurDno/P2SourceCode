public class ParticleBursterFire : MonoBehaviour
{
  private ParticleBurster particleBurster;

  private void Awake() => particleBurster = this.GetComponent<ParticleBurster>();

  private void OnEnable()
  {
    if ((Object) particleBurster == (Object) null)
      Debug.Log((object) "particleBurster is null");
    else
      particleBurster.Fire();
  }
}
