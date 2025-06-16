// Decompiled with JetBrains decompiler
// Type: AmplifyOcclusionBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
[AddComponentMenu("")]
public class AmplifyOcclusionBase : MonoBehaviour
{
  [Header("Ambient Occlusion")]
  public AmplifyOcclusionBase.ApplicationMethod ApplyMethod = AmplifyOcclusionBase.ApplicationMethod.PostEffect;
  public AmplifyOcclusionBase.SampleCountLevel SampleCount = AmplifyOcclusionBase.SampleCountLevel.Medium;
  public AmplifyOcclusionBase.PerPixelNormalSource PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.None;
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
  private int prevScreenWidth = 0;
  private int prevScreenHeight = 0;
  private bool prevHDR = false;
  private AmplifyOcclusionBase.ApplicationMethod prevApplyMethod;
  private AmplifyOcclusionBase.SampleCountLevel prevSampleCount;
  private AmplifyOcclusionBase.PerPixelNormalSource prevPerPixelNormals;
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
  private string[] m_layerOffsetNames = (string[]) null;
  private string[] m_layerRandomNames = (string[]) null;
  private string[] m_layerDepthNames = (string[]) null;
  private string[] m_layerNormalNames = (string[]) null;
  private string[] m_layerOcclusionNames = (string[]) null;
  private RenderTextureFormat m_depthRTFormat = RenderTextureFormat.RFloat;
  private RenderTextureFormat m_normalRTFormat = RenderTextureFormat.ARGB2101010;
  private RenderTextureFormat m_occlusionRTFormat = RenderTextureFormat.RGHalf;
  private RenderTexture m_occlusionRT = (RenderTexture) null;
  private int[] m_depthLayerRT = (int[]) null;
  private int[] m_normalLayerRT = (int[]) null;
  private int[] m_occlusionLayerRT = (int[]) null;
  private int m_mrtCount = 0;
  private RenderTargetIdentifier[] m_depthTargets = (RenderTargetIdentifier[]) null;
  private RenderTargetIdentifier[] m_normalTargets = (RenderTargetIdentifier[]) null;
  private int m_deinterleaveDepthPass = 0;
  private int m_deinterleaveNormalPass = 0;
  private RenderTargetIdentifier[] m_applyDeferredTargets = (RenderTargetIdentifier[]) null;
  private Mesh m_blitMesh = (Mesh) null;
  private AmplifyOcclusionBase.TargetDesc m_target = new AmplifyOcclusionBase.TargetDesc();
  private Dictionary<CameraEvent, CommandBuffer> m_registeredCommandBuffers = new Dictionary<CameraEvent, CommandBuffer>();

  private bool CheckParamsChanged()
  {
    return this.prevScreenWidth != this.m_camera.pixelWidth || this.prevScreenHeight != this.m_camera.pixelHeight || this.prevHDR != this.m_camera.allowHDR || this.prevApplyMethod != this.ApplyMethod || this.prevSampleCount != this.SampleCount || this.prevPerPixelNormals != this.PerPixelNormals || this.prevCacheAware != this.CacheAware || this.prevDownscale != this.Downsample || this.prevFadeEnabled != this.FadeEnabled || (double) this.prevFadeToIntensity != (double) this.FadeToIntensity || (double) this.prevFadeToRadius != (double) this.FadeToRadius || (double) this.prevFadeToPowerExponent != (double) this.FadeToPowerExponent || (double) this.prevFadeStart != (double) this.FadeStart || (double) this.prevFadeLength != (double) this.FadeLength || this.prevBlurEnabled != this.BlurEnabled || this.prevBlurRadius != this.BlurRadius || this.prevBlurPasses != this.BlurPasses;
  }

