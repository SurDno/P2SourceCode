using Engine.Impl.UI.Controls;

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
    OnPreCullEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void OnPreCullEvent2(Camera camera)
  {
    if ((1 << this.gameObject.layer & camera.cullingMask) == 0 || (Object) view == (Object) null)
      return;
    view.FloatValue = Vector3.Distance(camera.transform.position, cachedTransform.position);
  }

  private void OnEnable()
  {
    cachedTransform = this.transform;
    Camera.onPreCull += new Camera.CameraCallback(OnPreCullEvent);
  }

  private void OnDisable()
  {
    Camera.onPreCull -= new Camera.CameraCallback(OnPreCullEvent);
    cachedTransform = (Transform) null;
  }
}
