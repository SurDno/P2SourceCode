using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
  public sealed class MotionBlurComponent : PostProcessingComponentCommandBuffer<MotionBlurModel>
  {
    private ReconstructionFilter m_ReconstructionFilter;
    private FrameBlendingFilter m_FrameBlendingFilter;
    private bool m_FirstFrame = true;

    public ReconstructionFilter reconstructionFilter
    {
      get
      {
        if (m_ReconstructionFilter == null)
          m_ReconstructionFilter = new ReconstructionFilter();
        return m_ReconstructionFilter;
      }
    }

    public FrameBlendingFilter frameBlendingFilter
    {
      get
      {
        if (m_FrameBlendingFilter == null)
          m_FrameBlendingFilter = new FrameBlendingFilter();
        return m_FrameBlendingFilter;
      }
    }

    public override bool active
    {
      get
      {
        MotionBlurModel.Settings settings = model.settings;
        return model.enabled && (settings.shutterAngle > 0.0 && reconstructionFilter.IsSupported() || settings.frameBlending > 0.0) && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2 && !context.interrupted;
      }
    }

    public override string GetName() => "Motion Blur";

    public void ResetHistory()
    {
      if (m_FrameBlendingFilter != null)
        m_FrameBlendingFilter.Dispose();
      m_FrameBlendingFilter = null;
      m_FirstFrame = true;
    }

    public override DepthTextureMode GetCameraFlags()
    {
      return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
    }

    public override CameraEvent GetCameraEvent() => CameraEvent.BeforeImageEffects;

    public override void OnEnable() => m_FirstFrame = true;

    public override void PopulateCommandBuffer(CommandBuffer cb)
    {
      if (m_FirstFrame)
      {
        m_FirstFrame = false;
      }
      else
      {
        Material material = context.materialFactory.Get("Hidden/Post FX/Motion Blur");
        Material mat = context.materialFactory.Get("Hidden/Post FX/Blit");
        MotionBlurModel.Settings settings = model.settings;
        RenderTextureFormat format = context.isHdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
        int tempRt = Uniforms._TempRT;
        cb.GetTemporaryRT(tempRt, context.width, context.height, 0, FilterMode.Point, format);
        if (settings.shutterAngle > 0.0 && settings.frameBlending > 0.0)
        {
          reconstructionFilter.ProcessImage(context, cb, ref settings, BuiltinRenderTextureType.CameraTarget, tempRt, material);
          frameBlendingFilter.BlendFrames(cb, settings.frameBlending, tempRt, BuiltinRenderTextureType.CameraTarget, material);
          frameBlendingFilter.PushFrame(cb, tempRt, context.width, context.height, material);
        }
        else if (settings.shutterAngle > 0.0)
        {
          cb.SetGlobalTexture(Uniforms._MainTex, BuiltinRenderTextureType.CameraTarget);
          cb.Blit(BuiltinRenderTextureType.CameraTarget, tempRt, mat, 0);
          reconstructionFilter.ProcessImage(context, cb, ref settings, tempRt, BuiltinRenderTextureType.CameraTarget, material);
        }
        else if (settings.frameBlending > 0.0)
        {
          cb.SetGlobalTexture(Uniforms._MainTex, BuiltinRenderTextureType.CameraTarget);
          cb.Blit(BuiltinRenderTextureType.CameraTarget, tempRt, mat, 0);
          frameBlendingFilter.BlendFrames(cb, settings.frameBlending, tempRt, BuiltinRenderTextureType.CameraTarget, material);
          frameBlendingFilter.PushFrame(cb, tempRt, context.width, context.height, material);
        }
        cb.ReleaseTemporaryRT(tempRt);
      }
    }

    public override void OnDisable()
    {
      if (m_FrameBlendingFilter == null)
        return;
      m_FrameBlendingFilter.Dispose();
    }

    private static class Uniforms
    {
      internal static readonly int _VelocityScale = Shader.PropertyToID(nameof (_VelocityScale));
      internal static readonly int _MaxBlurRadius = Shader.PropertyToID(nameof (_MaxBlurRadius));
      internal static readonly int _RcpMaxBlurRadius = Shader.PropertyToID(nameof (_RcpMaxBlurRadius));
      internal static readonly int _VelocityTex = Shader.PropertyToID(nameof (_VelocityTex));
      internal static readonly int _MainTex = Shader.PropertyToID(nameof (_MainTex));
      internal static readonly int _Tile2RT = Shader.PropertyToID(nameof (_Tile2RT));
      internal static readonly int _Tile4RT = Shader.PropertyToID(nameof (_Tile4RT));
      internal static readonly int _Tile8RT = Shader.PropertyToID(nameof (_Tile8RT));
      internal static readonly int _TileMaxOffs = Shader.PropertyToID(nameof (_TileMaxOffs));
      internal static readonly int _TileMaxLoop = Shader.PropertyToID(nameof (_TileMaxLoop));
      internal static readonly int _TileVRT = Shader.PropertyToID(nameof (_TileVRT));
      internal static readonly int _NeighborMaxTex = Shader.PropertyToID(nameof (_NeighborMaxTex));
      internal static readonly int _LoopCount = Shader.PropertyToID(nameof (_LoopCount));
      internal static readonly int _TempRT = Shader.PropertyToID(nameof (_TempRT));
      internal static readonly int _History1LumaTex = Shader.PropertyToID(nameof (_History1LumaTex));
      internal static readonly int _History2LumaTex = Shader.PropertyToID(nameof (_History2LumaTex));
      internal static readonly int _History3LumaTex = Shader.PropertyToID(nameof (_History3LumaTex));
      internal static readonly int _History4LumaTex = Shader.PropertyToID(nameof (_History4LumaTex));
      internal static readonly int _History1ChromaTex = Shader.PropertyToID(nameof (_History1ChromaTex));
      internal static readonly int _History2ChromaTex = Shader.PropertyToID(nameof (_History2ChromaTex));
      internal static readonly int _History3ChromaTex = Shader.PropertyToID(nameof (_History3ChromaTex));
      internal static readonly int _History4ChromaTex = Shader.PropertyToID(nameof (_History4ChromaTex));
      internal static readonly int _History1Weight = Shader.PropertyToID(nameof (_History1Weight));
      internal static readonly int _History2Weight = Shader.PropertyToID(nameof (_History2Weight));
      internal static readonly int _History3Weight = Shader.PropertyToID(nameof (_History3Weight));
      internal static readonly int _History4Weight = Shader.PropertyToID(nameof (_History4Weight));
    }

    private enum Pass
    {
      VelocitySetup,
      TileMax1,
      TileMax2,
      TileMaxV,
      NeighborMax,
      Reconstruction,
      FrameCompression,
      FrameBlendingChroma,
      FrameBlendingRaw,
    }

    public class ReconstructionFilter
    {
      private RenderTextureFormat m_VectorRTFormat = RenderTextureFormat.RGHalf;
      private RenderTextureFormat m_PackedRTFormat = RenderTextureFormat.ARGB2101010;

      public ReconstructionFilter() => CheckTextureFormatSupport();

      private void CheckTextureFormatSupport()
      {
        if (SystemInfo.SupportsRenderTextureFormat(m_PackedRTFormat))
          return;
        m_PackedRTFormat = RenderTextureFormat.ARGB32;
      }

      public bool IsSupported() => SystemInfo.supportsMotionVectors;

      public void ProcessImage(
        PostProcessingContext context,
        CommandBuffer cb,
        ref MotionBlurModel.Settings settings,
        RenderTargetIdentifier source,
        RenderTargetIdentifier destination,
        Material material)
      {
        int num1 = (int) (5.0 * context.height / 100.0);
        int num2 = ((num1 - 1) / 8 + 1) * 8;
        float num3 = (float) (settings.shutterAngle / 360.0 / (Time.deltaTime * 30.0));
        cb.SetGlobalFloat(Uniforms._VelocityScale, num3);
        cb.SetGlobalFloat(Uniforms._MaxBlurRadius, num1);
        cb.SetGlobalFloat(Uniforms._RcpMaxBlurRadius, 1f / num1);
        int velocityTex = Uniforms._VelocityTex;
        cb.GetTemporaryRT(velocityTex, context.width, context.height, 0, FilterMode.Point, m_PackedRTFormat, RenderTextureReadWrite.Linear);
        cb.Blit(null, velocityTex, material, 0);
        int tile2Rt = Uniforms._Tile2RT;
        cb.GetTemporaryRT(tile2Rt, context.width / 2, context.height / 2, 0, FilterMode.Point, m_VectorRTFormat, RenderTextureReadWrite.Linear);
        cb.SetGlobalTexture(Uniforms._MainTex, velocityTex);
        cb.Blit(velocityTex, tile2Rt, material, 1);
        int tile4Rt = Uniforms._Tile4RT;
        cb.GetTemporaryRT(tile4Rt, context.width / 4, context.height / 4, 0, FilterMode.Point, m_VectorRTFormat, RenderTextureReadWrite.Linear);
        cb.SetGlobalTexture(Uniforms._MainTex, tile2Rt);
        cb.Blit(tile2Rt, tile4Rt, material, 2);
        cb.ReleaseTemporaryRT(tile2Rt);
        int tile8Rt = Uniforms._Tile8RT;
        cb.GetTemporaryRT(tile8Rt, context.width / 8, context.height / 8, 0, FilterMode.Point, m_VectorRTFormat, RenderTextureReadWrite.Linear);
        cb.SetGlobalTexture(Uniforms._MainTex, tile4Rt);
        cb.Blit(tile4Rt, tile8Rt, material, 2);
        cb.ReleaseTemporaryRT(tile4Rt);
        Vector2 vector2 = Vector2.one * (float) (num2 / 8.0 - 1.0) * -0.5f;
        cb.SetGlobalVector(Uniforms._TileMaxOffs, vector2);
        cb.SetGlobalFloat(Uniforms._TileMaxLoop, (int) (num2 / 8.0));
        int tileVrt = Uniforms._TileVRT;
        cb.GetTemporaryRT(tileVrt, context.width / num2, context.height / num2, 0, FilterMode.Point, m_VectorRTFormat, RenderTextureReadWrite.Linear);
        cb.SetGlobalTexture(Uniforms._MainTex, tile8Rt);
        cb.Blit(tile8Rt, tileVrt, material, 3);
        cb.ReleaseTemporaryRT(tile8Rt);
        int neighborMaxTex = Uniforms._NeighborMaxTex;
        int width = context.width / num2;
        int height = context.height / num2;
        cb.GetTemporaryRT(neighborMaxTex, width, height, 0, FilterMode.Point, m_VectorRTFormat, RenderTextureReadWrite.Linear);
        cb.SetGlobalTexture(Uniforms._MainTex, tileVrt);
        cb.Blit(tileVrt, neighborMaxTex, material, 4);
        cb.ReleaseTemporaryRT(tileVrt);
        cb.SetGlobalFloat(Uniforms._LoopCount, Mathf.Clamp(settings.sampleCount / 2, 1, 64));
        cb.SetGlobalTexture(Uniforms._MainTex, source);
        cb.Blit(source, destination, material, 5);
        cb.ReleaseTemporaryRT(velocityTex);
        cb.ReleaseTemporaryRT(neighborMaxTex);
      }
    }

    public class FrameBlendingFilter
    {
      private bool m_UseCompression = CheckSupportCompression();
      private RenderTextureFormat m_RawTextureFormat = GetPreferredRenderTextureFormat();
      private Frame[] m_FrameList = new Frame[4];
      private int m_LastFrameCount;

      public void Dispose()
      {
        foreach (Frame frame in m_FrameList)
          frame.Release();
      }

      public void PushFrame(
        CommandBuffer cb,
        RenderTargetIdentifier source,
        int width,
        int height,
        Material material)
      {
        int frameCount = Time.frameCount;
        if (frameCount == m_LastFrameCount)
          return;
        int index = frameCount % m_FrameList.Length;
        if (m_UseCompression)
          m_FrameList[index].MakeRecord(cb, source, width, height, material);
        else
          m_FrameList[index].MakeRecordRaw(cb, source, width, height, m_RawTextureFormat);
        m_LastFrameCount = frameCount;
      }

      public void BlendFrames(
        CommandBuffer cb,
        float strength,
        RenderTargetIdentifier source,
        RenderTargetIdentifier destination,
        Material material)
      {
        float time = Time.time;
        Frame frameRelative1 = GetFrameRelative(-1);
        Frame frameRelative2 = GetFrameRelative(-2);
        Frame frameRelative3 = GetFrameRelative(-3);
        Frame frameRelative4 = GetFrameRelative(-4);
        cb.SetGlobalTexture(Uniforms._History1LumaTex, (RenderTargetIdentifier) (Texture) frameRelative1.lumaTexture);
        cb.SetGlobalTexture(Uniforms._History2LumaTex, (RenderTargetIdentifier) (Texture) frameRelative2.lumaTexture);
        cb.SetGlobalTexture(Uniforms._History3LumaTex, (RenderTargetIdentifier) (Texture) frameRelative3.lumaTexture);
        cb.SetGlobalTexture(Uniforms._History4LumaTex, (RenderTargetIdentifier) (Texture) frameRelative4.lumaTexture);
        cb.SetGlobalTexture(Uniforms._History1ChromaTex, (RenderTargetIdentifier) (Texture) frameRelative1.chromaTexture);
        cb.SetGlobalTexture(Uniforms._History2ChromaTex, (RenderTargetIdentifier) (Texture) frameRelative2.chromaTexture);
        cb.SetGlobalTexture(Uniforms._History3ChromaTex, (RenderTargetIdentifier) (Texture) frameRelative3.chromaTexture);
        cb.SetGlobalTexture(Uniforms._History4ChromaTex, (RenderTargetIdentifier) (Texture) frameRelative4.chromaTexture);
        cb.SetGlobalFloat(Uniforms._History1Weight, frameRelative1.CalculateWeight(strength, time));
        cb.SetGlobalFloat(Uniforms._History2Weight, frameRelative2.CalculateWeight(strength, time));
        cb.SetGlobalFloat(Uniforms._History3Weight, frameRelative3.CalculateWeight(strength, time));
        cb.SetGlobalFloat(Uniforms._History4Weight, frameRelative4.CalculateWeight(strength, time));
        cb.SetGlobalTexture(Uniforms._MainTex, source);
        cb.Blit(source, destination, material, m_UseCompression ? 7 : 8);
      }

      private static bool CheckSupportCompression()
      {
        return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8) && SystemInfo.supportedRenderTargetCount > 1;
      }

      private static RenderTextureFormat GetPreferredRenderTextureFormat()
      {
        RenderTextureFormat[] renderTextureFormatArray = [
          RenderTextureFormat.RGB565,
          RenderTextureFormat.ARGB1555,
          RenderTextureFormat.ARGB4444
        ];
        foreach (RenderTextureFormat format in renderTextureFormatArray)
        {
          if (SystemInfo.SupportsRenderTextureFormat(format))
            return format;
        }
        return RenderTextureFormat.Default;
      }

      private Frame GetFrameRelative(int offset)
      {
        return m_FrameList[(Time.frameCount + m_FrameList.Length + offset) % m_FrameList.Length];
      }

      private struct Frame
      {
        public RenderTexture lumaTexture;
        public RenderTexture chromaTexture;
        private float m_Time;
        private RenderTargetIdentifier[] m_MRT;

        public float CalculateWeight(float strength, float currentTime)
        {
          if (Mathf.Approximately(m_Time, 0.0f))
            return 0.0f;
          float num = Mathf.Lerp(80f, 16f, strength);
          return Mathf.Exp((m_Time - currentTime) * num);
        }

        public void Release()
        {
          if (lumaTexture != null)
            RenderTexture.ReleaseTemporary(lumaTexture);
          if (chromaTexture != null)
            RenderTexture.ReleaseTemporary(chromaTexture);
          lumaTexture = null;
          chromaTexture = null;
        }

        public void MakeRecord(
          CommandBuffer cb,
          RenderTargetIdentifier source,
          int width,
          int height,
          Material material)
        {
          Release();
          lumaTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
          chromaTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
          lumaTexture.filterMode = FilterMode.Point;
          chromaTexture.filterMode = FilterMode.Point;
          if (m_MRT == null)
            m_MRT = new RenderTargetIdentifier[2];
          m_MRT[0] = (RenderTargetIdentifier) (Texture) lumaTexture;
          m_MRT[1] = (RenderTargetIdentifier) (Texture) chromaTexture;
          cb.SetGlobalTexture(Uniforms._MainTex, source);
          cb.SetRenderTarget(m_MRT, (RenderTargetIdentifier) (Texture) lumaTexture);
          cb.DrawMesh(GraphicsUtils.quad, Matrix4x4.identity, material, 0, 6);
          m_Time = Time.time;
        }

        public void MakeRecordRaw(
          CommandBuffer cb,
          RenderTargetIdentifier source,
          int width,
          int height,
          RenderTextureFormat format)
        {
          Release();
          lumaTexture = RenderTexture.GetTemporary(width, height, 0, format);
          lumaTexture.filterMode = FilterMode.Point;
          cb.SetGlobalTexture(Uniforms._MainTex, source);
          cb.Blit(source, (RenderTargetIdentifier) (Texture) lumaTexture);
          m_Time = Time.time;
        }
      }
    }
  }
}
