using Engine.Impl.UI.Controls;
using UnityEngine;
using UnityEngine.Profiling;

[ExecuteInEditMode]
public class CameraDistance : MonoBehaviour
{
  [SerializeField]
  private FloatView view;
  private Transform cachedTransform;

  private void OnPreCullEvent(Camera camera)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(nameof (CameraDistance));
    this.OnPreCullEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void OnPreCullEvent2(Camera camera)
  {
    if ((1 << this.gameObject.layer & camera.cullingMask) == 0 || (Object) this.view == (Object) null)
      return;
    this.view.FloatValue = Vector3.Distance(camera.transform.position, this.cachedTransform.position);
  }

  private void OnEnable()
  {
    this.cachedTransform = this.transform;
    Camera.onPreCull += new Camera.CameraCallback(this.OnPreCullEvent);
  }

  private void OnDisable()
  {
    Camera.onPreCull -= new Camera.CameraCallback(this.OnPreCullEvent);
    this.cachedTransform = (Transform) null;
  }
}
