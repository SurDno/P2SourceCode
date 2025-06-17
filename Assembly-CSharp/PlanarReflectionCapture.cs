using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
public class PlanarReflectionCapture : MonoBehaviour
{
  public LayerMask HeightCollidersLayer;
  public Condition UpdateCondition = Condition.Always;
  public float DefaultPlaneHeight;
  public bool ReflectWorld = true;
  [Space]
  public LayerMask WorldLayers = 1;
  public LayerMask SkyLayers = 1;
  public float SkyFarClip = 500f;
  [Space]
  public float RelativeResolution = 0.75f;
  public float RelativeWorldCullingDistance = 0.75f;
  public float RelativeLodBias = 0.25f;
  public float RelativeShadowDistance;
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
    reflectionMat.m00 = (float) (1.0 - 2.0 * plane[0] * plane[0]);
    reflectionMat.m01 = -2f * plane[0] * plane[1];
    reflectionMat.m02 = -2f * plane[0] * plane[2];
    reflectionMat.m03 = -2f * plane[3] * plane[0];
    reflectionMat.m10 = -2f * plane[1] * plane[0];
    reflectionMat.m11 = (float) (1.0 - 2.0 * plane[1] * plane[1]);
    reflectionMat.m12 = -2f * plane[1] * plane[2];
    reflectionMat.m13 = -2f * plane[3] * plane[1];
    reflectionMat.m20 = -2f * plane[2] * plane[0];
    reflectionMat.m21 = -2f * plane[2] * plane[1];
    reflectionMat.m22 = (float) (1.0 - 2.0 * plane[2] * plane[2]);
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
    if (currentCamera == null)
    {
      currentCamera = GetComponent<Camera>();
      if (currentCamera == null)
      {
        Debug.LogWarning("PlanarReflectionCapture: Camera component not found");
        return false;
      }
    }
    int width = Mathf.RoundToInt(currentCamera.pixelWidth * RelativeResolution);
    int height = Mathf.RoundToInt(currentCamera.pixelHeight * RelativeResolution);
    if (!(bool) (Object) reflectionTexture || reflectionTexture.width != width || reflectionTexture.height != height)
    {
      if ((bool) (Object) reflectionTexture)
      {
        reflectionTexture.Release();
        if (Application.isPlaying)
          Destroy(reflectionTexture);
        else
          DestroyImmediate(reflectionTexture);
      }
      reflectionTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
      reflectionTexture.name = "Planar Reflection";
      reflectionTexture.hideFlags = HideFlags.HideAndDontSave;
    }
    if (!(bool) (Object) reflectionCamera)
    {
      GameObject gameObject = new GameObject("Planar Reflection Camera", typeof (Camera), typeof (GlobalFogNonopaque), typeof (GlobalFog));
      gameObject.hideFlags = HideFlags.HideAndDontSave;
      reflectionCamera = gameObject.GetComponent<Camera>();
      reflectionCamera.enabled = false;
      skyReflectionFog = gameObject.GetComponent<GlobalFogNonopaque>();
      skyReflectionFog.Camera = reflectionCamera;
      worldReflectionFog = gameObject.GetComponent<GlobalFog>();
      worldReflectionFog.Camera = reflectionCamera;
    }
    if (skyCullDistances == null)
      skyCullDistances = new float[32];
    if (worldCullDistances == null)
      worldCullDistances = new float[32];
    return true;
  }

  private void OnDisable()
  {
    currentCamera = null;
    if ((bool) (Object) reflectionCamera)
    {
      DestroyImmediate(reflectionCamera.gameObject);
      reflectionCamera = null;
    }
    skyReflectionFog = null;
    worldReflectionFog = null;
    if (!(bool) (Object) reflectionTexture)
      return;
    reflectionTexture.Release();
    if (Application.isPlaying)
      Destroy(reflectionTexture);
    else
      DestroyImmediate(reflectionTexture);
    reflectionTexture = null;
    Shader.SetGlobalTexture("_PlanarReflection", null);
  }

  private void OnPreCull()
  {
    if (!CheckObjects())
    {
      enabled = false;
    }
    else
    {
      bool flag1 = UpdateCondition == Condition.Always;
      float num = DefaultPlaneHeight;
      if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 100f, HeightCollidersLayer, QueryTriggerInteraction.Collide))
      {
        if (UpdateCondition == Condition.OnHit)
          flag1 = true;
        num = hitInfo.point.y;
      }
      if (!flag1 || transform.position.y <= (double) num)
        return;
      reflectionCamera.CopyFrom(currentCamera);
      reflectionCamera.targetTexture = reflectionTexture;
      reflectionCamera.layerCullSpherical = true;
      reflectionCamera.useOcclusionCulling = false;
      reflectionCamera.renderingPath = RenderingPath.DeferredShading;
      reflectionCamera.depthTextureMode = DepthTextureMode.None;
      float lodBias = QualitySettings.lodBias;
      QualitySettings.lodBias = lodBias * RelativeLodBias;
      float shadowDistance = QualitySettings.shadowDistance;
      QualitySettings.shadowDistance = shadowDistance * RelativeShadowDistance;
      Vector3 position = transform.position;
      position.y = num * 2f - position.y;
      reflectionCamera.transform.position = position;
      Vector3 eulerAngles = currentCamera.transform.eulerAngles;
      reflectionCamera.transform.eulerAngles = new Vector3(-eulerAngles.x, eulerAngles.y, -eulerAngles.z);
      GlobalFog globalFog = null;
      bool flag2 = false;
      if (ReplicateGlobalFog)
      {
        globalFog = currentCamera.GetComponent<GlobalFog>();
        flag2 = globalFog != null && globalFog.enabled;
      }
      reflectionCamera.clearFlags = CameraClearFlags.Skybox;
      reflectionCamera.cullingMask = SkyLayers;
      reflectionCamera.farClipPlane = SkyFarClip;
      reflectionCamera.layerCullDistances = skyCullDistances;
      if (flag2 && !ReflectWorld)
      {
        skyReflectionFog.distanceFog = globalFog.distanceFog;
        skyReflectionFog.distanceDensity = globalFog.distanceDensity;
        skyReflectionFog.heightFog = globalFog.heightFog;
        skyReflectionFog.height = globalFog.height;
        skyReflectionFog.heightDensity = globalFog.heightDensity;
        skyReflectionFog.startDistance = globalFog.startDistance;
        skyReflectionFog.enabled = true;
      }
      else
        skyReflectionFog.enabled = false;
      worldReflectionFog.enabled = false;
      reflectionCamera.Render();
      if (ReflectWorld)
      {
        reflectionCamera.clearFlags = CameraClearFlags.Depth;
        reflectionCamera.cullingMask = WorldLayers;
        reflectionCamera.farClipPlane = currentCamera.farClipPlane * RelativeWorldCullingDistance;
        SetCullingDistances(currentCamera.layerCullDistances, worldCullDistances, RelativeWorldCullingDistance);
        reflectionCamera.layerCullDistances = worldCullDistances;
        skyReflectionFog.enabled = false;
        if (flag2)
        {
          worldReflectionFog.distanceFog = globalFog.distanceFog;
          worldReflectionFog.distanceDensity = globalFog.distanceDensity;
          worldReflectionFog.heightFog = globalFog.heightFog;
          worldReflectionFog.height = globalFog.height;
          worldReflectionFog.heightDensity = globalFog.heightDensity;
          worldReflectionFog.startDistance = globalFog.startDistance;
          worldReflectionFog.enabled = true;
        }
        else
          worldReflectionFog.enabled = false;
        reflectionCamera.Render();
      }
      Shader.SetGlobalTexture("_PlanarReflectionTex", reflectionTexture);
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
