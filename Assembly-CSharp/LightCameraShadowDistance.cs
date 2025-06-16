[RequireComponent(typeof (Light))]
public class LightCameraShadowDistance : MonoBehaviour
{
  [SerializeField]
  private float shadowDistance = 15f;
  private Transform cachedTransform;
  private Light cachedLight;
  private LightShadows shadows;

  private void OnPreCullEvent(Camera camera)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(nameof (LightCameraShadowDistance));
    OnPreCullEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void OnPreCullEvent2(Camera camera)
  {
    cachedLight.shadows = (double) Vector3.Distance(camera.transform.position, cachedTransform.position) > shadowDistance ? LightShadows.None : shadows;
  }

  private void OnEnable()
  {
    cachedTransform = this.transform;
    cachedLight = this.GetComponent<Light>();
    shadows = cachedLight.shadows;
    Camera.onPreCull += new Camera.CameraCallback(OnPreCullEvent);
  }

  private void OnDisable()
  {
    Camera.onPreCull -= new Camera.CameraCallback(OnPreCullEvent);
    cachedLight.shadows = shadows;
    cachedLight = (Light) null;
    cachedTransform = (Transform) null;
  }
}
