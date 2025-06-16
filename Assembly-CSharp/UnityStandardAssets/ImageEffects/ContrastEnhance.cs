namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Color Adjustments/Contrast Enhance (Unsharp Mask)")]
  public class ContrastEnhance : PostEffectsBase
  {
    [Range(0.0f, 1f)]
    public float intensity = 0.5f;
    [Range(0.0f, 0.999f)]
    public float threshold = 0.0f;
    private Material separableBlurMaterial;
    private Material contrastCompositeMaterial;
    [Range(0.0f, 1f)]
    public float blurSpread = 1f;
    public Shader separableBlurShader = (Shader) null;
    public Shader contrastCompositeShader = (Shader) null;

    public override bool CheckResources()
    {
      CheckSupport(false);
      contrastCompositeMaterial = CheckShaderAndCreateMaterial(contrastCompositeShader, contrastCompositeMaterial);
      separableBlurMaterial = CheckShaderAndCreateMaterial(separableBlurShader, separableBlurMaterial);
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        int width = source.width;
        int height = source.height;
        RenderTexture temporary1 = RenderTexture.GetTemporary(width / 2, height / 2, 0);
        Graphics.Blit((Texture) source, temporary1);
        RenderTexture temporary2 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
        Graphics.Blit((Texture) temporary1, temporary2);
        RenderTexture.ReleaseTemporary(temporary1);
        separableBlurMaterial.SetVector("offsets", new Vector4(0.0f, blurSpread * 1f / (float) temporary2.height, 0.0f, 0.0f));
        RenderTexture temporary3 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
        Graphics.Blit((Texture) temporary2, temporary3, separableBlurMaterial);
        RenderTexture.ReleaseTemporary(temporary2);
        separableBlurMaterial.SetVector("offsets", new Vector4(blurSpread * 1f / (float) temporary2.width, 0.0f, 0.0f, 0.0f));
        RenderTexture temporary4 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
        Graphics.Blit((Texture) temporary3, temporary4, separableBlurMaterial);
        RenderTexture.ReleaseTemporary(temporary3);
        contrastCompositeMaterial.SetTexture("_MainTexBlurred", (Texture) temporary4);
        contrastCompositeMaterial.SetFloat("intensity", intensity);
        contrastCompositeMaterial.SetFloat("threshold", threshold);
        Graphics.Blit((Texture) source, destination, contrastCompositeMaterial);
        RenderTexture.ReleaseTemporary(temporary4);
      }
    }
  }
}
