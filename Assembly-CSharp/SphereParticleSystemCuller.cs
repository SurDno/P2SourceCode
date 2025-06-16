using UnityEngine;

internal class SphereParticleSystemCuller : MonoBehaviour
{
  [SerializeField]
  private float raduis = 3f;
  [SerializeField]
  private Vector3 offset;
  private CullingGroup cullingGroup;
  private BoundingSphere[] boundingSpheres = new BoundingSphere[1];
  private ParticleSystemRenderer[] renderers;
  private bool[] renderersIsEnabled;
  private ParticleSystem[] particleSystems;
  private bool[] particleSystemsIsPlaying;

  private void OnEnable()
  {
    this.cullingGroup = new CullingGroup();
    this.boundingSpheres[0] = new BoundingSphere(this.transform.position + this.offset, this.raduis);
    this.cullingGroup.SetBoundingSpheres(this.boundingSpheres);
    this.cullingGroup.onStateChanged += new CullingGroup.StateChanged(this.StateChanged);
    if (this.renderers == null)
    {
      this.renderers = this.gameObject.GetComponentsInChildren<ParticleSystemRenderer>();
      this.renderersIsEnabled = new bool[this.renderers.Length];
      for (int index = 0; index < this.renderers.Length; ++index)
        this.renderersIsEnabled[index] = this.renderers[index].enabled;
      this.particleSystems = this.gameObject.GetComponentsInChildren<ParticleSystem>();
      this.particleSystemsIsPlaying = new bool[this.particleSystems.Length];
      for (int index = 0; index < this.particleSystems.Length; ++index)
        this.particleSystemsIsPlaying[index] = this.particleSystems[index].isPlaying;
    }
    Camera.onPreCull += new Camera.CameraCallback(this.OnPreCullEvent);
  }

  private void OnDisable()
  {
    this.cullingGroup.onStateChanged -= new CullingGroup.StateChanged(this.StateChanged);
    Camera.onPreCull -= new Camera.CameraCallback(this.OnPreCullEvent);
    this.cullingGroup.Dispose();
    this.cullingGroup = (CullingGroup) null;
  }

  public void StateChanged(CullingGroupEvent sphere)
  {
    for (int index = 0; index < this.renderers.Length; ++index)
    {
      if (this.renderersIsEnabled[index])
        this.renderers[index].enabled = sphere.isVisible;
    }
    for (int index = 0; index < this.particleSystems.Length; ++index)
    {
      if (this.particleSystemsIsPlaying[index])
      {
        if (sphere.isVisible)
          this.particleSystems[index].Play();
        else
          this.particleSystems[index].Pause();
      }
    }
  }

  private void OnPreCullEvent(Camera camera)
  {
    if ((Object) GameCamera.Instance == (Object) null || (Object) camera != (Object) GameCamera.Instance.Camera)
      return;
    this.cullingGroup.targetCamera = camera;
  }

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position + this.offset, this.raduis);
  }
}
