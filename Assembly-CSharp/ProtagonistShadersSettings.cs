[RequireComponent(typeof (Camera))]
public class ProtagonistShadersSettings : MonoBehaviourInstance<ProtagonistShadersSettings>
{
  [Range(1f, 179f)]
  [SerializeField]
  private float fieldOfView = 60f;
  [Range(0.0f, 1f)]
  [SerializeField]
  private float flatness = 0.9f;
  private Camera camera;

  private Camera Camera
  {
    get
    {
      if ((Object) camera == (Object) null)
        camera = this.GetComponent<Camera>();
      return camera;
    }
  }

  private void OnDisable() => Shader.SetGlobalFloat("_ProtagonistFlatness", 0.0f);

  private void OnPreCull() => UpdateShaders();

  private Matrix4x4 ProtagonistProjection()
  {
    return Matrix4x4.Perspective(fieldOfView, Camera.aspect, Camera.nearClipPlane, Camera.farClipPlane);
  }

  public Vector3 ProtagonistToWorld(Vector3 position)
  {
    Vector4 vector4_1 = Camera.worldToCameraMatrix * new Vector4(position.x, position.y, position.z, 1f);
    Vector4 vector4_2 = ProtagonistProjection() * vector4_1;
    vector4_2.z = Mathf.Lerp(vector4_2.z, -1f * vector4_2.w, flatness);
    Vector4 vector4_3 = Camera.cameraToWorldMatrix * (Camera.nonJitteredProjectionMatrix.inverse * vector4_2);
    return (Vector3) (vector4_3 / vector4_3.w);
  }

  private void UpdateShaders()
  {
    Shader.SetGlobalFloat("_ProtagonistFlatness", flatness);
    Shader.SetGlobalMatrix("_ProtagonistMatrixVP", GL.GetGPUProjectionMatrix(ProtagonistProjection(), true) * Camera.worldToCameraMatrix);
  }
}
