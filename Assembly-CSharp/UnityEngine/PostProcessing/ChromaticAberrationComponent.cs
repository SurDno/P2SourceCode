namespace UnityEngine.PostProcessing
{
  public sealed class ChromaticAberrationComponent : 
    PostProcessingComponentRenderTexture<ChromaticAberrationModel>
  {
    private Texture2D m_SpectrumLut;

    public override bool active
    {
      get
      {
        return model.enabled && model.settings.intensity > 0.0 && !context.interrupted;
      }
    }

    public override void OnDisable()
    {
      GraphicsUtils.Destroy(m_SpectrumLut);
      m_SpectrumLut = null;
    }

    public override void Prepare(Material uberMaterial)
    {
      ChromaticAberrationModel.Settings settings = model.settings;
      Texture2D texture2D1 = settings.spectralTexture;
      if (texture2D1 == null)
      {
        if (m_SpectrumLut == null)
        {
          Texture2D texture2D2 = new Texture2D(3, 1, TextureFormat.RGB24, false);
          texture2D2.name = "Chromatic Aberration Spectrum Lookup";
          texture2D2.filterMode = FilterMode.Bilinear;
          texture2D2.wrapMode = TextureWrapMode.Clamp;
          texture2D2.anisoLevel = 0;
          texture2D2.hideFlags = HideFlags.DontSave;
          m_SpectrumLut = texture2D2;
          m_SpectrumLut.SetPixels(new Color[3]
          {
            new Color(1f, 0.0f, 0.0f),
            new Color(0.0f, 1f, 0.0f),
            new Color(0.0f, 0.0f, 1f)
          });
          m_SpectrumLut.Apply();
        }
        texture2D1 = m_SpectrumLut;
      }
      uberMaterial.EnableKeyword("CHROMATIC_ABERRATION");
      uberMaterial.SetFloat(Uniforms._ChromaticAberration_Amount, settings.intensity * 0.03f);
      uberMaterial.SetTexture(Uniforms._ChromaticAberration_Spectrum, texture2D1);
    }

    private static class Uniforms
    {
      internal static readonly int _ChromaticAberration_Amount = Shader.PropertyToID(nameof (_ChromaticAberration_Amount));
      internal static readonly int _ChromaticAberration_Spectrum = Shader.PropertyToID(nameof (_ChromaticAberration_Spectrum));
    }
  }
}
