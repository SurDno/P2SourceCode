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
    cullingGroup = new CullingGroup();
    boundingSpheres[0] = new BoundingSphere(this.transform.position + offset, raduis);
    cullingGroup.SetBoundingSpheres(boundingSpheres);
    cullingGroup.onStateChanged += new CullingGroup.StateChanged(StateChanged);
    if (renderers == null)
    {
      renderers = this.gameObject.GetComponentsInChildren<ParticleSystemRenderer>();
      renderersIsEnabled = new bool[renderers.Length];
      for (int index = 0; index < renderers.Length; ++index)
        renderersIsEnabled[index] = renderers[index].enabled;
      particleSystems = this.gameObject.GetComponentsInChildren<ParticleSystem>();
      particleSystemsIsPlaying = new bool[particleSystems.Length];
      for (int index = 0; index < particleSystems.Length; ++index)
        particleSystemsIsPlaying[index] = particleSystems[index].isPlaying;
    }
    Camera.onPreCull += new Camera.CameraCallback(OnPreCullEvent);
  }

  private void OnDisable()
  {
    cullingGroup.onStateChanged -= new CullingGroup.StateChanged(StateChanged);
    Camera.onPreCull -= new Camera.CameraCallback(OnPreCullEvent);
    cullingGroup.Dispose();
    cullingGroup = (CullingGroup) null;
  }

  public void StateChanged(CullingGroupEvent sphere)
  {
    for (int index = 0; index < renderers.Length; ++index)
    {
      if (renderersIsEnabled[index])
        renderers[index].enabled = sphere.isVisible;
    }
    for (int index = 0; index < particleSystems.Length; ++index)
    {
      if (particleSystemsIsPlaying[index])
      {
        if (sphere.isVisible)
          particleSystems[index].Play();
        else
          particleSystems[index].Pause();
      }
    }
  }

  private void OnPreCullEvent(Camera camera)
  {
    if ((Object) GameCamera.Instance == (Object) null || (Object) camera != (Object) GameCamera.Instance.Camera)
      return;
    cullingGroup.targetCamera = camera;
  }

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position + offset, raduis);
  }
}
