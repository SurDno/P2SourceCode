using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  public class GlobalFogBase : PostEffectsBase
  {
    [Tooltip("Apply distance-based fog?")]
    public bool distanceFog = true;
    [Range(0.001f, 10f)]
    public float distanceDensity = 1f;
    [Tooltip("Apply height-based fog?")]
    public bool heightFog = true;
    [Tooltip("Fog top Y coordinate")]
    public float height = 1f;
    [Range(0.001f, 10f)]
    public float heightDensity = 2f;
    [Tooltip("Push fog away from the camera by this amount")]
    public float startDistance;
    private static Shader fogShader;
    private Material fogMaterial;
    [SerializeField]
    private Camera cam;

    public Camera Camera
    {
      get => cam;
      set => cam = value;
    }

    public override bool CheckResources()
    {
      CheckSupport(true);
      if (fogShader == null)
        fogShader = Shader.Find("Hidden/Pathologic/Image Effects/Fog");
      fogMaterial = CheckShaderAndCreateMaterial(fogShader, fogMaterial);
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    private void OnPostRender() => DisableFog();

    private void DisableFog()
    {
      Shader.SetGlobalVector("_FogHeightParams", Vector4.zero);
      Shader.SetGlobalVector("_FogDistanceParams", Vector4.zero);
    }

    protected void OnRenderImage_Internal(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources() || !distanceFog && !heightFog)
      {
        DisableFog();
        Graphics.Blit(source, destination);
      }
      else
      {
        Transform transform = cam.transform;
        Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
        Matrix4x4 matrix4x4 = GL.GetGPUProjectionMatrix(cam.projectionMatrix, true) * worldToCameraMatrix;
        fogMaterial.SetMatrix("_InverseView", worldToCameraMatrix.inverse);
        Shader.SetGlobalMatrix("_IPL_InverseViewProj", matrix4x4.inverse);
        Vector3 position = transform.position;
        float y = -height;
        float z = y <= 0.0 ? 1f : 0.0f;
        Shader.SetGlobalVector("_FogHeightParams", new Vector4(position.y + height, y, z, heightFog ? heightDensity * 0.5f : 0.0f));
        Shader.SetGlobalVector("_FogDistanceParams", new Vector4(distanceFog ? distanceDensity : 0.0f, -Mathf.Max(startDistance, 0.0f), 0.0f, 0.0f));
        Graphics.Blit(source, destination, fogMaterial);
      }
    }
  }
}
