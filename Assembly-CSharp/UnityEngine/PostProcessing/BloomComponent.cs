namespace UnityEngine.PostProcessing
{
  public sealed class BloomComponent : PostProcessingComponentRenderTexture<BloomModel>
  {
    private const int k_MaxPyramidBlurLevel = 16;
    private readonly RenderTexture[] m_BlurBuffer1 = new RenderTexture[16];
    private readonly RenderTexture[] m_BlurBuffer2 = new RenderTexture[16];

    public override bool active
    {
      get
      {
        return model.enabled && model.settings.bloom.intensity > 0.0 && !context.interrupted;
      }
    }

    public void Prepare(RenderTexture source, Material uberMaterial, Texture autoExposure)
    {
      BloomModel.BloomSettings bloom = model.settings.bloom;
      BloomModel.LensDirtSettings lensDirt = model.settings.lensDirt;
      Material mat = context.materialFactory.Get("Hidden/Post FX/Bloom");
      mat.shaderKeywords = null;
      mat.SetTexture(Uniforms._AutoExposure, autoExposure);
      int width = context.width / 2;
      int num1 = context.height / 2;
      if (width < 1)
        width = 1;
      if (num1 < 1)
        num1 = 1;
      RenderTextureFormat format = Application.isMobilePlatform ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
      float num2 = (float) (Mathf.Log(num1, 2f) + (double) bloom.radius - 8.0);
      int num3 = (int) num2;
      int num4 = Mathf.Clamp(num3, 1, 16);
      float thresholdLinear = bloom.thresholdLinear;
      mat.SetFloat(Uniforms._Threshold, thresholdLinear);
      float num5 = (float) (thresholdLinear * (double) bloom.softKnee + 9.9999997473787516E-06);
      Vector3 vector3 = new Vector3(thresholdLinear - num5, num5 * 2f, 0.25f / num5);
      mat.SetVector(Uniforms._Curve, vector3);
      mat.SetFloat(Uniforms._PrefilterOffs, bloom.antiFlicker ? -0.5f : 0.0f);
      float x = 0.5f + num2 - num3;
      mat.SetFloat(Uniforms._SampleScale, x);
      if (bloom.antiFlicker)
        mat.EnableKeyword("ANTI_FLICKER");
      RenderTexture renderTexture1 = context.renderTextureFactory.Get(width, num1, format: format);
      Graphics.Blit(source, renderTexture1, mat, 0);
      RenderTexture source1 = renderTexture1;
      for (int index = 0; index < num4; ++index)
      {
        m_BlurBuffer1[index] = context.renderTextureFactory.Get(Mathf.Max(1, source1.width / 2), Mathf.Max(1, source1.height / 2), format: format);
        int pass = index == 0 ? 1 : 2;
        Graphics.Blit(source1, m_BlurBuffer1[index], mat, pass);
        source1 = m_BlurBuffer1[index];
      }
      for (int index = num4 - 2; index >= 0; --index)
      {
        RenderTexture renderTexture2 = m_BlurBuffer1[index];
        mat.SetTexture(Uniforms._BaseTex, renderTexture2);
        m_BlurBuffer2[index] = context.renderTextureFactory.Get(renderTexture2.width, renderTexture2.height, format: format);
        Graphics.Blit(source1, m_BlurBuffer2[index], mat, 3);
        source1 = m_BlurBuffer2[index];
      }
      RenderTexture renderTexture3 = source1;
      for (int index = 0; index < 16; ++index)
      {
        if (m_BlurBuffer1[index] != null)
          context.renderTextureFactory.Release(m_BlurBuffer1[index]);
        if (m_BlurBuffer2[index] != null && m_BlurBuffer2[index] != renderTexture3)
          context.renderTextureFactory.Release(m_BlurBuffer2[index]);
        m_BlurBuffer1[index] = null;
        m_BlurBuffer2[index] = null;
      }
      context.renderTextureFactory.Release(renderTexture1);
      uberMaterial.SetTexture(Uniforms._BloomTex, renderTexture3);
      uberMaterial.SetVector(Uniforms._Bloom_Settings, new Vector2(x, bloom.intensity));
      if (lensDirt.intensity > 0.0 && lensDirt.texture != null)
      {
        uberMaterial.SetTexture(Uniforms._Bloom_DirtTex, lensDirt.texture);
        uberMaterial.SetFloat(Uniforms._Bloom_DirtIntensity, lensDirt.intensity);
        uberMaterial.EnableKeyword("BLOOM_LENS_DIRT");
      }
      else
        uberMaterial.EnableKeyword("BLOOM");
    }

    private static class Uniforms
    {
      internal static readonly int _AutoExposure = Shader.PropertyToID(nameof (_AutoExposure));
      internal static readonly int _Threshold = Shader.PropertyToID(nameof (_Threshold));
      internal static readonly int _Curve = Shader.PropertyToID(nameof (_Curve));
      internal static readonly int _PrefilterOffs = Shader.PropertyToID(nameof (_PrefilterOffs));
      internal static readonly int _SampleScale = Shader.PropertyToID(nameof (_SampleScale));
      internal static readonly int _BaseTex = Shader.PropertyToID(nameof (_BaseTex));
      internal static readonly int _BloomTex = Shader.PropertyToID(nameof (_BloomTex));
      internal static readonly int _Bloom_Settings = Shader.PropertyToID(nameof (_Bloom_Settings));
      internal static readonly int _Bloom_DirtTex = Shader.PropertyToID(nameof (_Bloom_DirtTex));
      internal static readonly int _Bloom_DirtIntensity = Shader.PropertyToID(nameof (_Bloom_DirtIntensity));
    }
  }
}
