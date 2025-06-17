using System;

namespace UnityEngine.PostProcessing
{
  public sealed class DepthOfFieldComponent : PostProcessingComponentRenderTexture<DepthOfFieldModel>
  {
    private const string k_ShaderString = "Hidden/Post FX/Depth Of Field";
    private RenderTexture m_CoCHistory;
    private const float k_FilmHeight = 0.024f;

    public override bool active => model.enabled && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf) && !context.interrupted;

    public override DepthTextureMode GetCameraFlags() => DepthTextureMode.Depth;

    private float CalculateFocalLength()
    {
      DepthOfFieldModel.Settings settings = model.settings;
      return !settings.useCameraFov ? settings.focalLength / 1000f : 0.012f / Mathf.Tan(0.5f * (context.camera.fieldOfView * ((float) Math.PI / 180f)));
    }

    private float CalculateMaxCoCRadius(int screenHeight)
    {
      return Mathf.Min(0.05f, (float) ((double) model.settings.kernelSize * 4.0 + 6.0) / screenHeight);
    }

    private bool CheckHistory(int width, int height)
    {
      return m_CoCHistory != null && m_CoCHistory.IsCreated() && m_CoCHistory.width == width && m_CoCHistory.height == height;
    }

    private RenderTextureFormat SelectFormat(
      RenderTextureFormat primary,
      RenderTextureFormat secondary)
    {
      if (SystemInfo.SupportsRenderTextureFormat(primary))
        return primary;
      return SystemInfo.SupportsRenderTextureFormat(secondary) ? secondary : RenderTextureFormat.Default;
    }

    public void Prepare(
      RenderTexture source,
      Material uberMaterial,
      bool antialiasCoC,
      Vector2 taaJitter,
      float taaBlending)
    {
      DepthOfFieldModel.Settings settings = model.settings;
      RenderTextureFormat format1 = RenderTextureFormat.ARGBHalf;
      RenderTextureFormat format2 = SelectFormat(RenderTextureFormat.R8, RenderTextureFormat.RHalf);
      float focalLength = CalculateFocalLength();
      float x = Mathf.Max(settings.focusDistance, focalLength);
      float num = source.width / (float) source.height;
      float y = (float) (focalLength * (double) focalLength / (settings.aperture * (x - (double) focalLength) * 0.024000000208616257 * 2.0));
      float maxCoCradius = CalculateMaxCoCRadius(source.height);
      Material mat = context.materialFactory.Get("Hidden/Post FX/Depth Of Field");
      mat.SetFloat(Uniforms._Distance, x);
      mat.SetFloat(Uniforms._LensCoeff, y);
      mat.SetFloat(Uniforms._MaxCoC, maxCoCradius);
      mat.SetFloat(Uniforms._RcpMaxCoC, 1f / maxCoCradius);
      mat.SetFloat(Uniforms._RcpAspect, 1f / num);
      RenderTexture renderTexture1 = context.renderTextureFactory.Get(Mathf.Max(1, context.width), Mathf.Max(1, context.height), format: format2);
      Graphics.Blit(null, renderTexture1, mat, 0);
      if (antialiasCoC)
      {
        mat.SetTexture(Uniforms._CoCTex, renderTexture1);
        float z = CheckHistory(context.width, context.height) ? taaBlending : 0.0f;
        mat.SetVector(Uniforms._TaaParams, new Vector3(taaJitter.x, taaJitter.y, z));
        RenderTexture temporary = RenderTexture.GetTemporary(context.width, context.height, 0, format2);
        Graphics.Blit(m_CoCHistory, temporary, mat, 1);
        context.renderTextureFactory.Release(renderTexture1);
        if (m_CoCHistory != null)
          RenderTexture.ReleaseTemporary(m_CoCHistory);
        m_CoCHistory = renderTexture1 = temporary;
      }
      int width = Mathf.Max(1, context.width / 2);
      int height = Mathf.Max(1, context.height / 2);
      RenderTexture renderTexture2 = context.renderTextureFactory.Get(width, height, format: format1);
      mat.SetTexture(Uniforms._CoCTex, renderTexture1);
      Graphics.Blit(source, renderTexture2, mat, 2);
      RenderTexture renderTexture3 = context.renderTextureFactory.Get(width, height, format: format1);
      Graphics.Blit(renderTexture2, renderTexture3, mat, (int) (3 + settings.kernelSize));
      Graphics.Blit(renderTexture3, renderTexture2, mat, 7);
      uberMaterial.SetVector(Uniforms._DepthOfFieldParams, new Vector3(x, y, maxCoCradius));
      if (context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.FocusPlane))
      {
        uberMaterial.EnableKeyword("DEPTH_OF_FIELD_COC_VIEW");
        context.Interrupt();
      }
      else
      {
        uberMaterial.SetTexture(Uniforms._DepthOfFieldTex, renderTexture2);
        uberMaterial.SetTexture(Uniforms._DepthOfFieldCoCTex, renderTexture1);
        uberMaterial.EnableKeyword("DEPTH_OF_FIELD");
      }
      context.renderTextureFactory.Release(renderTexture3);
    }

    public override void OnDisable()
    {
      if (m_CoCHistory != null)
        RenderTexture.ReleaseTemporary(m_CoCHistory);
      m_CoCHistory = null;
    }

    private static class Uniforms
    {
      internal static readonly int _DepthOfFieldTex = Shader.PropertyToID(nameof (_DepthOfFieldTex));
      internal static readonly int _DepthOfFieldCoCTex = Shader.PropertyToID(nameof (_DepthOfFieldCoCTex));
      internal static readonly int _Distance = Shader.PropertyToID(nameof (_Distance));
      internal static readonly int _LensCoeff = Shader.PropertyToID(nameof (_LensCoeff));
      internal static readonly int _MaxCoC = Shader.PropertyToID(nameof (_MaxCoC));
      internal static readonly int _RcpMaxCoC = Shader.PropertyToID(nameof (_RcpMaxCoC));
      internal static readonly int _RcpAspect = Shader.PropertyToID(nameof (_RcpAspect));
      internal static readonly int _MainTex = Shader.PropertyToID(nameof (_MainTex));
      internal static readonly int _CoCTex = Shader.PropertyToID(nameof (_CoCTex));
      internal static readonly int _TaaParams = Shader.PropertyToID(nameof (_TaaParams));
      internal static readonly int _DepthOfFieldParams = Shader.PropertyToID(nameof (_DepthOfFieldParams));
    }
  }
}
