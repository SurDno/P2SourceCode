using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class SphereReflectionCapture : MonoBehaviour
{
  public SphereReflectionCapture.TextureSize Resolution = SphereReflectionCapture.TextureSize.Square128;
  public LayerMask CullingMask = (LayerMask) 1;
  public float NearClip = 0.15f;
  public float FarClip = 500f;
  public float AmbientIntensity = 1f;
  private RenderTexture outputCubemap = (RenderTexture) null;
  private int currentFace = 0;
  private Camera captureCamera = (Camera) null;

  public static SphereReflectionCapture Create()
  {
    GameObject gameObject = new GameObject("Spherical Reflection Capture", new System.Type[2]
    {
      typeof (ReflectionProbe),
      typeof (SphereReflectionCapture)
    });
    ReflectionProbe component = gameObject.GetComponent<ReflectionProbe>();
    component.hdr = true;
    component.mode = ReflectionProbeMode.Custom;
    component.size = new Vector3(100f, 100f, 100f);
    return gameObject.GetComponent<SphereReflectionCapture>();
  }

  private void CheckCamera()
  {
    if ((UnityEngine.Object) this.captureCamera == (UnityEngine.Object) null)
    {
      this.captureCamera = this.GetComponent<Camera>();
      if ((UnityEngine.Object) this.captureCamera == (UnityEngine.Object) null)
        this.captureCamera = this.gameObject.AddComponent<Camera>();
      this.captureCamera.hideFlags = HideFlags.HideAndDontSave;
      this.captureCamera.enabled = false;
      this.captureCamera.fieldOfView = 90f;
      this.captureCamera.clearFlags = CameraClearFlags.Skybox;
      this.captureCamera.useOcclusionCulling = false;
      this.captureCamera.allowHDR = true;
    }
    this.captureCamera.cullingMask = (int) this.CullingMask;
    this.captureCamera.nearClipPlane = this.NearClip;
    this.captureCamera.farClipPlane = this.FarClip;
  }

  private bool CheckRenderTexture(ref RenderTexture rt, bool recreateOnResize = false)
  {
    if (!((UnityEngine.Object) rt == (UnityEngine.Object) null) && (!recreateOnResize || (SphereReflectionCapture.TextureSize) rt.width == this.Resolution))
      return false;
    if ((UnityEngine.Object) rt != (UnityEngine.Object) null)
    {
      rt.Release();
      UnityEngine.Object.Destroy((UnityEngine.Object) rt);
    }
    rt = new RenderTexture((int) this.Resolution, (int) this.Resolution, 16, RenderTextureFormat.DefaultHDR);
    rt.name = "Reflection Cubemap";
    rt.dimension = TextureDimension.Cube;
    rt.hideFlags = HideFlags.HideAndDontSave;
    rt.useMipMap = true;
    rt.autoGenerateMips = true;
    rt.filterMode = FilterMode.Trilinear;
    return true;
  }

  private void Clear(bool keepOutput = false)
  {
    if (!keepOutput && (UnityEngine.Object) this.outputCubemap != (UnityEngine.Object) null)
    {
      Shader.SetGlobalInt("Pathologic_AmbientCubemapSteps", 0);
      Shader.SetGlobalTexture("Pathologic_AmbientCubemap", (Texture) null);
      this.DestroyRenderTexutre(ref this.outputCubemap);
    }
    if (!((UnityEngine.Object) this.captureCamera != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.captureCamera);
    this.captureCamera = (Camera) null;
  }

  private void DestroyRenderTexutre(ref RenderTexture rt)
  {
    if (!((UnityEngine.Object) rt != (UnityEngine.Object) null))
      return;
    rt.Release();
    UnityEngine.Object.Destroy((UnityEngine.Object) rt);
    rt = (RenderTexture) null;
  }

  private void OnDestroy() => this.Clear();

  private void OnDisable() => this.Clear();

  private void LateUpdate()
  {
    this.CheckCamera();
    if (this.CheckRenderTexture(ref this.outputCubemap, true))
    {
      this.captureCamera.RenderToCubemap(this.outputCubemap);
      this.currentFace = 0;
    }
    else
    {
      this.captureCamera.RenderToCubemap(this.outputCubemap, 1 << this.currentFace);
      if (this.currentFace < 5)
        ++this.currentFace;
      else
        this.currentFace = 0;
    }
    Shader.SetGlobalInt("Pathologic_AmbientCubemapSteps", Mathf.RoundToInt(Mathf.Log((float) this.Resolution, 2f)));
    Shader.SetGlobalFloat("Pathologic_AmbientCubemapIntensity", this.AmbientIntensity);
    Shader.SetGlobalTexture("Pathologic_AmbientCubemap", (Texture) this.outputCubemap);
  }

  public enum TextureSize
  {
    Square16 = 16, // 0x00000010
    Square32 = 32, // 0x00000020
    Square64 = 64, // 0x00000040
    Square128 = 128, // 0x00000080
    Square256 = 256, // 0x00000100
    Square512 = 512, // 0x00000200
    Square1024 = 1024, // 0x00000400
    Square2048 = 2048, // 0x00000800
  }
}
