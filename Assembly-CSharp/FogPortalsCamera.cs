using UnityEngine;

[RequireComponent(typeof (Camera))]
public class FogPortalsCamera : MonoBehaviour
{
  [SerializeField]
  private Camera baseCamera;
  public bool HalfResolution = false;
  private RenderTexture rt;

  private void DestroyTexture()
  {
    if ((Object) this.rt == (Object) null)
      return;
    this.rt.Release();
    Object.Destroy((Object) this.rt);
    this.rt = (RenderTexture) null;
    this.GetComponent<Camera>().targetTexture = (RenderTexture) null;
    Shader.SetGlobalInt("_IndoorDepthEnabled", 0);
    Shader.SetGlobalTexture("_IndoorDepthTexture", (Texture) null);
  }

  private void OnDisable() => this.DestroyTexture();

  private void OnPreCull()
  {
    Camera component = this.GetComponent<Camera>();
    int width = this.HalfResolution ? component.pixelWidth / 2 : component.pixelWidth;
    int height = this.HalfResolution ? component.pixelHeight / 2 : component.pixelHeight;
    if ((Object) this.rt != (Object) null && (this.rt.width != width || this.rt.height != height))
      this.DestroyTexture();
    if ((Object) this.rt == (Object) null)
    {
      this.rt = new RenderTexture(width, height, 24, RenderTextureFormat.Depth);
      component.targetTexture = this.rt;
      Shader.SetGlobalInt("_IndoorDepthEnabled", 1);
      Shader.SetGlobalTexture("_IndoorDepthTexture", (Texture) this.rt);
    }
    component.nearClipPlane = this.baseCamera.nearClipPlane;
    component.farClipPlane = this.baseCamera.farClipPlane;
    component.fieldOfView = this.baseCamera.fieldOfView;
  }
}