  private void UpdateParams()
  {
    this.prevScreenWidth = this.m_camera.pixelWidth;
    this.prevScreenHeight = this.m_camera.pixelHeight;
    this.prevHDR = this.m_camera.allowHDR;
    this.prevApplyMethod = this.ApplyMethod;
    this.prevSampleCount = this.SampleCount;
    this.prevPerPixelNormals = this.PerPixelNormals;
    this.prevCacheAware = this.CacheAware;
    this.prevDownscale = this.Downsample;
    this.prevFadeEnabled = this.FadeEnabled;
    this.prevFadeToIntensity = this.FadeToIntensity;
    this.prevFadeToRadius = this.FadeToRadius;
    this.prevFadeToPowerExponent = this.FadeToPowerExponent;
    this.prevFadeStart = this.FadeStart;
    this.prevFadeLength = this.FadeLength;
    this.prevBlurEnabled = this.BlurEnabled;
    this.prevBlurRadius = this.BlurRadius;
    this.prevBlurPasses = this.BlurPasses;
  }

  private void Warmup()
  {
    this.CheckMaterial();
    this.CheckRandomData();
    this.m_depthLayerRT = new int[16];
    this.m_normalLayerRT = new int[16];
    this.m_occlusionLayerRT = new int[16];
    this.m_mrtCount = Mathf.Min(SystemInfo.supportedRenderTargetCount, 4);
    this.m_layerOffsetNames = new string[this.m_mrtCount];
    this.m_layerRandomNames = new string[this.m_mrtCount];
    for (int index = 0; index < this.m_mrtCount; ++index)
    {
      this.m_layerOffsetNames[index] = "_AO_LayerOffset" + (object) index;
      this.m_layerRandomNames[index] = "_AO_LayerRandom" + (object) index;
    }
    this.m_layerDepthNames = new string[16];
    this.m_layerNormalNames = new string[16];
    this.m_layerOcclusionNames = new string[16];
    for (int index = 0; index < 16; ++index)
    {
      this.m_layerDepthNames[index] = "_AO_DepthLayer" + (object) index;
      this.m_layerNormalNames[index] = "_AO_NormalLayer" + (object) index;
      this.m_layerOcclusionNames[index] = "_AO_OcclusionLayer" + (object) index;
    }
    this.m_depthTargets = new RenderTargetIdentifier[this.m_mrtCount];
    this.m_normalTargets = new RenderTargetIdentifier[this.m_mrtCount];
    if (this.m_mrtCount == 4)
    {
      this.m_deinterleaveDepthPass = 10;
      this.m_deinterleaveNormalPass = 11;
    }
    else
    {
      this.m_deinterleaveDepthPass = 5;
      this.m_deinterleaveNormalPass = 6;
    }
    this.m_applyDeferredTargets = new RenderTargetIdentifier[2];
    if ((UnityEngine.Object) this.m_blitMesh != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_blitMesh);
    this.m_blitMesh = new Mesh();
    this.m_blitMesh.vertices = new Vector3[4]
    {
      new Vector3(0.0f, 0.0f, 0.0f),
      new Vector3(0.0f, 1f, 0.0f),
      new Vector3(1f, 1f, 0.0f),
      new Vector3(1f, 0.0f, 0.0f)
    };
    this.m_blitMesh.uv = new Vector2[4]
    {
      new Vector2(0.0f, 0.0f),
      new Vector2(0.0f, 1f),
      new Vector2(1f, 1f),
      new Vector2(1f, 0.0f)
    };
    this.m_blitMesh.triangles = new int[6]
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
    this.CommandBuffer_UnregisterAll();
    this.SafeReleaseRT(ref this.m_occlusionRT);
    if ((UnityEngine.Object) this.m_occlusionMat != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_occlusionMat);
    if ((UnityEngine.Object) this.m_blurMat != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_blurMat);
    if ((UnityEngine.Object) this.m_copyMat != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_copyMat);
    if (!((UnityEngine.Object) this.m_blitMesh != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_blitMesh);
  }

  private bool CheckRenderTextureFormats()
  {
    if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32) || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
      return false;
    this.m_depthRTFormat = RenderTextureFormat.RFloat;
    if (!SystemInfo.SupportsRenderTextureFormat(this.m_depthRTFormat))
    {
      this.m_depthRTFormat = RenderTextureFormat.RHalf;
      if (!SystemInfo.SupportsRenderTextureFormat(this.m_depthRTFormat))
        this.m_depthRTFormat = RenderTextureFormat.ARGBHalf;
    }
    this.m_normalRTFormat = RenderTextureFormat.ARGB2101010;
    if (!SystemInfo.SupportsRenderTextureFormat(this.m_normalRTFormat))
      this.m_normalRTFormat = RenderTextureFormat.ARGB32;
    this.m_occlusionRTFormat = RenderTextureFormat.RGHalf;
    if (!SystemInfo.SupportsRenderTextureFormat(this.m_occlusionRTFormat))
    {
      this.m_occlusionRTFormat = RenderTextureFormat.RGFloat;
      if (!SystemInfo.SupportsRenderTextureFormat(this.m_occlusionRTFormat))
        this.m_occlusionRTFormat = RenderTextureFormat.ARGBHalf;
    }
    return true;
  }

