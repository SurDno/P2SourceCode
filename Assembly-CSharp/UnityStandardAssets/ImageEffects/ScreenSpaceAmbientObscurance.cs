namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Rendering/Screen Space Ambient Obscurance")]
  internal class ScreenSpaceAmbientObscurance : PostEffectsBase
  {
    [Range(0.0f, 3f)]
    public float intensity = 0.5f;
    [Range(0.1f, 3f)]
    public float radius = 0.2f;
    [Range(0.0f, 3f)]
    public int blurIterations = 1;
    [Range(0.0f, 5f)]
    public float blurFilterDistance = 1.25f;
    [Range(0.0f, 1f)]
    public int downsample = 0;
    public Texture2D rand = (Texture2D) null;
    public Shader aoShader = (Shader) null;
    private Material aoMaterial = (Material) null;

    public override bool CheckResources()
    {
      CheckSupport(true);
      aoMaterial = CheckShaderAndCreateMaterial(aoShader, aoMaterial);
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    private void OnDisable()
    {
      if ((bool) (Object) aoMaterial)
        Object.DestroyImmediate((Object) aoMaterial);
      aoMaterial = (Material) null;
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        Matrix4x4 projectionMatrix = this.GetComponent<Camera>().projectionMatrix;
        Matrix4x4 inverse = projectionMatrix.inverse;
        aoMaterial.SetVector("_ProjInfo", new Vector4((float) (-2.0 / ((double) Screen.width * (double) projectionMatrix[0])), (float) (-2.0 / ((double) Screen.height * (double) projectionMatrix[5])), (1f - projectionMatrix[2]) / projectionMatrix[0], (1f + projectionMatrix[6]) / projectionMatrix[5]));
        aoMaterial.SetMatrix("_ProjectionInv", inverse);
        aoMaterial.SetTexture("_Rand", (Texture) rand);
        aoMaterial.SetFloat("_Radius", radius);
        aoMaterial.SetFloat("_Radius2", radius * radius);
        aoMaterial.SetFloat("_Intensity", intensity);
        aoMaterial.SetFloat("_BlurFilterDistance", blurFilterDistance);
        int width = source.width;
        int height = source.height;
        RenderTexture renderTexture = RenderTexture.GetTemporary(width >> downsample, height >> downsample);
        Graphics.Blit((Texture) source, renderTexture, aoMaterial, 0);
        if (downsample > 0)
        {
          RenderTexture temporary = RenderTexture.GetTemporary(width, height);
          Graphics.Blit((Texture) renderTexture, temporary, aoMaterial, 4);
          RenderTexture.ReleaseTemporary(renderTexture);
          renderTexture = temporary;
        }
        for (int index = 0; index < blurIterations; ++index)
        {
          aoMaterial.SetVector("_Axis", (Vector4) new Vector2(1f, 0.0f));
          RenderTexture temporary = RenderTexture.GetTemporary(width, height);
          Graphics.Blit((Texture) renderTexture, temporary, aoMaterial, 1);
          RenderTexture.ReleaseTemporary(renderTexture);
          aoMaterial.SetVector("_Axis", (Vector4) new Vector2(0.0f, 1f));
          renderTexture = RenderTexture.GetTemporary(width, height);
          Graphics.Blit((Texture) temporary, renderTexture, aoMaterial, 1);
          RenderTexture.ReleaseTemporary(temporary);
        }
        aoMaterial.SetTexture("_AOTex", (Texture) renderTexture);
        Graphics.Blit((Texture) source, destination, aoMaterial, 2);
        RenderTexture.ReleaseTemporary(renderTexture);
      }
    }
  }
}
