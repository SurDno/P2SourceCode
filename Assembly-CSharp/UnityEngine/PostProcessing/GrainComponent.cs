namespace UnityEngine.PostProcessing
{
  public sealed class GrainComponent : PostProcessingComponentRenderTexture<GrainModel>
  {
    private RenderTexture m_GrainLookupRT;

    public override bool active
    {
      get
      {
        return model.enabled && model.settings.intensity > 0.0 && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf) && !context.interrupted;
      }
    }

    public override void OnDisable()
    {
      GraphicsUtils.Destroy((Object) m_GrainLookupRT);
      m_GrainLookupRT = (RenderTexture) null;
    }

    public override void Prepare(Material uberMaterial)
    {
      GrainModel.Settings settings = model.settings;
      uberMaterial.EnableKeyword("GRAIN");
      float realtimeSinceStartup = Time.realtimeSinceStartup;
      float z = Random.value;
      float w = Random.value;
      if ((Object) m_GrainLookupRT == (Object) null || !m_GrainLookupRT.IsCreated())
      {
        GraphicsUtils.Destroy((Object) m_GrainLookupRT);
        RenderTexture renderTexture = new RenderTexture(192, 192, 0, RenderTextureFormat.ARGBHalf);
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.wrapMode = TextureWrapMode.Repeat;
        renderTexture.anisoLevel = 0;
        renderTexture.name = "Grain Lookup Texture";
        m_GrainLookupRT = renderTexture;
        m_GrainLookupRT.Create();
      }
      Material mat = context.materialFactory.Get("Hidden/Post FX/Grain Generator");
      mat.SetFloat(Uniforms._Phase, realtimeSinceStartup / 20f);
      Graphics.Blit((Texture) null, m_GrainLookupRT, mat, settings.colored ? 1 : 0);
      uberMaterial.SetTexture(Uniforms._GrainTex, (Texture) m_GrainLookupRT);
      uberMaterial.SetVector(Uniforms._Grain_Params1, (Vector4) new Vector2(settings.luminanceContribution, settings.intensity * 20f));
      uberMaterial.SetVector(Uniforms._Grain_Params2, new Vector4(context.width / (float) m_GrainLookupRT.width / settings.size, context.height / (float) m_GrainLookupRT.height / settings.size, z, w));
    }

    private static class Uniforms
    {
      internal static readonly int _Grain_Params1 = Shader.PropertyToID(nameof (_Grain_Params1));
      internal static readonly int _Grain_Params2 = Shader.PropertyToID(nameof (_Grain_Params2));
      internal static readonly int _GrainTex = Shader.PropertyToID(nameof (_GrainTex));
      internal static readonly int _Phase = Shader.PropertyToID(nameof (_Phase));
    }
  }
}
