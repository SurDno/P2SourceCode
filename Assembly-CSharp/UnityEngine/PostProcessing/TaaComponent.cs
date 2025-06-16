using System;

namespace UnityEngine.PostProcessing
{
  public sealed class TaaComponent : PostProcessingComponentRenderTexture<AntialiasingModel>
  {
    private const string k_ShaderString = "Hidden/Post FX/Temporal Anti-aliasing";
    private const int k_SampleCount = 8;
    private readonly RenderBuffer[] m_MRT = new RenderBuffer[2];
    private int m_SampleIndex;
    private bool m_ResetHistory = true;
    private RenderTexture m_HistoryTexture;

    public override bool active
    {
      get
      {
        return model.enabled && model.settings.method == AntialiasingModel.Method.Taa && SystemInfo.supportsMotionVectors && SystemInfo.supportedRenderTargetCount >= 2 && !context.interrupted;
      }
    }

    public override DepthTextureMode GetCameraFlags()
    {
      return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
    }

    public Vector2 jitterVector { get; private set; }

    public void ResetHistory() => m_ResetHistory = true;

    public void SetProjectionMatrix(Func<Vector2, Matrix4x4> jitteredFunc)
    {
      AntialiasingModel.TaaSettings taaSettings = model.settings.taaSettings;
      Vector2 offset = GenerateRandomOffset() * taaSettings.jitterSpread;
      context.camera.nonJitteredProjectionMatrix = context.camera.projectionMatrix;
      if (jitteredFunc != null)
        context.camera.projectionMatrix = jitteredFunc(offset);
      else
        context.camera.projectionMatrix = context.camera.orthographic ? GetOrthographicProjectionMatrix(offset) : GetPerspectiveProjectionMatrix(offset);
      context.camera.useJitteredProjectionMatrixForTransparentRendering = true;
      offset.x /= (float) context.width;
      offset.y /= (float) context.height;
      context.materialFactory.Get("Hidden/Post FX/Temporal Anti-aliasing").SetVector(Uniforms._Jitter, (Vector4) offset);
      jitterVector = offset;
    }

    public void Render(RenderTexture source, RenderTexture destination)
    {
      Material material = context.materialFactory.Get("Hidden/Post FX/Temporal Anti-aliasing");
      material.shaderKeywords = (string[]) null;
      AntialiasingModel.TaaSettings taaSettings = model.settings.taaSettings;
      if (m_ResetHistory || (UnityEngine.Object) m_HistoryTexture == (UnityEngine.Object) null || m_HistoryTexture.width != source.width || m_HistoryTexture.height != source.height)
      {
        if ((bool) (UnityEngine.Object) m_HistoryTexture)
          RenderTexture.ReleaseTemporary(m_HistoryTexture);
        m_HistoryTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        m_HistoryTexture.name = "TAA History";
        Graphics.Blit((Texture) source, m_HistoryTexture, material, 2);
      }
      material.SetVector(Uniforms._SharpenParameters, new Vector4(taaSettings.sharpen, 0.0f, 0.0f, 0.0f));
      material.SetVector(Uniforms._FinalBlendParameters, new Vector4(taaSettings.stationaryBlending, taaSettings.motionBlending, 6000f, 0.0f));
      material.SetTexture(Uniforms._MainTex, (Texture) source);
      material.SetTexture(Uniforms._HistoryTex, (Texture) m_HistoryTexture);
      RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
      temporary.name = "TAA History";
      m_MRT[0] = destination.colorBuffer;
      m_MRT[1] = temporary.colorBuffer;
      Graphics.SetRenderTarget(m_MRT, source.depthBuffer);
      GraphicsUtils.Blit(material, context.camera.orthographic ? 1 : 0);
      RenderTexture.ReleaseTemporary(m_HistoryTexture);
      m_HistoryTexture = temporary;
      m_ResetHistory = false;
    }