  private void OnEnable()
  {
    if (!this.CheckRenderTextureFormats())
    {
      Debug.LogError((object) "[AmplifyOcclusion] Target platform does not meet the minimum requirements for this effect to work properly.");
      this.enabled = false;
    }
    else
    {
      this.m_camera = this.GetComponent<Camera>();
      this.Warmup();
      this.CommandBuffer_UnregisterAll();
    }
  }

  private void OnDisable() => this.Shutdown();

  private void OnDestroy() => this.Shutdown();

  private void Update()
  {
    if (this.m_camera.actualRenderingPath != RenderingPath.DeferredShading)
    {
      if (this.PerPixelNormals != AmplifyOcclusionBase.PerPixelNormalSource.None && this.PerPixelNormals != AmplifyOcclusionBase.PerPixelNormalSource.Camera)
      {
        this.PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.Camera;
        Debug.LogWarning((object) "[AmplifyOcclusion] GBuffer Normals only available in Camera Deferred Shading mode. Switched to Camera source.");
      }
      if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred)
      {
        this.ApplyMethod = AmplifyOcclusionBase.ApplicationMethod.PostEffect;
        Debug.LogWarning((object) "[AmplifyOcclusion] Deferred Method requires a Deferred Shading path. Switching to Post Effect Method.");
      }
    }
    if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred && this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.Camera)
    {
      this.PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.GBuffer;
      Debug.LogWarning((object) "[AmplifyOcclusion] Camera Normals not supported for Deferred Method. Switching to GBuffer Normals.");
    }
    if ((this.m_camera.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None)
      this.m_camera.depthTextureMode |= DepthTextureMode.Depth;
    if (this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.Camera && (this.m_camera.depthTextureMode & DepthTextureMode.DepthNormals) == DepthTextureMode.None)
      this.m_camera.depthTextureMode |= DepthTextureMode.DepthNormals;
    this.CheckMaterial();
    this.CheckRandomData();
  }

  private void CheckMaterial()
  {
    if ((UnityEngine.Object) this.m_occlusionMat == (UnityEngine.Object) null)
    {
      Material material = new Material(Shader.Find("Hidden/Amplify Occlusion/Occlusion"));
      material.hideFlags = HideFlags.DontSave;
      this.m_occlusionMat = material;
    }
    if ((UnityEngine.Object) this.m_blurMat == (UnityEngine.Object) null)
    {
      Material material = new Material(Shader.Find("Hidden/Amplify Occlusion/Blur"));
      material.hideFlags = HideFlags.DontSave;
      this.m_blurMat = material;
    }
    if (!((UnityEngine.Object) this.m_copyMat == (UnityEngine.Object) null))
      return;
    Material material1 = new Material(Shader.Find("Hidden/Amplify Occlusion/Copy"));
    material1.hideFlags = HideFlags.DontSave;
    this.m_copyMat = material1;
  }

  private void CheckRandomData()
  {
    if (this.m_randomData != null)
      return;
    this.m_randomData = AmplifyOcclusionBase.GenerateRandomizationData();
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
      float f = (float) (6.2831854820251465 * (double) num3 / 8.0);
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
    cb.SetGlobalVector("_AO_Target_TexelSize", new Vector4(1f / (float) targetWidth, 1f / (float) targetHeight, (float) targetWidth, (float) targetHeight));
    cb.SetGlobalVector("_AO_Target_Position", (Vector4) Vector2.zero);
    cb.SetRenderTarget(targets, targets[0]);
  }

  private void SetBlitTarget(
    CommandBuffer cb,
    RenderTargetIdentifier target,
    int targetWidth,
    int targetHeight)
  {
    cb.SetGlobalVector("_AO_Target_TexelSize", new Vector4(1f / (float) targetWidth, 1f / (float) targetHeight, (float) targetWidth, (float) targetHeight));
    cb.SetRenderTarget(target);
  }

  private void PerformBlit(CommandBuffer cb, Material mat, int pass)
  {
    cb.DrawMesh(this.m_blitMesh, Matrix4x4.identity, mat, 0, pass);
  }

  private void PerformBlit(CommandBuffer cb, Material mat, int pass, int x, int y)
  {
    cb.SetGlobalVector("_AO_Target_Position", (Vector4) new Vector2((float) x, (float) y));
    this.PerformBlit(cb, mat, pass);
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
    cb.SetGlobalVector("_AO_Source_TexelSize", new Vector4(1f / (float) sourceWidth, 1f / (float) sourceHeight, (float) sourceWidth, (float) sourceHeight));
    this.PerformBlit(cb, mat, pass);
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
    this.PerformBlit(cb, source, sourceWidth, sourceHeight, mat, pass);
  }

  private CommandBuffer CommandBuffer_Allocate(string name)
  {
    return new CommandBuffer() { name = name };
  }

  private void CommandBuffer_Register(CameraEvent cameraEvent, CommandBuffer commandBuffer)
  {
    this.m_camera.AddCommandBuffer(cameraEvent, commandBuffer);
    this.m_registeredCommandBuffers.Add(cameraEvent, commandBuffer);
  }

  private void CommandBuffer_Unregister(CameraEvent cameraEvent, CommandBuffer commandBuffer)
  {
    if (!((UnityEngine.Object) this.m_camera != (UnityEngine.Object) null))
      return;
    foreach (CommandBuffer commandBuffer1 in this.m_camera.GetCommandBuffers(cameraEvent))
    {
      if (commandBuffer1.name == commandBuffer.name)
        this.m_camera.RemoveCommandBuffer(cameraEvent, commandBuffer1);
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
    CommandBuffer commandBuffer = this.CommandBuffer_Allocate(name);
    this.CommandBuffer_Register(cameraEvent, commandBuffer);
    return commandBuffer;
  }

  private void CommandBuffer_UnregisterAll()
  {
    foreach (KeyValuePair<CameraEvent, CommandBuffer> registeredCommandBuffer in this.m_registeredCommandBuffers)
      this.CommandBuffer_Unregister(registeredCommandBuffer.Key, registeredCommandBuffer.Value);
    this.m_registeredCommandBuffers.Clear();
  }

  private void UpdateGlobalShaderConstants(AmplifyOcclusionBase.TargetDesc target)
  {
    float num1 = this.m_camera.fieldOfView * ((float) Math.PI / 180f);
    Vector2 vector2_1 = new Vector2((float) (1.0 / (double) Mathf.Tan(num1 * 0.5f) * ((double) target.height / (double) target.width)), 1f / Mathf.Tan(num1 * 0.5f));
    Vector2 vector2_2 = new Vector2(1f / vector2_1.x, 1f / vector2_1.y);
    float num2 = !this.m_camera.orthographic ? (float) target.height / (Mathf.Tan(num1 * 0.5f) * 2f) : (float) target.height / this.m_camera.orthographicSize;
    float num3 = Mathf.Clamp(this.Bias, 0.0f, 1f);
    this.FadeStart = Mathf.Max(0.0f, this.FadeStart);
    this.FadeLength = Mathf.Max(0.01f, this.FadeLength);
    float y = this.FadeEnabled ? 1f / this.FadeLength : 0.0f;
    Shader.SetGlobalMatrix("_AO_CameraProj", GL.GetGPUProjectionMatrix(Matrix4x4.Ortho(0.0f, 1f, 0.0f, 1f, -1f, 100f), false));
    Shader.SetGlobalMatrix("_AO_CameraView", this.m_camera.worldToCameraMatrix);
    Shader.SetGlobalVector("_AO_UVToView", new Vector4(2f * vector2_2.x, -2f * vector2_2.y, -1f * vector2_2.x, 1f * vector2_2.y));
    Shader.SetGlobalFloat("_AO_HalfProjScale", 0.5f * num2);
    Shader.SetGlobalFloat("_AO_Radius", this.Radius);
    Shader.SetGlobalFloat("_AO_PowExponent", this.PowerExponent);
    Shader.SetGlobalFloat("_AO_Bias", num3);
    Shader.SetGlobalFloat("_AO_Multiplier", (float) (1.0 / (1.0 - (double) num3)));
    Shader.SetGlobalFloat("_AO_BlurSharpness", this.BlurSharpness);
    Shader.SetGlobalColor("_AO_Levels", new Color(this.Tint.r, this.Tint.g, this.Tint.b, this.Intensity));
    Shader.SetGlobalVector("_AO_FadeParams", (Vector4) new Vector2(this.FadeStart, y));
    Shader.SetGlobalVector("_AO_FadeValues", (Vector4) new Vector3(this.FadeToIntensity, this.FadeToRadius, this.FadeToPowerExponent));
  }

  private void CommandBuffer_FillComputeOcclusion(
    CommandBuffer cb,
    AmplifyOcclusionBase.TargetDesc target)
  {
    this.CheckMaterial();
    this.CheckRandomData();
    cb.SetGlobalVector("_AO_Buffer_PadScale", new Vector4(target.padRatioWidth, target.padRatioHeight, 1f / target.padRatioWidth, 1f / target.padRatioHeight));
    cb.SetGlobalVector("_AO_Buffer_TexelSize", new Vector4(1f / (float) target.width, 1f / (float) target.height, (float) target.width, (float) target.height));
    cb.SetGlobalVector("_AO_QuarterBuffer_TexelSize", new Vector4(1f / (float) target.quarterWidth, 1f / (float) target.quarterHeight, (float) target.quarterWidth, (float) target.quarterHeight));
    cb.SetGlobalFloat("_AO_MaxRadiusPixels", (float) Mathf.Min(target.width, target.height));
    if ((UnityEngine.Object) this.m_occlusionRT == (UnityEngine.Object) null || this.m_occlusionRT.width != target.width || this.m_occlusionRT.height != target.height || !this.m_occlusionRT.IsCreated())
    {
      this.SafeReleaseRT(ref this.m_occlusionRT);
      this.m_occlusionRT = this.SafeAllocateRT("_AO_OcclusionTexture", target.width, target.height, this.m_occlusionRTFormat, RenderTextureReadWrite.Linear);
    }
    int num1 = -1;
    if (this.Downsample)
      num1 = this.SafeAllocateTemporaryRT(cb, "_AO_SmallOcclusionTexture", target.width / 2, target.height / 2, this.m_occlusionRTFormat, RenderTextureReadWrite.Linear, FilterMode.Bilinear);
    if (this.CacheAware && !this.Downsample)
    {
      int num2 = this.SafeAllocateTemporaryRT(cb, "_AO_OcclusionAtlas", target.width, target.height, this.m_occlusionRTFormat, RenderTextureReadWrite.Linear);
      for (int index = 0; index < 16; ++index)
      {
        this.m_depthLayerRT[index] = this.SafeAllocateTemporaryRT(cb, this.m_layerDepthNames[index], target.quarterWidth, target.quarterHeight, this.m_depthRTFormat, RenderTextureReadWrite.Linear);
        this.m_normalLayerRT[index] = this.SafeAllocateTemporaryRT(cb, this.m_layerNormalNames[index], target.quarterWidth, target.quarterHeight, this.m_normalRTFormat, RenderTextureReadWrite.Linear);
        this.m_occlusionLayerRT[index] = this.SafeAllocateTemporaryRT(cb, this.m_layerOcclusionNames[index], target.quarterWidth, target.quarterHeight, this.m_occlusionRTFormat, RenderTextureReadWrite.Linear);
      }
      for (int index1 = 0; index1 < 16; index1 += this.m_mrtCount)
      {
        for (int index2 = 0; index2 < this.m_mrtCount; ++index2)
        {
          int index3 = index2 + index1;
          int num3 = index3 & 3;
          int num4 = index3 >> 2;
          cb.SetGlobalVector(this.m_layerOffsetNames[index2], (Vector4) new Vector2((float) num3 + 0.5f, (float) num4 + 0.5f));
          this.m_depthTargets[index2] = (RenderTargetIdentifier) this.m_depthLayerRT[index3];
          this.m_normalTargets[index2] = (RenderTargetIdentifier) this.m_normalLayerRT[index3];
        }
        this.SetBlitTarget(cb, this.m_depthTargets, target.quarterWidth, target.quarterHeight);
        this.PerformBlit(cb, this.m_occlusionMat, this.m_deinterleaveDepthPass);
        this.SetBlitTarget(cb, this.m_normalTargets, target.quarterWidth, target.quarterHeight);
        this.PerformBlit(cb, this.m_occlusionMat, (int) (this.m_deinterleaveNormalPass + this.PerPixelNormals));
      }
      for (int index = 0; index < 16; ++index)
      {
        cb.SetGlobalVector("_AO_LayerOffset", (Vector4) new Vector2((float) (index & 3) + 0.5f, (float) (index >> 2) + 0.5f));
        cb.SetGlobalVector("_AO_LayerRandom", (Vector4) this.m_randomData[index]);
        cb.SetGlobalTexture("_AO_NormalTexture", (RenderTargetIdentifier) this.m_normalLayerRT[index]);
        cb.SetGlobalTexture("_AO_DepthTexture", (RenderTargetIdentifier) this.m_depthLayerRT[index]);
        this.SetBlitTarget(cb, (RenderTargetIdentifier) this.m_occlusionLayerRT[index], target.quarterWidth, target.quarterHeight);
        this.PerformBlit(cb, this.m_occlusionMat, (int) (15 + this.SampleCount));
      }
      this.SetBlitTarget(cb, (RenderTargetIdentifier) num2, target.width, target.height);
      for (int index = 0; index < 16; ++index)
      {
        int x = (index & 3) * target.quarterWidth;
        int y = (index >> 2) * target.quarterHeight;
        this.PerformBlit(cb, (RenderTargetIdentifier) this.m_occlusionLayerRT[index], target.quarterWidth, target.quarterHeight, this.m_copyMat, 0, x, y);
      }
      cb.SetGlobalTexture("_AO_OcclusionAtlas", (RenderTargetIdentifier) num2);
      this.SetBlitTarget(cb, (RenderTargetIdentifier) (Texture) this.m_occlusionRT, target.width, target.height);
      this.PerformBlit(cb, this.m_occlusionMat, 19);
      for (int index = 0; index < 16; ++index)
      {
        this.SafeReleaseTemporaryRT(cb, this.m_occlusionLayerRT[index]);
        this.SafeReleaseTemporaryRT(cb, this.m_normalLayerRT[index]);
        this.SafeReleaseTemporaryRT(cb, this.m_depthLayerRT[index]);
      }
      this.SafeReleaseTemporaryRT(cb, num2);
    }
    else
    {
      int pass = (int) (20 + (int) this.SampleCount * 4 + this.PerPixelNormals);
      if (this.Downsample)
      {
        cb.Blit((Texture) null, new RenderTargetIdentifier(num1), this.m_occlusionMat, pass);
        this.SetBlitTarget(cb, (RenderTargetIdentifier) (Texture) this.m_occlusionRT, target.width, target.height);
        this.PerformBlit(cb, (RenderTargetIdentifier) num1, target.width / 2, target.height / 2, this.m_occlusionMat, 41);
      }
      else
        cb.Blit((Texture) null, (RenderTargetIdentifier) (Texture) this.m_occlusionRT, this.m_occlusionMat, pass);
    }
    if (this.BlurEnabled)
    {
      int num5 = this.SafeAllocateTemporaryRT(cb, "_AO_TEMP", target.width, target.height, this.m_occlusionRTFormat, RenderTextureReadWrite.Linear);
      for (int index = 0; index < this.BlurPasses; ++index)
      {
        this.SetBlitTarget(cb, (RenderTargetIdentifier) num5, target.width, target.height);
        this.PerformBlit(cb, (RenderTargetIdentifier) (Texture) this.m_occlusionRT, target.width, target.height, this.m_blurMat, (this.BlurRadius - 1) * 2);
        this.SetBlitTarget(cb, (RenderTargetIdentifier) (Texture) this.m_occlusionRT, target.width, target.height);
        this.PerformBlit(cb, (RenderTargetIdentifier) num5, target.width, target.height, this.m_blurMat, 1 + (this.BlurRadius - 1) * 2);
      }
      this.SafeReleaseTemporaryRT(cb, num5);
    }
    if (this.Downsample && num1 >= 0)
      this.SafeReleaseTemporaryRT(cb, num1);
    cb.SetRenderTarget((RenderTargetIdentifier) (Texture) null);
  }

  private void CommandBuffer_FillApplyDeferred(
    CommandBuffer cb,
    AmplifyOcclusionBase.TargetDesc target,
    bool logTarget)
  {
    cb.SetGlobalTexture("_AO_OcclusionTexture", (RenderTargetIdentifier) (Texture) this.m_occlusionRT);
    this.m_applyDeferredTargets[0] = (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer0;
    this.m_applyDeferredTargets[1] = (RenderTargetIdentifier) (logTarget ? BuiltinRenderTextureType.GBuffer3 : BuiltinRenderTextureType.CameraTarget);
    if (!logTarget)
    {
      this.SetBlitTarget(cb, this.m_applyDeferredTargets, target.fullWidth, target.fullHeight);
      this.PerformBlit(cb, this.m_occlusionMat, 37);
    }
    else
    {
      int num1 = this.SafeAllocateTemporaryRT(cb, "_AO_GBufferAlbedo", target.fullWidth, target.fullHeight, RenderTextureFormat.ARGB32);
      int num2 = this.SafeAllocateTemporaryRT(cb, "_AO_GBufferEmission", target.fullWidth, target.fullHeight, RenderTextureFormat.ARGB32);
      cb.Blit(this.m_applyDeferredTargets[0], (RenderTargetIdentifier) num1);
      cb.Blit(this.m_applyDeferredTargets[1], (RenderTargetIdentifier) num2);
      cb.SetGlobalTexture("_AO_GBufferAlbedo", (RenderTargetIdentifier) num1);
      cb.SetGlobalTexture("_AO_GBufferEmission", (RenderTargetIdentifier) num2);
      this.SetBlitTarget(cb, this.m_applyDeferredTargets, target.fullWidth, target.fullHeight);
      this.PerformBlit(cb, this.m_occlusionMat, 38);
      this.SafeReleaseTemporaryRT(cb, num1);
      this.SafeReleaseTemporaryRT(cb, num2);
    }
    cb.SetRenderTarget((RenderTargetIdentifier) (Texture) null);
  }

  private void CommandBuffer_FillApplyPostEffect(
    CommandBuffer cb,
    AmplifyOcclusionBase.TargetDesc target,
    bool logTarget)
  {
    cb.SetGlobalTexture("_AO_OcclusionTexture", (RenderTargetIdentifier) (Texture) this.m_occlusionRT);
    if (!logTarget)
    {
      this.SetBlitTarget(cb, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, target.fullWidth, target.fullHeight);
      this.PerformBlit(cb, this.m_occlusionMat, 39);
    }
    else
    {
      int num = this.SafeAllocateTemporaryRT(cb, "_AO_GBufferEmission", target.fullWidth, target.fullHeight, RenderTextureFormat.ARGB32);
      cb.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer3, (RenderTargetIdentifier) num);
      cb.SetGlobalTexture("_AO_GBufferEmission", (RenderTargetIdentifier) num);
      this.SetBlitTarget(cb, (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer3, target.fullWidth, target.fullHeight);
      this.PerformBlit(cb, this.m_occlusionMat, 40);
      this.SafeReleaseTemporaryRT(cb, num);
    }
    cb.SetRenderTarget((RenderTargetIdentifier) (Texture) null);
  }

  private void CommandBuffer_FillApplyDebug(
    CommandBuffer cb,
    AmplifyOcclusionBase.TargetDesc target)
  {
    cb.SetGlobalTexture("_AO_OcclusionTexture", (RenderTargetIdentifier) (Texture) this.m_occlusionRT);
    this.SetBlitTarget(cb, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, target.fullWidth, target.fullHeight);
    this.PerformBlit(cb, this.m_occlusionMat, 36);
    cb.SetRenderTarget((RenderTargetIdentifier) (Texture) null);
  }

  private void CommandBuffer_Rebuild(AmplifyOcclusionBase.TargetDesc target)
  {
    bool flag = this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.GBuffer || this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.GBufferOctaEncoded;
    CameraEvent cameraEvent = flag ? CameraEvent.AfterLighting : CameraEvent.BeforeImageEffectsOpaque;
    if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Debug)
    {
      CommandBuffer cb = this.CommandBuffer_AllocateRegister(cameraEvent);
      this.CommandBuffer_FillComputeOcclusion(cb, target);
      this.CommandBuffer_FillApplyDebug(cb, target);
    }
    else
    {
      bool logTarget = !this.m_camera.allowHDR & flag;
      CommandBuffer cb = this.CommandBuffer_AllocateRegister(this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred ? CameraEvent.BeforeReflections : cameraEvent);
      this.CommandBuffer_FillComputeOcclusion(cb, target);
      if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.PostEffect)
        this.CommandBuffer_FillApplyPostEffect(cb, target, logTarget);
      else if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred)
        this.CommandBuffer_FillApplyDeferred(cb, target, logTarget);
    }
  }

  private void OnPreRender()
  {
    bool allowHdr = this.m_camera.allowHDR;
    this.m_target.fullWidth = this.m_camera.pixelWidth;
    this.m_target.fullHeight = this.m_camera.pixelHeight;
    this.m_target.format = allowHdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
    this.m_target.width = this.CacheAware ? this.m_target.fullWidth + 3 & -4 : this.m_target.fullWidth;
    this.m_target.height = this.CacheAware ? this.m_target.fullHeight + 3 & -4 : this.m_target.fullHeight;
    this.m_target.quarterWidth = this.m_target.width / 4;
    this.m_target.quarterHeight = this.m_target.height / 4;
    this.m_target.padRatioWidth = (float) this.m_target.width / (float) this.m_target.fullWidth;
    this.m_target.padRatioHeight = (float) this.m_target.height / (float) this.m_target.fullHeight;
    this.UpdateGlobalShaderConstants(this.m_target);
    if (!this.CheckParamsChanged() && this.m_registeredCommandBuffers.Count != 0)
      return;
    this.CommandBuffer_UnregisterAll();
    this.CommandBuffer_Rebuild(this.m_target);
    this.UpdateParams();
  }

  private void OnPostRender() => this.m_occlusionRT.MarkRestoreExpected();

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
