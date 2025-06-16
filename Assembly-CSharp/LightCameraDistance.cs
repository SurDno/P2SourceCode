using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof (Light))]
public class LightCameraDistance : MonoBehaviour
{
  [SerializeField]
  private AnimationCurve rangeCurve = AnimationCurve.Constant(0.0f, 10f, 1f);
  private Transform cachedTransform;
  private Light cachedLight;
  private float baseRange;
  private LightShadows shadows;

  private void OnPreCullEvent(Camera camera)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(nameof (LightCameraDistance));
    this.OnPreCullEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void OnPreCullEvent2(Camera camera)
  {
    float num = this.rangeCurve.Evaluate(Vector3.Distance(camera.transform.position, this.cachedTransform.position));
    this.cachedLight.range = num * this.baseRange;
    this.cachedLight.shadows = (double) num < 0.3 ? LightShadows.None : this.shadows;
  }

  private void OnEnable()
  {
    this.cachedTransform = this.transform;
    this.cachedLight = this.GetComponent<Light>();
    this.baseRange = this.cachedLight.range;
    this.shadows = this.cachedLight.shadows;
    Camera.onPreCull += new Camera.CameraCallback(this.OnPreCullEvent);
  }

  private void OnDisable()
  {
    Camera.onPreCull -= new Camera.CameraCallback(this.OnPreCullEvent);
    this.cachedLight.range = this.baseRange;
    this.cachedLight.shadows = this.shadows;
    this.cachedLight = (Light) null;
    this.cachedTransform = (Transform) null;
  }
}
