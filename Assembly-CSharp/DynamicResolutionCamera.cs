using UnityEngine;

[RequireComponent(typeof (Camera))]
public class DynamicResolutionCamera : MonoBehaviour
{
  private Camera camera;

  private void Awake() => camera = GetComponent<Camera>();

  private void OnDisable()
  {
    camera.targetTexture = null;
    DynamicResolution.Instance.RemoveCamera(camera);
  }

  private void OnEnable() => DynamicResolution.Instance.AddCamera(camera);

  private void LateUpdate()
  {
    camera.targetTexture = DynamicResolution.Instance.GetTargetTexture();
  }
}
