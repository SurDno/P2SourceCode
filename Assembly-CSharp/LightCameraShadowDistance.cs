using UnityEngine;
using UnityEngine.Profiling;

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
    this.OnPreCullEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void OnPreCullEvent2(Camera camera)
  {
    this.cachedLight.shadows = (double) Vector3.Distance(camera.transform.position, this.cachedTransform.position) > (double) this.shadowDistance ? LightShadows.None : this.shadows;
  }

  private void OnEnable()
  {
    this.cachedTransform = this.transform;
    this.cachedLight = this.GetComponent<Light>();
    this.shadows = this.cachedLight.shadows;
    Camera.onPreCull += new Camera.CameraCallback(this.OnPreCullEvent);
  }

  private void OnDisable()
  {
    Camera.onPreCull -= new Camera.CameraCallback(this.OnPreCullEvent);
    this.cachedLight.shadows = this.shadows;
    this.cachedLight = (Light) null;
    this.cachedTransform = (Transform) null;
  }
}
