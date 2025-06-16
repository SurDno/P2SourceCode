using System;

namespace UnityEngine.PostProcessing
{
  public sealed class EyeAdaptationComponent : 
    PostProcessingComponentRenderTexture<EyeAdaptationModel>
  {
    private ComputeShader m_EyeCompute;
    private ComputeBuffer m_HistogramBuffer;
    private readonly RenderTexture[] m_AutoExposurePool = new RenderTexture[2];
    private int m_AutoExposurePingPing;
    private RenderTexture m_CurrentAutoExposure;
    private RenderTexture m_DebugHistogram;
    private static uint[] s_EmptyHistogramBuffer;
    private bool m_FirstFrame = true;
    private const int k_HistogramBins = 64;
    private const int k_HistogramThreadX = 16;
    private const int k_HistogramThreadY = 16;

    public override bool active
    {
      get => model.enabled && SystemInfo.supportsComputeShaders && !context.interrupted;
    }

    public void ResetHistory() => m_FirstFrame = true;

    public override void OnEnable() => m_FirstFrame = true;

    public override void OnDisable()
    {
      foreach (UnityEngine.Object @object in m_AutoExposurePool)
        GraphicsUtils.Destroy(@object);
      if (m_HistogramBuffer != null)
        m_HistogramBuffer.Release();
      m_HistogramBuffer = (ComputeBuffer) null;
      if ((UnityEngine.Object) m_DebugHistogram != (UnityEngine.Object) null)
        m_DebugHistogram.Release();
      m_DebugHistogram = (RenderTexture) null;
    }

    private Vector4 GetHistogramScaleOffsetRes()
    {
      EyeAdaptationModel.Settings settings = model.settings;
      float x = 1f / (settings.logMax - settings.logMin);
      float y = -settings.logMin * x;
      return new Vector4(x, y, Mathf.Max(1f, Mathf.Floor(context.width / 2f)), Mathf.Max(1f, Mathf.Floor(context.height / 2f)));
    }

    public Texture Prepare(RenderTexture source, Material uberMaterial)
    {
      EyeAdaptationModel.Settings settings = model.settings;
      if ((UnityEngine.Object) m_EyeCompute == (UnityEngine.Object) null)
        m_EyeCompute = Resources.Load<ComputeShader>("Shaders/EyeHistogram");
      Material mat = context.materialFactory.Get("Hidden/Post FX/Eye Adaptation");
      mat.shaderKeywords = (string[]) null;
      if (m_HistogramBuffer == null)
        m_HistogramBuffer = new ComputeBuffer(64, 4);
      if (s_EmptyHistogramBuffer == null)
        s_EmptyHistogramBuffer = new uint[64];
      Vector4 histogramScaleOffsetRes = GetHistogramScaleOffsetRes();
      RenderTexture renderTexture1 = context.renderTextureFactory.Get((int) histogramScaleOffsetRes.z, (int) histogramScaleOffsetRes.w, format: source.format);
      Graphics.Blit((Texture) source, renderTexture1);
      if ((UnityEngine.Object) m_AutoExposurePool[0] == (UnityEngine.Object) null || !m_AutoExposurePool[0].IsCreated())
        m_AutoExposurePool[0] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
      if ((UnityEngine.Object) m_AutoExposurePool[1] == (UnityEngine.Object) null || !m_AutoExposurePool[1].IsCreated())
        m_AutoExposurePool[1] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
      m_HistogramBuffer.SetData((Array) s_EmptyHistogramBuffer);
      int kernel = m_EyeCompute.FindKernel("KEyeHistogram");
      m_EyeCompute.SetBuffer(kernel, "_Histogram", m_HistogramBuffer);
      m_EyeCompute.SetTexture(kernel, "_Source", (Texture) renderTexture1);
      m_EyeCompute.SetVector("_ScaleOffsetRes", histogramScaleOffsetRes);
      m_EyeCompute.Dispatch(kernel, Mathf.CeilToInt((float) renderTexture1.width / 16f), Mathf.CeilToInt((float) renderTexture1.height / 16f), 1);
      context.renderTextureFactory.Release(renderTexture1);
      settings.highPercent = Mathf.Clamp(settings.highPercent, 1.01f, 99f);
      settings.lowPercent = Mathf.Clamp(settings.lowPercent, 1f, settings.highPercent - 0.01f);
      mat.SetBuffer("_Histogram", m_HistogramBuffer);
      mat.SetVector(Uniforms._Params, new Vector4(settings.lowPercent * 0.01f, settings.highPercent * 0.01f, Mathf.Exp(settings.minLuminance * 0.6931472f), Mathf.Exp(settings.maxLuminance * 0.6931472f)));
      mat.SetVector(Uniforms._Speed, (Vector4) new Vector2(settings.speedDown, settings.speedUp));
      mat.SetVector(Uniforms._ScaleOffsetRes, histogramScaleOffsetRes);
      mat.SetFloat(Uniforms._ExposureCompensation, settings.keyValue);
      if (settings.dynamicKeyValue)
        mat.EnableKeyword("AUTO_KEY_VALUE");
      if (m_FirstFrame || !Application.isPlaying)
      {
        m_CurrentAutoExposure = m_AutoExposurePool[0];
        Graphics.Blit((Texture) null, m_CurrentAutoExposure, mat, 1);
        Graphics.Blit((Texture) m_AutoExposurePool[0], m_AutoExposurePool[1]);
      }
      else
      {
        int exposurePingPing = m_AutoExposurePingPing;
        int num1;
        RenderTexture source1 = m_AutoExposurePool[(num1 = exposurePingPing + 1) % 2];
        int num2;
        RenderTexture dest = m_AutoExposurePool[(num2 = num1 + 1) % 2];
        Graphics.Blit((Texture) source1, dest, mat, (int) settings.adaptationType);
        int num3;
        m_AutoExposurePingPing = (num3 = num2 + 1) % 2;
        m_CurrentAutoExposure = dest;
      }
      if (context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.EyeAdaptation))
      {
        if ((UnityEngine.Object) m_DebugHistogram == (UnityEngine.Object) null || !m_DebugHistogram.IsCreated())
        {
          RenderTexture renderTexture2 = new RenderTexture(256, 128, 0, RenderTextureFormat.ARGB32);
          renderTexture2.filterMode = FilterMode.Point;
          renderTexture2.wrapMode = TextureWrapMode.Clamp;
          m_DebugHistogram = renderTexture2;
        }
        mat.SetFloat(Uniforms._DebugWidth, (float) m_DebugHistogram.width);
        Graphics.Blit((Texture) null, m_DebugHistogram, mat, 2);
      }
      m_FirstFrame = false;
      return (Texture) m_CurrentAutoExposure;
    }

    public void OnGUI()
    {
      if ((UnityEngine.Object) m_DebugHistogram == (UnityEngine.Object) null || !m_DebugHistogram.IsCreated())
        return;
      GUI.DrawTexture(new Rect((float) ((double) context.viewport.x * (double) Screen.width + 8.0), 8f, (float) m_DebugHistogram.width, (float) m_DebugHistogram.height), (Texture) m_DebugHistogram);
    }

    private static class Uniforms
    {
      internal static readonly int _Params = Shader.PropertyToID(nameof (_Params));
      internal static readonly int _Speed = Shader.PropertyToID(nameof (_Speed));
      internal static readonly int _ScaleOffsetRes = Shader.PropertyToID(nameof (_ScaleOffsetRes));
      internal static readonly int _ExposureCompensation = Shader.PropertyToID(nameof (_ExposureCompensation));
      internal static readonly int _AutoExposure = Shader.PropertyToID(nameof (_AutoExposure));
      internal static readonly int _DebugWidth = Shader.PropertyToID(nameof (_DebugWidth));
    }
  }
}
