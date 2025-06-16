using UnityEngine;

public class ParticleBursterFire : MonoBehaviour
{
  private ParticleBurster particleBurster;

  private void Awake() => this.particleBurster = this.GetComponent<ParticleBurster>();

  private void OnEnable()
  {
    if ((Object) this.particleBurster == (Object) null)
      Debug.Log((object) "particleBurster is null");
    else
      this.particleBurster.Fire();
  }
}
