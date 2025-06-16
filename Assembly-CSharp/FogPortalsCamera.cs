[RequireComponent(typeof (Camera))]
public class FogPortalsCamera : MonoBehaviour
{
  [SerializeField]
  private Camera baseCamera;
  public bool HalfResolution = false;
  private RenderTexture rt;

  private void DestroyTexture()
  {
    if ((Object) rt == (Object) null)
      return;
    rt.Release();
    Object.Destroy((Object) rt);
    rt = (RenderTexture) null;
    this.GetComponent<Camera>().targetTexture = (RenderTexture) null;
    Shader.SetGlobalInt("_IndoorDepthEnabled", 0);
    Shader.SetGlobalTexture("_IndoorDepthTexture", (Texture) null);
  }

  private void OnDisable() => DestroyTexture();

  private void OnPreCull()
  {
    Camera component = this.GetComponent<Camera>();
    int width = HalfResolution ? component.pixelWidth / 2 : component.pixelWidth;
    int height = HalfResolution ? component.pixelHeight / 2 : component.pixelHeight;
    if ((Object) rt != (Object) null && (rt.width != width || rt.height != height))
      DestroyTexture();
    if ((Object) rt == (Object) null)
    {
      rt = new RenderTexture(width, height, 24, RenderTextureFormat.Depth);
      component.targetTexture = rt;
      Shader.SetGlobalInt("_IndoorDepthEnabled", 1);
      Shader.SetGlobalTexture("_IndoorDepthTexture", (Texture) rt);
    }
    component.nearClipPlane = baseCamera.nearClipPlane;
    component.farClipPlane = baseCamera.farClipPlane;
    component.fieldOfView = baseCamera.fieldOfView;
  }
}
