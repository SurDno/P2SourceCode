using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
public class PlanarReflectionCapture : MonoBehaviour
{
  public LayerMask HeightCollidersLayer;
  public PlanarReflectionCapture.Condition UpdateCondition = PlanarReflectionCapture.Condition.Always;
  public float DefaultPlaneHeight = 0.0f;
  public bool ReflectWorld = true;
  [Space]
  public LayerMask WorldLayers = (LayerMask) 1;
  public LayerMask SkyLayers = (LayerMask) 1;
  public float SkyFarClip = 500f;
  [Space]
  public float RelativeResolution = 0.75f;
  public float RelativeWorldCullingDistance = 0.75f;
  public float RelativeLodBias = 0.25f;
  public float RelativeShadowDistance = 0.0f;
  [Space]
  public bool ReplicateGlobalFog = true;
  private Camera currentCamera;
  private Camera reflectionCamera;
  private GlobalFog worldReflectionFog;
  private GlobalFogNonopaque skyReflectionFog;
  private RenderTexture reflectionTexture;
  private float[] skyCullDistances;
  private float[] worldCullDistances;

  private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
  {
    reflectionMat.m00 = (float) (1.0 - 2.0 * (double) plane[0] * (double) plane[0]);
    reflectionMat.m01 = -2f * plane[0] * plane[1];
    reflectionMat.m02 = -2f * plane[0] * plane[2];
    reflectionMat.m03 = -2f * plane[3] * plane[0];
    reflectionMat.m10 = -2f * plane[1] * plane[0];
    reflectionMat.m11 = (float) (1.0 - 2.0 * (double) plane[1] * (double) plane[1]);
    reflectionMat.m12 = -2f * plane[1] * plane[2];
    reflectionMat.m13 = -2f * plane[3] * plane[1];
    reflectionMat.m20 = -2f * plane[2] * plane[0];
    reflectionMat.m21 = -2f * plane[2] * plane[1];
    reflectionMat.m22 = (float) (1.0 - 2.0 * (double) plane[2] * (double) plane[2]);
    reflectionMat.m23 = -2f * plane[3] * plane[2];
    reflectionMat.m30 = 0.0f;
    reflectionMat.m31 = 0.0f;
    reflectionMat.m32 = 0.0f;
    reflectionMat.m33 = 1f;
  }

  private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
  {
    Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
    Vector3 lhs = worldToCameraMatrix.MultiplyPoint(pos);
    Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
    return new Vector4(rhs.x, rhs.y, rhs.z, -Vector3.Dot(lhs, rhs));
  }

