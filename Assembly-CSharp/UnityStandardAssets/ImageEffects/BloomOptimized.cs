namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Bloom and Glow/Bloom (Optimized)")]
  public class BloomOptimized : PostEffectsBase
  {
    [Range(0.0f, 1.5f)]
    public float threshold = 0.25f;
    [Range(0.0f, 2.5f)]
    public float intensity = 0.75f;
    [Range(0.25f, 5.5f)]
    public float blurSize = 1f;
    private Resolution resolution = Resolution.Low;
    [Range(1f, 4f)]
    public int blurIterations = 1;
    public BlurType blurType = BlurType.Standard;
    public Shader fastBloomShader = (Shader) null;
    private Material fastBloomMaterial = (Material) null;

    public override bool CheckResources()
    {
      CheckSupport(false);
      fastBloomMaterial = CheckShaderAndCreateMaterial(fastBloomShader, fastBloomMaterial);
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    private void OnDisable()
    {
      if (!(bool) (Object) fastBloomMaterial)
        return;
      Object.DestroyImmediate((Object) fastBloomMaterial);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        int num1 = resolution == Resolution.Low ? 4 : 2;
        float num2 = resolution == Resolution.Low ? 0.5f : 1f;
        fastBloomMaterial.SetVector("_Parameter", new Vector4(blurSize * num2, 0.0f, threshold, intensity));
        source.filterMode = FilterMode.Bilinear;
        int width = source.width / num1;
        int height = source.height / num1;
        RenderTexture renderTexture1 = RenderTexture.GetTemporary(width, height, 0, source.format);
        renderTexture1.filterMode = FilterMode.Bilinear;
        Graphics.Blit((Texture) source, renderTexture1, fastBloomMaterial, 1);
        int num3 = blurType == BlurType.Standard ? 0 : 2;
        for (int index = 0; index < blurIterations; ++index)
        {
          fastBloomMaterial.SetVector("_Parameter", new Vector4((float) (blurSize * (double) num2 + index * 1.0), 0.0f, threshold, intensity));
          RenderTexture temporary1 = RenderTexture.GetTemporary(width, height, 0, source.format);
          temporary1.filterMode = FilterMode.Bilinear;
          Graphics.Blit((Texture) renderTexture1, temporary1, fastBloomMaterial, 2 + num3);
          RenderTexture.ReleaseTemporary(renderTexture1);
          RenderTexture renderTexture2 = temporary1;
          RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, source.format);
          temporary2.filterMode = FilterMode.Bilinear;
          Graphics.Blit((Texture) renderTexture2, temporary2, fastBloomMaterial, 3 + num3);
          RenderTexture.ReleaseTemporary(renderTexture2);
          renderTexture1 = temporary2;
        }
        fastBloomMaterial.SetTexture("_Bloom", (Texture) renderTexture1);
        Graphics.Blit((Texture) source, destination, fastBloomMaterial, 0);
        RenderTexture.ReleaseTemporary(renderTexture1);
      }
    }

    public enum Resolution
    {
      Low,
      High,
    }

    public enum BlurType
    {
      Standard,
      Sgx,
    }
  }
}
