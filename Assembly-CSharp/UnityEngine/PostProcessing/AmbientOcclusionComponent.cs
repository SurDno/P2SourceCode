using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
  public sealed class AmbientOcclusionComponent : 
    PostProcessingComponentCommandBuffer<AmbientOcclusionModel>
  {
    private const string k_BlitShaderString = "Hidden/Post FX/Blit";
    private const string k_ShaderString = "Hidden/Post FX/Ambient Occlusion";
    private readonly RenderTargetIdentifier[] m_MRT = [
      BuiltinRenderTextureType.GBuffer0,
      BuiltinRenderTextureType.CameraTarget
    ];

    private OcclusionSource occlusionSource
    {
      get
      {
        if (context.isGBufferAvailable && !model.settings.forceForwardCompatibility)
          return OcclusionSource.GBuffer;
        return model.settings.highPrecision && (!context.isGBufferAvailable || model.settings.forceForwardCompatibility) ? OcclusionSource.DepthTexture : OcclusionSource.DepthNormalsTexture;
      }
    }

    private bool ambientOnlySupported => context.isHdr && model.settings.ambientOnly && context.isGBufferAvailable && !model.settings.forceForwardCompatibility;

    public override bool active => model.enabled && model.settings.intensity > 0.0 && !context.interrupted;

    public override DepthTextureMode GetCameraFlags()
    {
      DepthTextureMode cameraFlags = DepthTextureMode.None;
      if (occlusionSource == OcclusionSource.DepthTexture)
        cameraFlags |= DepthTextureMode.Depth;
      if (occlusionSource != OcclusionSource.GBuffer)
        cameraFlags |= DepthTextureMode.DepthNormals;
      return cameraFlags;
    }

    public override string GetName() => "Ambient Occlusion";

    public override CameraEvent GetCameraEvent()
    {
      return !ambientOnlySupported || context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.AmbientOcclusion) ? CameraEvent.BeforeImageEffectsOpaque : CameraEvent.BeforeReflections;
    }

    public override void PopulateCommandBuffer(CommandBuffer cb)
    {
      AmbientOcclusionModel.Settings settings = model.settings;
      Material mat = context.materialFactory.Get("Hidden/Post FX/Blit");
      Material material = context.materialFactory.Get("Hidden/Post FX/Ambient Occlusion");
      material.shaderKeywords = null;
      material.SetFloat(Uniforms._Intensity, settings.intensity);
      material.SetFloat(Uniforms._Radius, settings.radius);
      material.SetFloat(Uniforms._Downsample, settings.downsampling ? 0.5f : 1f);
      material.SetInt(Uniforms._SampleCount, (int) settings.sampleCount);
      if (!context.isGBufferAvailable && RenderSettings.fog)
      {
        material.SetVector(Uniforms._FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
        switch (RenderSettings.fogMode)
        {
          case FogMode.Linear:
            material.EnableKeyword("FOG_LINEAR");
            break;
          case FogMode.Exponential:
            material.EnableKeyword("FOG_EXP");
            break;
          case FogMode.ExponentialSquared:
            material.EnableKeyword("FOG_EXP2");
            break;
        }
      }
      else
        material.EnableKeyword("FOG_OFF");
      int width = context.width;
      int height = context.height;
      int num = settings.downsampling ? 2 : 1;
      int occlusionTexture1 = Uniforms._OcclusionTexture1;
      cb.GetTemporaryRT(occlusionTexture1, width / num, height / num, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
      cb.Blit(null, occlusionTexture1, material, (int) occlusionSource);
      int occlusionTexture2 = Uniforms._OcclusionTexture2;
      cb.GetTemporaryRT(occlusionTexture2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
      cb.SetGlobalTexture(Uniforms._MainTex, occlusionTexture1);
      cb.Blit(occlusionTexture1, occlusionTexture2, material, occlusionSource == OcclusionSource.GBuffer ? 4 : 3);
      cb.ReleaseTemporaryRT(occlusionTexture1);
      int occlusionTexture = Uniforms._OcclusionTexture;
      cb.GetTemporaryRT(occlusionTexture, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
      cb.SetGlobalTexture(Uniforms._MainTex, occlusionTexture2);
      cb.Blit(occlusionTexture2, occlusionTexture, material, 5);
      cb.ReleaseTemporaryRT(occlusionTexture2);
      if (context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.AmbientOcclusion))
      {
        cb.SetGlobalTexture(Uniforms._MainTex, occlusionTexture);
        cb.Blit(occlusionTexture, BuiltinRenderTextureType.CameraTarget, material, 8);
        context.Interrupt();
      }
      else if (ambientOnlySupported)
      {
        cb.SetRenderTarget(m_MRT, BuiltinRenderTextureType.CameraTarget);
        cb.DrawMesh(GraphicsUtils.quad, Matrix4x4.identity, material, 0, 7);
      }
      else
      {
        RenderTextureFormat format = context.isHdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
        int tempRt = Uniforms._TempRT;
        cb.GetTemporaryRT(tempRt, context.width, context.height, 0, FilterMode.Bilinear, format);
        cb.Blit(BuiltinRenderTextureType.CameraTarget, tempRt, mat, 0);
        cb.SetGlobalTexture(Uniforms._MainTex, tempRt);
        cb.Blit(tempRt, BuiltinRenderTextureType.CameraTarget, material, 6);
        cb.ReleaseTemporaryRT(tempRt);
      }
      cb.ReleaseTemporaryRT(occlusionTexture);
    }

    private static class Uniforms
    {
      internal static readonly int _Intensity = Shader.PropertyToID(nameof (_Intensity));
      internal static readonly int _Radius = Shader.PropertyToID(nameof (_Radius));
      internal static readonly int _FogParams = Shader.PropertyToID(nameof (_FogParams));
      internal static readonly int _Downsample = Shader.PropertyToID(nameof (_Downsample));
      internal static readonly int _SampleCount = Shader.PropertyToID(nameof (_SampleCount));
      internal static readonly int _OcclusionTexture1 = Shader.PropertyToID(nameof (_OcclusionTexture1));
      internal static readonly int _OcclusionTexture2 = Shader.PropertyToID(nameof (_OcclusionTexture2));
      internal static readonly int _OcclusionTexture = Shader.PropertyToID(nameof (_OcclusionTexture));
      internal static readonly int _MainTex = Shader.PropertyToID(nameof (_MainTex));
      internal static readonly int _TempRT = Shader.PropertyToID(nameof (_TempRT));
    }

    private enum OcclusionSource
    {
      DepthTexture,
      DepthNormalsTexture,
      GBuffer,
    }
  }
}