    private float GetHaltonValue(int index, int radix)
    {
      float haltonValue = 0.0f;
      float num = 1f / radix;
      while (index > 0)
      {
        haltonValue += index % radix * num;
        index /= radix;
        num /= radix;
      }
      return haltonValue;
    }

    private Vector2 GenerateRandomOffset()
    {
      Vector2 randomOffset = new Vector2(GetHaltonValue(m_SampleIndex & 1023, 2), GetHaltonValue(m_SampleIndex & 1023, 3));
      if (++m_SampleIndex >= 8)
        m_SampleIndex = 0;
      return randomOffset;
    }

    private Matrix4x4 GetPerspectiveProjectionMatrix(Vector2 offset)
    {
      float num1 = Mathf.Tan((float) Math.PI / 360f * context.camera.fieldOfView);
      float num2 = num1 * context.camera.aspect;
      offset.x *= num2 / (0.5f * context.width);
      offset.y *= num1 / (0.5f * context.height);
      float num3 = (offset.x - num2) * context.camera.nearClipPlane;
      float num4 = (offset.x + num2) * context.camera.nearClipPlane;
      float num5 = (offset.y + num1) * context.camera.nearClipPlane;
      float num6 = (offset.y - num1) * context.camera.nearClipPlane;
      return new Matrix4x4 {
        [0, 0] = (float) (2.0 * (double) context.camera.nearClipPlane / (num4 - (double) num3)),
        [0, 1] = 0.0f,
        [0, 2] = (float) ((num4 + (double) num3) / (num4 - (double) num3)),
        [0, 3] = 0.0f,
        [1, 0] = 0.0f,
        [1, 1] = (float) (2.0 * (double) context.camera.nearClipPlane / (num5 - (double) num6)),
        [1, 2] = (float) ((num5 + (double) num6) / (num5 - (double) num6)),
        [1, 3] = 0.0f,
        [2, 0] = 0.0f,
        [2, 1] = 0.0f,
        [2, 2] = (float) (-((double) context.camera.farClipPlane + (double) context.camera.nearClipPlane) / ((double) context.camera.farClipPlane - (double) context.camera.nearClipPlane)),
        [2, 3] = (float) (-(2.0 * (double) context.camera.farClipPlane * (double) context.camera.nearClipPlane) / ((double) context.camera.farClipPlane - (double) context.camera.nearClipPlane)),
        [3, 0] = 0.0f,
        [3, 1] = 0.0f,
        [3, 2] = -1f,
        [3, 3] = 0.0f
      };
    }

    private Matrix4x4 GetOrthographicProjectionMatrix(Vector2 offset)
    {
      float orthographicSize = context.camera.orthographicSize;
      float num = orthographicSize * context.camera.aspect;
      offset.x *= num / (0.5f * context.width);
      offset.y *= orthographicSize / (0.5f * context.height);
      float left = offset.x - num;
      float right = offset.x + num;
      float top = offset.y + orthographicSize;
      float bottom = offset.y - orthographicSize;
      return Matrix4x4.Ortho(left, right, bottom, top, context.camera.nearClipPlane, context.camera.farClipPlane);
    }

    public override void OnDisable()
    {
      if ((UnityEngine.Object) m_HistoryTexture != (UnityEngine.Object) null)
        RenderTexture.ReleaseTemporary(m_HistoryTexture);
      m_HistoryTexture = (RenderTexture) null;
      m_SampleIndex = 0;
      ResetHistory();
    }

    private static class Uniforms
    {
      internal static int _Jitter = Shader.PropertyToID(nameof (_Jitter));
      internal static int _SharpenParameters = Shader.PropertyToID(nameof (_SharpenParameters));
      internal static int _FinalBlendParameters = Shader.PropertyToID(nameof (_FinalBlendParameters));
      internal static int _HistoryTex = Shader.PropertyToID(nameof (_HistoryTex));
      internal static int _MainTex = Shader.PropertyToID(nameof (_MainTex));
    }
  }
}
