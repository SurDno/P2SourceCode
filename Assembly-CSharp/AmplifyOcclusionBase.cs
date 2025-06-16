using System;
using System.Collections.Generic;

[AddComponentMenu("")]
public class AmplifyOcclusionBase : MonoBehaviour
{
  [Header("Ambient Occlusion")]
  public ApplicationMethod ApplyMethod = ApplicationMethod.PostEffect;
  public SampleCountLevel SampleCount = SampleCountLevel.Medium;
  public PerPixelNormalSource PerPixelNormals = PerPixelNormalSource.None;
  [Range(0.0f, 1f)]
  public float Intensity = 1f;
  public Color Tint = Color.black;
  [Range(0.0f, 16f)]
  public float Radius = 1f;
  [Range(0.0f, 16f)]
  public float PowerExponent = 1.8f;
  [Range(0.0f, 0.99f)]
  public float Bias = 0.05f;
  public bool CacheAware = false;
  public bool Downsample = false;
  [Header("Distance Fade")]
  public bool FadeEnabled = false;
  public float FadeStart = 100f;
  public float FadeLength = 50f;
  [Range(0.0f, 1f)]
  public float FadeToIntensity = 1f;
  [Range(0.0f, 16f)]
  public float FadeToRadius = 1f;
  [Range(0.0f, 16f)]
  public float FadeToPowerExponent = 1.8f;
  [Header("Bilateral Blur")]
  public bool BlurEnabled = true;
  [Range(1f, 4f)]
  public int BlurRadius = 2;
  [Range(1f, 4f)]
  public int BlurPasses = 1;
  [Range(0.0f, 20f)]
  public float BlurSharpness = 10f;
  private const int PerPixelNormalSourceCount = 4;
  private int prevScreenWidth;
  private int prevScreenHeight;
  private bool prevHDR;
  private ApplicationMethod prevApplyMethod;
  private SampleCountLevel prevSampleCount;
  private PerPixelNormalSource prevPerPixelNormals;
  private bool prevCacheAware;
  private bool prevDownscale;
  private bool prevFadeEnabled;
  private float prevFadeToIntensity;
  private float prevFadeToRadius;
  private float prevFadeToPowerExponent;
  private float prevFadeStart;
  private float prevFadeLength;
  private bool prevBlurEnabled;
  private int prevBlurRadius;
  private int prevBlurPasses;
  private Camera m_camera;
  private Material m_occlusionMat;
  private Material m_blurMat;
  private Material m_copyMat;
  private const int RandomSize = 4;
  private const int DirectionCount = 8;
  private Color[] m_randomData;
  private string[] m_layerOffsetNames;
  private string[] m_layerRandomNames;
  private string[] m_layerDepthNames;
  private string[] m_layerNormalNames;
  private string[] m_layerOcclusionNames;
  private RenderTextureFormat m_depthRTFormat = RenderTextureFormat.RFloat;
  private RenderTextureFormat m_normalRTFormat = RenderTextureFormat.ARGB2101010;
  private RenderTextureFormat m_occlusionRTFormat = RenderTextureFormat.RGHalf;
  private RenderTexture m_occlusionRT = (RenderTexture) null;
  private int[] m_depthLayerRT;
  private int[] m_normalLayerRT;
  private int[] m_occlusionLayerRT;
  private int m_mrtCount;
  private RenderTargetIdentifier[] m_depthTargets = (RenderTargetIdentifier[]) null;
  private RenderTargetIdentifier[] m_normalTargets = (RenderTargetIdentifier[]) null;
  private int m_deinterleaveDepthPass;
  private int m_deinterleaveNormalPass;
  private RenderTargetIdentifier[] m_applyDeferredTargets = (RenderTargetIdentifier[]) null;
  private Mesh m_blitMesh = (Mesh) null;
  private TargetDesc m_target;
  private Dictionary<CameraEvent, CommandBuffer> m_registeredCommandBuffers = new Dictionary<CameraEvent, CommandBuffer>();

  private bool CheckParamsChanged()
  {
    return prevScreenWidth != m_camera.pixelWidth || prevScreenHeight != m_camera.pixelHeight || prevHDR != m_camera.allowHDR || prevApplyMethod != ApplyMethod || prevSampleCount != SampleCount || prevPerPixelNormals != PerPixelNormals || prevCacheAware != CacheAware || prevDownscale != Downsample || prevFadeEnabled != FadeEnabled || prevFadeToIntensity != (double) FadeToIntensity || prevFadeToRadius != (double) FadeToRadius || prevFadeToPowerExponent != (double) FadeToPowerExponent || prevFadeStart != (double) FadeStart || prevFadeLength != (double) FadeLength || prevBlurEnabled != BlurEnabled || prevBlurRadius != BlurRadius || prevBlurPasses != BlurPasses;
  }

