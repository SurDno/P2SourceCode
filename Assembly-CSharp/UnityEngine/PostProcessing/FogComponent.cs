using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
  public sealed class FogComponent : PostProcessingComponentCommandBuffer<FogModel>
  {
    private const string k_ShaderString = "Hidden/Post FX/Fog";

    public override bool active => model.enabled && context.isGBufferAvailable && RenderSettings.fog && !context.interrupted;

    public override string GetName() => "Fog";

    public override DepthTextureMode GetCameraFlags() => DepthTextureMode.Depth;

    public override CameraEvent GetCameraEvent() => CameraEvent.AfterImageEffectsOpaque;

    public override void PopulateCommandBuffer(CommandBuffer cb)
    {
      FogModel.Settings settings = model.settings;
      Material mat = context.materialFactory.Get("Hidden/Post FX/Fog");
      mat.shaderKeywords = null;
      Color color = GraphicsUtils.isLinearColorSpace ? RenderSettings.fogColor.linear : RenderSettings.fogColor;
      mat.SetColor(Uniforms._FogColor, color);
      mat.SetFloat(Uniforms._Density, RenderSettings.fogDensity);
      mat.SetFloat(Uniforms._Start, RenderSettings.fogStartDistance);
      mat.SetFloat(Uniforms._End, RenderSettings.fogEndDistance);
      switch (RenderSettings.fogMode)
      {
        case FogMode.Linear:
          mat.EnableKeyword("FOG_LINEAR");
          break;
        case FogMode.Exponential:
          mat.EnableKeyword("FOG_EXP");
          break;
        case FogMode.ExponentialSquared:
          mat.EnableKeyword("FOG_EXP2");
          break;
      }
      RenderTextureFormat format = context.isHdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
      cb.GetTemporaryRT(Uniforms._TempRT, context.width, context.height, 24, FilterMode.Bilinear, format);
      cb.Blit(BuiltinRenderTextureType.CameraTarget, Uniforms._TempRT);
      cb.Blit(Uniforms._TempRT, BuiltinRenderTextureType.CameraTarget, mat, settings.excludeSkybox ? 1 : 0);
      cb.ReleaseTemporaryRT(Uniforms._TempRT);
    }

    private static class Uniforms
    {
      internal static readonly int _FogColor = Shader.PropertyToID(nameof (_FogColor));
      internal static readonly int _Density = Shader.PropertyToID(nameof (_Density));
      internal static readonly int _Start = Shader.PropertyToID(nameof (_Start));
      internal static readonly int _End = Shader.PropertyToID(nameof (_End));
      internal static readonly int _TempRT = Shader.PropertyToID(nameof (_TempRT));
    }
  }
}
