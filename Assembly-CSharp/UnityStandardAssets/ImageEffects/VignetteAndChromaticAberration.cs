namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Camera/Vignette and Chromatic Aberration")]
  public class VignetteAndChromaticAberration : PostEffectsBase
  {
    public AberrationMode mode = AberrationMode.Simple;
    public float intensity = 0.036f;
    public float chromaticAberration = 0.2f;
    public float axialAberration = 0.5f;
    public float blur = 0.0f;
    public float blurSpread = 0.75f;
    public float luminanceDependency = 0.25f;
    public float blurDistance = 2.5f;
    public Shader vignetteShader;
    public Shader separableBlurShader;
    public Shader chromAberrationShader;
    private Material m_VignetteMaterial;
    private Material m_SeparableBlurMaterial;
    private Material m_ChromAberrationMaterial;

    public override bool CheckResources()
    {
      CheckSupport(false);
      m_VignetteMaterial = CheckShaderAndCreateMaterial(vignetteShader, m_VignetteMaterial);
      m_SeparableBlurMaterial = CheckShaderAndCreateMaterial(separableBlurShader, m_SeparableBlurMaterial);
      m_ChromAberrationMaterial = CheckShaderAndCreateMaterial(chromAberrationShader, m_ChromAberrationMaterial);
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
        bool flag = (double) Mathf.Abs(blur) > 0.0 || (double) Mathf.Abs(intensity) > 0.0;
        float num = (float) (1.0 * width / (1.0 * height));
        RenderTexture renderTexture1 = (RenderTexture) null;
        RenderTexture renderTexture2 = (RenderTexture) null;
        if (flag)
        {
          renderTexture1 = RenderTexture.GetTemporary(width, height, 0, source.format);
          if ((double) Mathf.Abs(blur) > 0.0)
          {
            renderTexture2 = RenderTexture.GetTemporary(width / 2, height / 2, 0, source.format);
            Graphics.Blit((Texture) source, renderTexture2, m_ChromAberrationMaterial, 0);
            for (int index = 0; index < 2; ++index)
            {
              m_SeparableBlurMaterial.SetVector("offsets", new Vector4(0.0f, blurSpread * (1f / 512f), 0.0f, 0.0f));
              RenderTexture temporary = RenderTexture.GetTemporary(width / 2, height / 2, 0, source.format);
              Graphics.Blit((Texture) renderTexture2, temporary, m_SeparableBlurMaterial);
              RenderTexture.ReleaseTemporary(renderTexture2);
              m_SeparableBlurMaterial.SetVector("offsets", new Vector4(blurSpread * (1f / 512f) / num, 0.0f, 0.0f, 0.0f));
              renderTexture2 = RenderTexture.GetTemporary(width / 2, height / 2, 0, source.format);
              Graphics.Blit((Texture) temporary, renderTexture2, m_SeparableBlurMaterial);
              RenderTexture.ReleaseTemporary(temporary);
            }
          }
          m_VignetteMaterial.SetFloat("_Intensity", (float) (1.0 / (1.0 - intensity) - 1.0));
          m_VignetteMaterial.SetFloat("_Blur", (float) (1.0 / (1.0 - blur) - 1.0));
          m_VignetteMaterial.SetTexture("_VignetteTex", (Texture) renderTexture2);
          Graphics.Blit((Texture) source, renderTexture1, m_VignetteMaterial, 0);
        }
        m_ChromAberrationMaterial.SetFloat("_ChromaticAberration", chromaticAberration);
        m_ChromAberrationMaterial.SetFloat("_AxialAberration", axialAberration);
        m_ChromAberrationMaterial.SetVector("_BlurDistance", (Vector4) new Vector2(-blurDistance, blurDistance));
        m_ChromAberrationMaterial.SetFloat("_Luminance", 1f / Mathf.Max(Mathf.Epsilon, luminanceDependency));
        if (flag)
          renderTexture1.wrapMode = TextureWrapMode.Clamp;
        else
          source.wrapMode = TextureWrapMode.Clamp;
        Graphics.Blit(flag ? (Texture) renderTexture1 : (Texture) source, destination, m_ChromAberrationMaterial, mode == AberrationMode.Advanced ? 2 : 1);
        RenderTexture.ReleaseTemporary(renderTexture1);
        RenderTexture.ReleaseTemporary(renderTexture2);
      }
    }

    public enum AberrationMode
    {
      Simple,
      Advanced,
    }
  }
}