  private void UpdateParams()
  {
    prevScreenWidth = m_camera.pixelWidth;
    prevScreenHeight = m_camera.pixelHeight;
    prevHDR = m_camera.allowHDR;
    prevApplyMethod = ApplyMethod;
    prevSampleCount = SampleCount;
    prevPerPixelNormals = PerPixelNormals;
    prevCacheAware = CacheAware;
    prevDownscale = Downsample;
    prevFadeEnabled = FadeEnabled;
    prevFadeToIntensity = FadeToIntensity;
    prevFadeToRadius = FadeToRadius;
    prevFadeToPowerExponent = FadeToPowerExponent;
    prevFadeStart = FadeStart;
    prevFadeLength = FadeLength;
    prevBlurEnabled = BlurEnabled;
    prevBlurRadius = BlurRadius;
    prevBlurPasses = BlurPasses;
  }

  private void Warmup()
  {
    CheckMaterial();
    CheckRandomData();
    m_depthLayerRT = new int[16];
    m_normalLayerRT = new int[16];
    m_occlusionLayerRT = new int[16];
    m_mrtCount = Mathf.Min(SystemInfo.supportedRenderTargetCount, 4);
    m_layerOffsetNames = new string[m_mrtCount];
    m_layerRandomNames = new string[m_mrtCount];
    for (int index = 0; index < m_mrtCount; ++index)
    {
      m_layerOffsetNames[index] = "_AO_LayerOffset" + index;
      m_layerRandomNames[index] = "_AO_LayerRandom" + index;
    }
    m_layerDepthNames = new string[16];
    m_layerNormalNames = new string[16];
    m_layerOcclusionNames = new string[16];
    for (int index = 0; index < 16; ++index)
    {
      m_layerDepthNames[index] = "_AO_DepthLayer" + index;
      m_layerNormalNames[index] = "_AO_NormalLayer" + index;
      m_layerOcclusionNames[index] = "_AO_OcclusionLayer" + index;
    }
    m_depthTargets = new RenderTargetIdentifier[m_mrtCount];
    m_normalTargets = new RenderTargetIdentifier[m_mrtCount];
    if (m_mrtCount == 4)
    {
      m_deinterleaveDepthPass = 10;
      m_deinterleaveNormalPass = 11;
    }
    else
    {
      m_deinterleaveDepthPass = 5;
      m_deinterleaveNormalPass = 6;
    }
    m_applyDeferredTargets = new RenderTargetIdentifier[2];
    if ((UnityEngine.Object) m_blitMesh != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) m_blitMesh);
    m_blitMesh = new Mesh();
    m_blitMesh.vertices = new Vector3[4]
    {
      new Vector3(0.0f, 0.0f, 0.0f),
      new Vector3(0.0f, 1f, 0.0f),
      new Vector3(1f, 1f, 0.0f),
      new Vector3(1f, 0.0f, 0.0f)
    };
    m_blitMesh.uv = new Vector2[4]
    {
      new Vector2(0.0f, 0.0f),
      new Vector2(0.0f, 1f),
      new Vector2(1f, 1f),
      new Vector2(1f, 0.0f)
    };
    m_blitMesh.triangles = new int[6]
    {
      0,
      1,
      2,
      0,
      2,
      3
    };
  }

  private void Shutdown()
  {
    CommandBuffer_UnregisterAll();
    SafeReleaseRT(ref m_occlusionRT);
    if ((UnityEngine.Object) m_occlusionMat != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) m_occlusionMat);
    if ((UnityEngine.Object) m_blurMat != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) m_blurMat);
    if ((UnityEngine.Object) m_copyMat != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) m_copyMat);
    if (!((UnityEngine.Object) m_blitMesh != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) m_blitMesh);
  }

  private bool CheckRenderTextureFormats()
  {
    if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32) || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
      return false;
    m_depthRTFormat = RenderTextureFormat.RFloat;
    if (!SystemInfo.SupportsRenderTextureFormat(m_depthRTFormat))
    {
      m_depthRTFormat = RenderTextureFormat.RHalf;
      if (!SystemInfo.SupportsRenderTextureFormat(m_depthRTFormat))
        m_depthRTFormat = RenderTextureFormat.ARGBHalf;
    }
    m_normalRTFormat = RenderTextureFormat.ARGB2101010;
    if (!SystemInfo.SupportsRenderTextureFormat(m_normalRTFormat))
      m_normalRTFormat = RenderTextureFormat.ARGB32;
    m_occlusionRTFormat = RenderTextureFormat.RGHalf;
    if (!SystemInfo.SupportsRenderTextureFormat(m_occlusionRTFormat))
    {
      m_occlusionRTFormat = RenderTextureFormat.RGFloat;
      if (!SystemInfo.SupportsRenderTextureFormat(m_occlusionRTFormat))
        m_occlusionRTFormat = RenderTextureFormat.ARGBHalf;
    }
    return true;
  }

  private void OnEnable()
  {
    if (!CheckRenderTextureFormats())
    {
      Debug.LogError((object) "[AmplifyOcclusion] Target platform does not meet the minimum requirements for this effect to work properly.");
      this.enabled = false;
    }
    else
    {
      m_camera = this.GetComponent<Camera>();
      Warmup();
      CommandBuffer_UnregisterAll();
    }
  }

  private void OnDisable() => Shutdown();

  private void OnDestroy() => Shutdown();

  private void Update()
  {
    if (m_camera.actualRenderingPath != RenderingPath.DeferredShading)
    {
      if (PerPixelNormals != PerPixelNormalSource.None && PerPixelNormals != PerPixelNormalSource.Camera)
      {
        PerPixelNormals = PerPixelNormalSource.Camera;
        Debug.LogWarning((object) "[AmplifyOcclusion] GBuffer Normals only available in Camera Deferred Shading mode. Switched to Camera source.");
      }
      if (ApplyMethod == ApplicationMethod.Deferred)
      {
        ApplyMethod = ApplicationMethod.PostEffect;
        Debug.LogWarning((object) "[AmplifyOcclusion] Deferred Method requires a Deferred Shading path. Switching to Post Effect Method.");
      }
    }
    if (ApplyMethod == ApplicationMethod.Deferred && PerPixelNormals == PerPixelNormalSource.Camera)
    {
      PerPixelNormals = PerPixelNormalSource.GBuffer;
      Debug.LogWarning((object) "[AmplifyOcclusion] Camera Normals not supported for Deferred Method. Switching to GBuffer Normals.");
    }
    if ((m_camera.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None)
      m_camera.depthTextureMode |= DepthTextureMode.Depth;
    if (PerPixelNormals == PerPixelNormalSource.Camera && (m_camera.depthTextureMode & DepthTextureMode.DepthNormals) == DepthTextureMode.None)
      m_camera.depthTextureMode |= DepthTextureMode.DepthNormals;
    CheckMaterial();
    CheckRandomData();
  }

  private void CheckMaterial()
  {
    if ((UnityEngine.Object) m_occlusionMat == (UnityEngine.Object) null)
    {
      Material material = new Material(Shader.Find("Hidden/Amplify Occlusion/Occlusion"));
      material.hideFlags = HideFlags.DontSave;
      m_occlusionMat = material;
    }
    if ((UnityEngine.Object) m_blurMat == (UnityEngine.Object) null)
    {
      Material material = new Material(Shader.Find("Hidden/Amplify Occlusion/Blur"));
      material.hideFlags = HideFlags.DontSave;
      m_blurMat = material;
    }
    if (!((UnityEngine.Object) m_copyMat == (UnityEngine.Object) null))
      return;
    Material material1 = new Material(Shader.Find("Hidden/Amplify Occlusion/Copy"));
    material1.hideFlags = HideFlags.DontSave;
    m_copyMat = material1;
  }

  private void CheckRandomData()
  {
    if (m_randomData != null)
      return;
    m_randomData = GenerateRandomizationData();
  }

  public static Color[] GenerateRandomizationData()
  {
    Color[] randomizationData = new Color[16];
    int index1 = 0;
    int num1 = 0;
    for (; index1 < 16; ++index1)
    {
      float[] values1 = RandomTable.Values;
      int index2 = num1;
      int num2 = index2 + 1;
      float num3 = values1[index2];
      float[] values2 = RandomTable.Values;
      int index3 = num2;
      num1 = index3 + 1;
      float num4 = values2[index3];
      float f = (float) (6.2831854820251465 * num3 / 8.0);
      randomizationData[index1].r = Mathf.Cos(f);
      randomizationData[index1].g = Mathf.Sin(f);
      randomizationData[index1].b = num4;
      randomizationData[index1].a = 0.0f;
    }
    return randomizationData;
  }

  public static Texture2D GenerateRandomizationTexture(Color[] randomPixels)
  {
    Texture2D texture2D = new Texture2D(4, 4, TextureFormat.ARGB32, false, true);
    texture2D.hideFlags = HideFlags.DontSave;
    Texture2D randomizationTexture = texture2D;
    randomizationTexture.name = "RandomTexture";
    randomizationTexture.filterMode = FilterMode.Point;
    randomizationTexture.wrapMode = TextureWrapMode.Repeat;
    randomizationTexture.SetPixels(randomPixels);
    randomizationTexture.Apply();
    return randomizationTexture;
  }

  private RenderTexture SafeAllocateRT(
    string name,
    int width,
    int height,
    RenderTextureFormat format,
    RenderTextureReadWrite readWrite)
  {
    width = Mathf.Max(width, 1);
    height = Mathf.Max(height, 1);
    RenderTexture renderTexture1 = new RenderTexture(width, height, 0, format, readWrite);
    renderTexture1.hideFlags = HideFlags.DontSave;
    RenderTexture renderTexture2 = renderTexture1;
    renderTexture2.name = name;
    renderTexture2.filterMode = FilterMode.Point;
    renderTexture2.wrapMode = TextureWrapMode.Clamp;
    renderTexture2.Create();
    return renderTexture2;
  }

  private void SafeReleaseRT(ref RenderTexture rt)
  {
    if (!((UnityEngine.Object) rt != (UnityEngine.Object) null))
      return;
    RenderTexture.active = (RenderTexture) null;
    rt.Release();
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) rt);
    rt = (RenderTexture) null;
  }

  private int SafeAllocateTemporaryRT(
    CommandBuffer cb,
    string propertyName,
    int width,
    int height,
    RenderTextureFormat format = RenderTextureFormat.Default,
    RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default,
    FilterMode filterMode = FilterMode.Point)
  {
    int id = Shader.PropertyToID(propertyName);
    cb.GetTemporaryRT(id, width, height, 0, filterMode, format, readWrite);
    return id;
  }

  private void SafeReleaseTemporaryRT(CommandBuffer cb, int id) => cb.ReleaseTemporaryRT(id);

  private void SetBlitTarget(
    CommandBuffer cb,
    RenderTargetIdentifier[] targets,
    int targetWidth,
    int targetHeight)
  {
    cb.SetGlobalVector("_AO_Target_TexelSize", new Vector4(1f / targetWidth, 1f / targetHeight, (float) targetWidth, (float) targetHeight));
    cb.SetGlobalVector("_AO_Target_Position", (Vector4) Vector2.zero);
    cb.SetRenderTarget(targets, targets[0]);
  }

  private void SetBlitTarget(
    CommandBuffer cb,
    RenderTargetIdentifier target,
    int targetWidth,
    int targetHeight)
  {
    cb.SetGlobalVector("_AO_Target_TexelSize", new Vector4(1f / targetWidth, 1f / targetHeight, (float) targetWidth, (float) targetHeight));
    cb.SetRenderTarget(target);
  }

  private void PerformBlit(CommandBuffer cb, Material mat, int pass)
  {
    cb.DrawMesh(m_blitMesh, Matrix4x4.identity, mat, 0, pass);
  }

  private void PerformBlit(CommandBuffer cb, Material mat, int pass, int x, int y)
  {
    cb.SetGlobalVector("_AO_Target_Position", (Vector4) new Vector2((float) x, (float) y));
    PerformBlit(cb, mat, pass);
  }

  private void PerformBlit(
    CommandBuffer cb,
    RenderTargetIdentifier source,
    int sourceWidth,
    int sourceHeight,
    Material mat,
    int pass)
  {
    cb.SetGlobalTexture("_AO_Source", source);
    cb.SetGlobalVector("_AO_Source_TexelSize", new Vector4(1f / sourceWidth, 1f / sourceHeight, (float) sourceWidth, (float) sourceHeight));
    PerformBlit(cb, mat, pass);
  }

  private void PerformBlit(
    CommandBuffer cb,
    RenderTargetIdentifier source,
    int sourceWidth,
    int sourceHeight,
    Material mat,
    int pass,
    int x,
    int y)
  {
    cb.SetGlobalVector("_AO_Target_Position", (Vector4) new Vector2((float) x, (float) y));
    PerformBlit(cb, source, sourceWidth, sourceHeight, mat, pass);
  }

  private CommandBuffer CommandBuffer_Allocate(string name)
  {
    return new CommandBuffer { name = name };
  }

  private void CommandBuffer_Register(CameraEvent cameraEvent, CommandBuffer commandBuffer)
  {
    m_camera.AddCommandBuffer(cameraEvent, commandBuffer);
    m_registeredCommandBuffers.Add(cameraEvent, commandBuffer);
  }

  private void CommandBuffer_Unregister(CameraEvent cameraEvent, CommandBuffer commandBuffer)
  {
    if (!((UnityEngine.Object) m_camera != (UnityEngine.Object) null))
      return;
    foreach (CommandBuffer commandBuffer1 in m_camera.GetCommandBuffers(cameraEvent))
    {
      if (commandBuffer1.name == commandBuffer.name)
        m_camera.RemoveCommandBuffer(cameraEvent, commandBuffer1);
    }
  }

  private CommandBuffer CommandBuffer_AllocateRegister(CameraEvent cameraEvent)
  {
    string name = "";
    switch (cameraEvent)
    {
      case CameraEvent.AfterLighting:
        name = "AO-AfterLighting";
        break;
      case CameraEvent.BeforeImageEffectsOpaque:
        name = "AO-BeforePostOpaque";
        break;
      case CameraEvent.BeforeReflections:
        name = "AO-BeforeRefl";
        break;
      default:
        Debug.LogError((object) "[AmplifyOcclusion] Unsupported CameraEvent. Please contact support.");
        break;
    }
    CommandBuffer commandBuffer = CommandBuffer_Allocate(name);
    CommandBuffer_Register(cameraEvent, commandBuffer);
    return commandBuffer;
  }

  private void CommandBuffer_UnregisterAll()
  {
    foreach (KeyValuePair<CameraEvent, CommandBuffer> registeredCommandBuffer in m_registeredCommandBuffers)
      CommandBuffer_Unregister(registeredCommandBuffer.Key, registeredCommandBuffer.Value);
    m_registeredCommandBuffers.Clear();
  }

  private void UpdateGlobalShaderConstants(TargetDesc target)
  {
    float num1 = m_camera.fieldOfView * ((float) Math.PI / 180f);
    Vector2 vector2_1 = new Vector2((float) (1.0 / (double) Mathf.Tan(num1 * 0.5f) * (target.height / (double) target.width)), 1f / Mathf.Tan(num1 * 0.5f));
    Vector2 vector2_2 = new Vector2(1f / vector2_1.x, 1f / vector2_1.y);
    float num2 = !m_camera.orthographic ? target.height / (Mathf.Tan(num1 * 0.5f) * 2f) : (float) target.height / m_camera.orthographicSize;
    float num3 = Mathf.Clamp(Bias, 0.0f, 1f);
    FadeStart = Mathf.Max(0.0f, FadeStart);
    FadeLength = Mathf.Max(0.01f, FadeLength);
    float y = FadeEnabled ? 1f / FadeLength : 0.0f;
    Shader.SetGlobalMatrix("_AO_CameraProj", GL.GetGPUProjectionMatrix(Matrix4x4.Ortho(0.0f, 1f, 0.0f, 1f, -1f, 100f), false));
    Shader.SetGlobalMatrix("_AO_CameraView", m_camera.worldToCameraMatrix);
    Shader.SetGlobalVector("_AO_UVToView", new Vector4(2f * vector2_2.x, -2f * vector2_2.y, -1f * vector2_2.x, 1f * vector2_2.y));
    Shader.SetGlobalFloat("_AO_HalfProjScale", 0.5f * num2);
    Shader.SetGlobalFloat("_AO_Radius", Radius);
    Shader.SetGlobalFloat("_AO_PowExponent", PowerExponent);
    Shader.SetGlobalFloat("_AO_Bias", num3);
    Shader.SetGlobalFloat("_AO_Multiplier", (float) (1.0 / (1.0 - num3)));
    Shader.SetGlobalFloat("_AO_BlurSharpness", BlurSharpness);
    Shader.SetGlobalColor("_AO_Levels", new Color(Tint.r, Tint.g, Tint.b, Intensity));
    Shader.SetGlobalVector("_AO_FadeParams", (Vector4) new Vector2(FadeStart, y));
    Shader.SetGlobalVector("_AO_FadeValues", (Vector4) new Vector3(FadeToIntensity, FadeToRadius, FadeToPowerExponent));
  }

  private void CommandBuffer_FillComputeOcclusion(
    CommandBuffer cb,
    TargetDesc target)
  {
    CheckMaterial();
    CheckRandomData();
    cb.SetGlobalVector("_AO_Buffer_PadScale", new Vector4(target.padRatioWidth, target.padRatioHeight, 1f / target.padRatioWidth, 1f / target.padRatioHeight));
    cb.SetGlobalVector("_AO_Buffer_TexelSize", new Vector4(1f / target.width, 1f / target.height, (float) target.width, (float) target.height));
    cb.SetGlobalVector("_AO_QuarterBuffer_TexelSize", new Vector4(1f / target.quarterWidth, 1f / target.quarterHeight, (float) target.quarterWidth, (float) target.quarterHeight));
    cb.SetGlobalFloat("_AO_MaxRadiusPixels", (float) Mathf.Min(target.width, target.height));
    if ((UnityEngine.Object) m_occlusionRT == (UnityEngine.Object) null || m_occlusionRT.width != target.width || m_occlusionRT.height != target.height || !m_occlusionRT.IsCreated())
    {
      SafeReleaseRT(ref m_occlusionRT);
      m_occlusionRT = SafeAllocateRT("_AO_OcclusionTexture", target.width, target.height, m_occlusionRTFormat, RenderTextureReadWrite.Linear);
    }
    int num1 = -1;
    if (Downsample)
      num1 = SafeAllocateTemporaryRT(cb, "_AO_SmallOcclusionTexture", target.width / 2, target.height / 2, m_occlusionRTFormat, RenderTextureReadWrite.Linear, FilterMode.Bilinear);
    if (CacheAware && !Downsample)
    {
      int num2 = SafeAllocateTemporaryRT(cb, "_AO_OcclusionAtlas", target.width, target.height, m_occlusionRTFormat, RenderTextureReadWrite.Linear);
      for (int index = 0; index < 16; ++index)
      {
        m_depthLayerRT[index] = SafeAllocateTemporaryRT(cb, m_layerDepthNames[index], target.quarterWidth, target.quarterHeight, m_depthRTFormat, RenderTextureReadWrite.Linear);
        m_normalLayerRT[index] = SafeAllocateTemporaryRT(cb, m_layerNormalNames[index], target.quarterWidth, target.quarterHeight, m_normalRTFormat, RenderTextureReadWrite.Linear);
        m_occlusionLayerRT[index] = SafeAllocateTemporaryRT(cb, m_layerOcclusionNames[index], target.quarterWidth, target.quarterHeight, m_occlusionRTFormat, RenderTextureReadWrite.Linear);
      }
      for (int index1 = 0; index1 < 16; index1 += m_mrtCount)
      {
        for (int index2 = 0; index2 < m_mrtCount; ++index2)
        {
          int index3 = index2 + index1;
          int num3 = index3 & 3;
          int num4 = index3 >> 2;
          cb.SetGlobalVector(m_layerOffsetNames[index2], (Vector4) new Vector2(num3 + 0.5f, num4 + 0.5f));
          m_depthTargets[index2] = (RenderTargetIdentifier) m_depthLayerRT[index3];
          m_normalTargets[index2] = (RenderTargetIdentifier) m_normalLayerRT[index3];
        }
        SetBlitTarget(cb, m_depthTargets, target.quarterWidth, target.quarterHeight);
        PerformBlit(cb, m_occlusionMat, m_deinterleaveDepthPass);
        SetBlitTarget(cb, m_normalTargets, target.quarterWidth, target.quarterHeight);
        PerformBlit(cb, m_occlusionMat, (int) (m_deinterleaveNormalPass + PerPixelNormals));
      }
      for (int index = 0; index < 16; ++index)
      {
        cb.SetGlobalVector("_AO_LayerOffset", (Vector4) new Vector2((index & 3) + 0.5f, (index >> 2) + 0.5f));
        cb.SetGlobalVector("_AO_LayerRandom", (Vector4) m_randomData[index]);
        cb.SetGlobalTexture("_AO_NormalTexture", (RenderTargetIdentifier) m_normalLayerRT[index]);
        cb.SetGlobalTexture("_AO_DepthTexture", (RenderTargetIdentifier) m_depthLayerRT[index]);
        SetBlitTarget(cb, (RenderTargetIdentifier) m_occlusionLayerRT[index], target.quarterWidth, target.quarterHeight);
        PerformBlit(cb, m_occlusionMat, (int) (15 + SampleCount));
      }
      SetBlitTarget(cb, (RenderTargetIdentifier) num2, target.width, target.height);
      for (int index = 0; index < 16; ++index)
      {
        int x = (index & 3) * target.quarterWidth;
        int y = (index >> 2) * target.quarterHeight;
        PerformBlit(cb, (RenderTargetIdentifier) m_occlusionLayerRT[index], target.quarterWidth, target.quarterHeight, m_copyMat, 0, x, y);
      }
      cb.SetGlobalTexture("_AO_OcclusionAtlas", (RenderTargetIdentifier) num2);
      SetBlitTarget(cb, (RenderTargetIdentifier) (Texture) m_occlusionRT, target.width, target.height);
      PerformBlit(cb, m_occlusionMat, 19);
      for (int index = 0; index < 16; ++index)
      {
        SafeReleaseTemporaryRT(cb, m_occlusionLayerRT[index]);
        SafeReleaseTemporaryRT(cb, m_normalLayerRT[index]);
        SafeReleaseTemporaryRT(cb, m_depthLayerRT[index]);
      }
      SafeReleaseTemporaryRT(cb, num2);
    }
    else
    {
      int pass = (int) (20 + (int) SampleCount * 4 + PerPixelNormals);
      if (Downsample)
      {
        cb.Blit((Texture) null, new RenderTargetIdentifier(num1), m_occlusionMat, pass);
        SetBlitTarget(cb, (RenderTargetIdentifier) (Texture) m_occlusionRT, target.width, target.height);
        PerformBlit(cb, (RenderTargetIdentifier) num1, target.width / 2, target.height / 2, m_occlusionMat, 41);
      }
      else
        cb.Blit((Texture) null, (RenderTargetIdentifier) (Texture) m_occlusionRT, m_occlusionMat, pass);
    }
    if (BlurEnabled)
    {
      int num5 = SafeAllocateTemporaryRT(cb, "_AO_TEMP", target.width, target.height, m_occlusionRTFormat, RenderTextureReadWrite.Linear);
      for (int index = 0; index < BlurPasses; ++index)
      {
        SetBlitTarget(cb, (RenderTargetIdentifier) num5, target.width, target.height);
        PerformBlit(cb, (RenderTargetIdentifier) (Texture) m_occlusionRT, target.width, target.height, m_blurMat, (BlurRadius - 1) * 2);
        SetBlitTarget(cb, (RenderTargetIdentifier) (Texture) m_occlusionRT, target.width, target.height);
        PerformBlit(cb, (RenderTargetIdentifier) num5, target.width, target.height, m_blurMat, 1 + (BlurRadius - 1) * 2);
      }
      SafeReleaseTemporaryRT(cb, num5);
    }
    if (Downsample && num1 >= 0)
      SafeReleaseTemporaryRT(cb, num1);
    cb.SetRenderTarget((RenderTargetIdentifier) (Texture) null);
  }

  private void CommandBuffer_FillApplyDeferred(
    CommandBuffer cb,
    TargetDesc target,
    bool logTarget)
  {
    cb.SetGlobalTexture("_AO_OcclusionTexture", (RenderTargetIdentifier) (Texture) m_occlusionRT);
    m_applyDeferredTargets[0] = (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer0;
    m_applyDeferredTargets[1] = (RenderTargetIdentifier) (logTarget ? BuiltinRenderTextureType.GBuffer3 : BuiltinRenderTextureType.CameraTarget);
    if (!logTarget)
    {
      SetBlitTarget(cb, m_applyDeferredTargets, target.fullWidth, target.fullHeight);
      PerformBlit(cb, m_occlusionMat, 37);
    }
    else
    {
      int num1 = SafeAllocateTemporaryRT(cb, "_AO_GBufferAlbedo", target.fullWidth, target.fullHeight, RenderTextureFormat.ARGB32);
      int num2 = SafeAllocateTemporaryRT(cb, "_AO_GBufferEmission", target.fullWidth, target.fullHeight, RenderTextureFormat.ARGB32);
      cb.Blit(m_applyDeferredTargets[0], (RenderTargetIdentifier) num1);
      cb.Blit(m_applyDeferredTargets[1], (RenderTargetIdentifier) num2);
      cb.SetGlobalTexture("_AO_GBufferAlbedo", (RenderTargetIdentifier) num1);
      cb.SetGlobalTexture("_AO_GBufferEmission", (RenderTargetIdentifier) num2);
      SetBlitTarget(cb, m_applyDeferredTargets, target.fullWidth, target.fullHeight);
      PerformBlit(cb, m_occlusionMat, 38);
      SafeReleaseTemporaryRT(cb, num1);
      SafeReleaseTemporaryRT(cb, num2);
    }
    cb.SetRenderTarget((RenderTargetIdentifier) (Texture) null);
  }

  private void CommandBuffer_FillApplyPostEffect(
    CommandBuffer cb,
    TargetDesc target,
    bool logTarget)
  {
    cb.SetGlobalTexture("_AO_OcclusionTexture", (RenderTargetIdentifier) (Texture) m_occlusionRT);
    if (!logTarget)
    {
      SetBlitTarget(cb, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, target.fullWidth, target.fullHeight);
      PerformBlit(cb, m_occlusionMat, 39);
    }
    else
    {
      int num = SafeAllocateTemporaryRT(cb, "_AO_GBufferEmission", target.fullWidth, target.fullHeight, RenderTextureFormat.ARGB32);
      cb.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer3, (RenderTargetIdentifier) num);
      cb.SetGlobalTexture("_AO_GBufferEmission", (RenderTargetIdentifier) num);
      SetBlitTarget(cb, (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer3, target.fullWidth, target.fullHeight);
      PerformBlit(cb, m_occlusionMat, 40);
      SafeReleaseTemporaryRT(cb, num);
    }
    cb.SetRenderTarget((RenderTargetIdentifier) (Texture) null);
  }

  private void CommandBuffer_FillApplyDebug(
    CommandBuffer cb,
    TargetDesc target)
  {
    cb.SetGlobalTexture("_AO_OcclusionTexture", (RenderTargetIdentifier) (Texture) m_occlusionRT);
    SetBlitTarget(cb, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, target.fullWidth, target.fullHeight);
    PerformBlit(cb, m_occlusionMat, 36);
    cb.SetRenderTarget((RenderTargetIdentifier) (Texture) null);
  }

  private void CommandBuffer_Rebuild(TargetDesc target)
  {
    bool flag = PerPixelNormals == PerPixelNormalSource.GBuffer || PerPixelNormals == PerPixelNormalSource.GBufferOctaEncoded;
    CameraEvent cameraEvent = flag ? CameraEvent.AfterLighting : CameraEvent.BeforeImageEffectsOpaque;
    if (ApplyMethod == ApplicationMethod.Debug)
    {
      CommandBuffer cb = CommandBuffer_AllocateRegister(cameraEvent);
      CommandBuffer_FillComputeOcclusion(cb, target);
      CommandBuffer_FillApplyDebug(cb, target);
    }
    else
    {
      bool logTarget = !m_camera.allowHDR & flag;
      CommandBuffer cb = CommandBuffer_AllocateRegister(ApplyMethod == ApplicationMethod.Deferred ? CameraEvent.BeforeReflections : cameraEvent);
      CommandBuffer_FillComputeOcclusion(cb, target);
      if (ApplyMethod == ApplicationMethod.PostEffect)
        CommandBuffer_FillApplyPostEffect(cb, target, logTarget);
      else if (ApplyMethod == ApplicationMethod.Deferred)
        CommandBuffer_FillApplyDeferred(cb, target, logTarget);
    }
  }

  private void OnPreRender()
  {
    bool allowHdr = m_camera.allowHDR;
    m_target.fullWidth = m_camera.pixelWidth;
    m_target.fullHeight = m_camera.pixelHeight;
    m_target.format = allowHdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
    m_target.width = CacheAware ? m_target.fullWidth + 3 & -4 : m_target.fullWidth;
    m_target.height = CacheAware ? m_target.fullHeight + 3 & -4 : m_target.fullHeight;
    m_target.quarterWidth = m_target.width / 4;
    m_target.quarterHeight = m_target.height / 4;
    m_target.padRatioWidth = m_target.width / (float) m_target.fullWidth;
    m_target.padRatioHeight = m_target.height / (float) m_target.fullHeight;
    UpdateGlobalShaderConstants(m_target);
    if (!CheckParamsChanged() && m_registeredCommandBuffers.Count != 0)
      return;
    CommandBuffer_UnregisterAll();
    CommandBuffer_Rebuild(m_target);
    UpdateParams();
  }

  private void OnPostRender() => m_occlusionRT.MarkRestoreExpected();

  public enum ApplicationMethod
  {
    PostEffect,
    Deferred,
    Debug,
  }

  public enum PerPixelNormalSource
  {
    None,
    Camera,
    GBuffer,
    GBufferOctaEncoded,
  }

  public enum SampleCountLevel
  {
    Low,
    Medium,
    High,
    VeryHigh,
  }

  private static class ShaderPass
  {
    public const int FullDepth = 0;
    public const int FullNormal_None = 1;
    public const int FullNormal_Camera = 2;
    public const int FullNormal_GBuffer = 3;
    public const int FullNormal_GBufferOctaEncoded = 4;
    public const int DeinterleaveDepth1 = 5;
    public const int DeinterleaveNormal1_None = 6;
    public const int DeinterleaveNormal1_Camera = 7;
    public const int DeinterleaveNormal1_GBuffer = 8;
    public const int DeinterleaveNormal1_GBufferOctaEncoded = 9;
    public const int DeinterleaveDepth4 = 10;
    public const int DeinterleaveNormal4_None = 11;
    public const int DeinterleaveNormal4_Camera = 12;
    public const int DeinterleaveNormal4_GBuffer = 13;
    public const int DeinterleaveNormal4_GBufferOctaEncoded = 14;
    public const int OcclusionCache_Low = 15;
    public const int OcclusionCache_Medium = 16;
    public const int OcclusionCache_High = 17;
    public const int OcclusionCache_VeryHigh = 18;
    public const int Reinterleave = 19;
    public const int OcclusionLow_None = 20;
    public const int OcclusionLow_Camera = 21;
    public const int OcclusionLow_GBuffer = 22;
    public const int OcclusionLow_GBufferOctaEncoded = 23;
    public const int OcclusionMedium_None = 24;
    public const int OcclusionMedium_Camera = 25;
    public const int OcclusionMedium_GBuffer = 26;
    public const int OcclusionMedium_GBufferOctaEncoded = 27;
    public const int OcclusionHigh_None = 28;
    public const int OcclusionHigh_Camera = 29;
    public const int OcclusionHigh_GBuffer = 30;
    public const int OcclusionHigh_GBufferOctaEncoded = 31;
    public const int OcclusionVeryHigh_None = 32;
    public const int OcclusionVeryHigh_Camera = 33;
    public const int OcclusionVeryHigh_GBuffer = 34;
    public const int OcclusionVeryHigh_GBufferNormalEncoded = 35;
    public const int ApplyDebug = 36;
    public const int ApplyDeferred = 37;
    public const int ApplyDeferredLog = 38;
    public const int ApplyPostEffect = 39;
    public const int ApplyPostEffectLog = 40;
    public const int CombineDownsampledOcclusionDepth = 41;
    public const int BlurHorizontal1 = 0;
    public const int BlurVertical1 = 1;
    public const int BlurHorizontal2 = 2;
    public const int BlurVertical2 = 3;
    public const int BlurHorizontal3 = 4;
    public const int BlurVertical3 = 5;
    public const int BlurHorizontal4 = 6;
    public const int BlurVertical4 = 7;
    public const int Copy = 0;
  }

  private struct TargetDesc
  {
    public int fullWidth;
    public int fullHeight;
    public RenderTextureFormat format;
    public int width;
    public int height;
    public int quarterWidth;
    public int quarterHeight;
    public float padRatioWidth;
    public float padRatioHeight;
  }
}
