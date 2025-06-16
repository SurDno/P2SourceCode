// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.ImageEffects.GlobalFogBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
    public float startDistance = 0.0f;
    private static Shader fogShader = (Shader) null;
    private Material fogMaterial = (Material) null;
    [SerializeField]
    private Camera cam;

    public Camera Camera
    {
      get => this.cam;
      set => this.cam = value;
    }

    public override bool CheckResources()
    {
      this.CheckSupport(true);
      if ((Object) GlobalFogBase.fogShader == (Object) null)
        GlobalFogBase.fogShader = Shader.Find("Hidden/Pathologic/Image Effects/Fog");
      this.fogMaterial = this.CheckShaderAndCreateMaterial(GlobalFogBase.fogShader, this.fogMaterial);
      if (!this.isSupported)
        this.ReportAutoDisable();
      return this.isSupported;
    }

    private void OnPostRender() => this.DisableFog();

    private void DisableFog()
    {
      Shader.SetGlobalVector("_FogHeightParams", Vector4.zero);
      Shader.SetGlobalVector("_FogDistanceParams", Vector4.zero);
    }

    protected void OnRenderImage_Internal(RenderTexture source, RenderTexture destination)
    {
      if (!this.CheckResources() || !this.distanceFog && !this.heightFog)
      {
        this.DisableFog();
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        Transform transform = this.cam.transform;
        Matrix4x4 worldToCameraMatrix = this.cam.worldToCameraMatrix;
        Matrix4x4 matrix4x4 = GL.GetGPUProjectionMatrix(this.cam.projectionMatrix, true) * worldToCameraMatrix;
        this.fogMaterial.SetMatrix("_InverseView", worldToCameraMatrix.inverse);
        Shader.SetGlobalMatrix("_IPL_InverseViewProj", matrix4x4.inverse);
        Vector3 position = transform.position;
        float y = -this.height;
        float z = (double) y <= 0.0 ? 1f : 0.0f;
        Shader.SetGlobalVector("_FogHeightParams", new Vector4(position.y + this.height, y, z, this.heightFog ? this.heightDensity * 0.5f : 0.0f));
        Shader.SetGlobalVector("_FogDistanceParams", new Vector4(this.distanceFog ? this.distanceDensity : 0.0f, -Mathf.Max(this.startDistance, 0.0f), 0.0f, 0.0f));
        Graphics.Blit((Texture) source, destination, this.fogMaterial);
      }
    }
  }
}
