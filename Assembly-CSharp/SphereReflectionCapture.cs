using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class SphereReflectionCapture : MonoBehaviour
{
  public TextureSize Resolution = TextureSize.Square128;
  public LayerMask CullingMask = 1;
  public float NearClip = 0.15f;
  public float FarClip = 500f;
  public float AmbientIntensity = 1f;
  private RenderTexture outputCubemap;
  private int currentFace;
  private Camera captureCamera;

  public static SphereReflectionCapture Create()
  {
    GameObject gameObject = new GameObject("Spherical Reflection Capture", typeof (ReflectionProbe), typeof (SphereReflectionCapture));
    ReflectionProbe component = gameObject.GetComponent<ReflectionProbe>();
    component.hdr = true;
    component.mode = ReflectionProbeMode.Custom;
    component.size = new Vector3(100f, 100f, 100f);
    return gameObject.GetComponent<SphereReflectionCapture>();
  }

  private void CheckCamera()
  {
    if (captureCamera == null)
    {
      captureCamera = GetComponent<Camera>();
      if (captureCamera == null)
        captureCamera = gameObject.AddComponent<Camera>();
      captureCamera.hideFlags = HideFlags.HideAndDontSave;
      captureCamera.enabled = false;
      captureCamera.fieldOfView = 90f;
      captureCamera.clearFlags = CameraClearFlags.Skybox;
      captureCamera.useOcclusionCulling = false;
      captureCamera.allowHDR = true;
    }
    captureCamera.cullingMask = CullingMask;
    captureCamera.nearClipPlane = NearClip;
    captureCamera.farClipPlane = FarClip;
  }

  private bool CheckRenderTexture(ref RenderTexture rt, bool recreateOnResize = false)
  {
    if (!(rt == null) && (!recreateOnResize || (TextureSize) rt.width == Resolution))
      return false;
    if (rt != null)
    {
      rt.Release();
      Destroy(rt);
    }
    rt = new RenderTexture((int) Resolution, (int) Resolution, 16, RenderTextureFormat.DefaultHDR);
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
    if (!keepOutput && outputCubemap != null)
    {
      Shader.SetGlobalInt("Pathologic_AmbientCubemapSteps", 0);
      Shader.SetGlobalTexture("Pathologic_AmbientCubemap", null);
      DestroyRenderTexutre(ref outputCubemap);
    }
    if (!(captureCamera != null))
      return;
    Destroy(captureCamera);
    captureCamera = null;
  }

  private void DestroyRenderTexutre(ref RenderTexture rt)
  {
    if (!(rt != null))
      return;
    rt.Release();
    Destroy(rt);
    rt = null;
  }

  private void OnDestroy() => Clear();

  private void OnDisable() => Clear();

  private void LateUpdate()
  {
    CheckCamera();
    if (CheckRenderTexture(ref outputCubemap, true))
    {
      captureCamera.RenderToCubemap(outputCubemap);
      currentFace = 0;
    }
    else
    {
      captureCamera.RenderToCubemap(outputCubemap, 1 << currentFace);
      if (currentFace < 5)
        ++currentFace;
      else
        currentFace = 0;
    }
    Shader.SetGlobalInt("Pathologic_AmbientCubemapSteps", Mathf.RoundToInt(Mathf.Log((float) Resolution, 2f)));
    Shader.SetGlobalFloat("Pathologic_AmbientCubemapIntensity", AmbientIntensity);
    Shader.SetGlobalTexture("Pathologic_AmbientCubemap", outputCubemap);
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
