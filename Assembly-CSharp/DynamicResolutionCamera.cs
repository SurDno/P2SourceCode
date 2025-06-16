using UnityEngine;

[RequireComponent(typeof (Camera))]
public class DynamicResolutionCamera : MonoBehaviour
{
  private Camera camera;

  private void Awake() => this.camera = this.GetComponent<Camera>();

  private void OnDisable()
  {
    this.camera.targetTexture = (RenderTexture) null;
    DynamicResolution.Instance.RemoveCamera(this.camera);
  }

  private void OnEnable() => DynamicResolution.Instance.AddCamera(this.camera);

  private void LateUpdate()
  {
    this.camera.targetTexture = DynamicResolution.Instance.GetTargetTexture();
  }
}