  private bool CheckObjects()
  {
    if ((UnityEngine.Object) this.currentCamera == (UnityEngine.Object) null)
    {
      this.currentCamera = this.GetComponent<Camera>();
      if ((UnityEngine.Object) this.currentCamera == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "PlanarReflectionCapture: Camera component not found");
        return false;
      }
    }
    int width = Mathf.RoundToInt((float) this.currentCamera.pixelWidth * this.RelativeResolution);
    int height = Mathf.RoundToInt((float) this.currentCamera.pixelHeight * this.RelativeResolution);
    if (!(bool) (UnityEngine.Object) this.reflectionTexture || this.reflectionTexture.width != width || this.reflectionTexture.height != height)
    {
      if ((bool) (UnityEngine.Object) this.reflectionTexture)
      {
        this.reflectionTexture.Release();
        if (Application.isPlaying)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.reflectionTexture);
        else
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.reflectionTexture);
      }
      this.reflectionTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
      this.reflectionTexture.name = "Planar Reflection";
      this.reflectionTexture.hideFlags = HideFlags.HideAndDontSave;
    }
    if (!(bool) (UnityEngine.Object) this.reflectionCamera)
    {
      GameObject gameObject = new GameObject("Planar Reflection Camera", new System.Type[3]
      {
        typeof (Camera),
        typeof (GlobalFogNonopaque),
        typeof (GlobalFog)
      });
      gameObject.hideFlags = HideFlags.HideAndDontSave;
      this.reflectionCamera = gameObject.GetComponent<Camera>();
      this.reflectionCamera.enabled = false;
      this.skyReflectionFog = gameObject.GetComponent<GlobalFogNonopaque>();
      this.skyReflectionFog.Camera = this.reflectionCamera;
      this.worldReflectionFog = gameObject.GetComponent<GlobalFog>();
      this.worldReflectionFog.Camera = this.reflectionCamera;
    }
    if (this.skyCullDistances == null)
      this.skyCullDistances = new float[32];
    if (this.worldCullDistances == null)
      this.worldCullDistances = new float[32];
    return true;
  }

  private void OnDisable()
  {
    this.currentCamera = (Camera) null;
    if ((bool) (UnityEngine.Object) this.reflectionCamera)
    {
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.reflectionCamera.gameObject);
      this.reflectionCamera = (Camera) null;
    }
    this.skyReflectionFog = (GlobalFogNonopaque) null;
    this.worldReflectionFog = (GlobalFog) null;
    if (!(bool) (UnityEngine.Object) this.reflectionTexture)
      return;
    this.reflectionTexture.Release();
    if (Application.isPlaying)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.reflectionTexture);
    else
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.reflectionTexture);
    this.reflectionTexture = (RenderTexture) null;
    Shader.SetGlobalTexture("_PlanarReflection", (Texture) null);
  }

  private void OnPreCull()
  {
    if (!this.CheckObjects())
    {
      this.enabled = false;
    }
    else
    {
      bool flag1 = this.UpdateCondition == PlanarReflectionCapture.Condition.Always;
      float num = this.DefaultPlaneHeight;
      RaycastHit hitInfo;
      if (Physics.Raycast(this.transform.position, Vector3.down, out hitInfo, 100f, (int) this.HeightCollidersLayer, QueryTriggerInteraction.Collide))
      {
        if (this.UpdateCondition == PlanarReflectionCapture.Condition.OnHit)
          flag1 = true;
        num = hitInfo.point.y;
      }
      if (!flag1 || (double) this.transform.position.y <= (double) num)
        return;
      this.reflectionCamera.CopyFrom(this.currentCamera);
      this.reflectionCamera.targetTexture = this.reflectionTexture;
      this.reflectionCamera.layerCullSpherical = true;
      this.reflectionCamera.useOcclusionCulling = false;
      this.reflectionCamera.renderingPath = RenderingPath.DeferredShading;
      this.reflectionCamera.depthTextureMode = DepthTextureMode.None;
      float lodBias = QualitySettings.lodBias;
      QualitySettings.lodBias = lodBias * this.RelativeLodBias;
      float shadowDistance = QualitySettings.shadowDistance;
      QualitySettings.shadowDistance = shadowDistance * this.RelativeShadowDistance;
      Vector3 position = this.transform.position;
      position.y = num * 2f - position.y;
      this.reflectionCamera.transform.position = position;
      Vector3 eulerAngles = this.currentCamera.transform.eulerAngles;
      this.reflectionCamera.transform.eulerAngles = new Vector3(-eulerAngles.x, eulerAngles.y, -eulerAngles.z);
      GlobalFog globalFog = (GlobalFog) null;
      bool flag2 = false;
      if (this.ReplicateGlobalFog)
      {
        globalFog = this.currentCamera.GetComponent<GlobalFog>();
        flag2 = (UnityEngine.Object) globalFog != (UnityEngine.Object) null && globalFog.enabled;
      }
      this.reflectionCamera.clearFlags = CameraClearFlags.Skybox;
      this.reflectionCamera.cullingMask = (int) this.SkyLayers;
      this.reflectionCamera.farClipPlane = this.SkyFarClip;
      this.reflectionCamera.layerCullDistances = this.skyCullDistances;
      if (flag2 && !this.ReflectWorld)
      {
        this.skyReflectionFog.distanceFog = globalFog.distanceFog;
        this.skyReflectionFog.distanceDensity = globalFog.distanceDensity;
        this.skyReflectionFog.heightFog = globalFog.heightFog;
        this.skyReflectionFog.height = globalFog.height;
        this.skyReflectionFog.heightDensity = globalFog.heightDensity;
        this.skyReflectionFog.startDistance = globalFog.startDistance;
        this.skyReflectionFog.enabled = true;
      }
      else
        this.skyReflectionFog.enabled = false;
      this.worldReflectionFog.enabled = false;
      this.reflectionCamera.Render();
      if (this.ReflectWorld)
      {
        this.reflectionCamera.clearFlags = CameraClearFlags.Depth;
        this.reflectionCamera.cullingMask = (int) this.WorldLayers;
        this.reflectionCamera.farClipPlane = this.currentCamera.farClipPlane * this.RelativeWorldCullingDistance;
        this.SetCullingDistances(this.currentCamera.layerCullDistances, this.worldCullDistances, this.RelativeWorldCullingDistance);
        this.reflectionCamera.layerCullDistances = this.worldCullDistances;
        this.skyReflectionFog.enabled = false;
        if (flag2)
        {
          this.worldReflectionFog.distanceFog = globalFog.distanceFog;
          this.worldReflectionFog.distanceDensity = globalFog.distanceDensity;
          this.worldReflectionFog.heightFog = globalFog.heightFog;
          this.worldReflectionFog.height = globalFog.height;
          this.worldReflectionFog.heightDensity = globalFog.heightDensity;
          this.worldReflectionFog.startDistance = globalFog.startDistance;
          this.worldReflectionFog.enabled = true;
        }
        else
          this.worldReflectionFog.enabled = false;
        this.reflectionCamera.Render();
      }
      Shader.SetGlobalTexture("_PlanarReflectionTex", (Texture) this.reflectionTexture);
      QualitySettings.lodBias = lodBias;
      QualitySettings.shadowDistance = shadowDistance;
    }
  }

  private void SetCullingDistances(float[] source, float[] destination, float multiplier)
  {
    int num = source.Length <= destination.Length ? source.Length : destination.Length;
    for (int index = 0; index < num; ++index)
      destination[index] = source[index] * multiplier;
  }

  public enum Condition
  {
    Always,
    OnHit,
    Never,
  }
}
