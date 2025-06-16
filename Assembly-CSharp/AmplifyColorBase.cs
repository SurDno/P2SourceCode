// Decompiled with JetBrains decompiler
// Type: AmplifyColorBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using AmplifyColor;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
[AddComponentMenu("")]
public class AmplifyColorBase : MonoBehaviour
{
  public const int LutSize = 32;
  public const int LutWidth = 1024;
  public const int LutHeight = 32;
  private const int DepthCurveLutRange = 1024;
  public Tonemapping Tonemapper = Tonemapping.Disabled;
  public float Exposure = 1f;
  public float LinearWhitePoint = 11.2f;
  [FormerlySerializedAs("UseDithering")]
  public bool ApplyDithering = false;
  public Quality QualityLevel = Quality.Standard;
  public float BlendAmount = 0.0f;
  public Texture LutTexture = (Texture) null;
  public Texture LutBlendTexture = (Texture) null;
  public Texture MaskTexture = (Texture) null;
  public bool UseDepthMask = false;
  public AnimationCurve DepthMaskCurve = new AnimationCurve(new Keyframe[2]
  {
    new Keyframe(0.0f, 1f),
    new Keyframe(1f, 1f)
  });
  public bool UseVolumes = false;
  public float ExitVolumeBlendTime = 1f;
  public Transform TriggerVolumeProxy = (Transform) null;
  public LayerMask VolumeCollisionMask = (LayerMask) -1;
  private Camera ownerCamera = (Camera) null;
  private Shader shaderBase = (Shader) null;
  private Shader shaderBlend = (Shader) null;
  private Shader shaderBlendCache = (Shader) null;
  private Shader shaderMask = (Shader) null;
  private Shader shaderMaskBlend = (Shader) null;
  private Shader shaderDepthMask = (Shader) null;
  private Shader shaderDepthMaskBlend = (Shader) null;
  private Shader shaderProcessOnly = (Shader) null;
  private RenderTexture blendCacheLut = (RenderTexture) null;
  private Texture2D defaultLut = (Texture2D) null;
  private Texture2D depthCurveLut = (Texture2D) null;
  private Color32[] depthCurveColors = (Color32[]) null;
  private ColorSpace colorSpace = ColorSpace.Uninitialized;
  private Quality qualityLevel = Quality.Standard;
  private Material materialBase = (Material) null;
  private Material materialBlend = (Material) null;
  private Material materialBlendCache = (Material) null;
  private Material materialMask = (Material) null;
  private Material materialMaskBlend = (Material) null;
  private Material materialDepthMask = (Material) null;
  private Material materialDepthMaskBlend = (Material) null;
  private Material materialProcessOnly = (Material) null;
  private bool blending;
  private float blendingTime;
  private float blendingTimeCountdown;
  private Action onFinishBlend;
  private AnimationCurve prevDepthMaskCurve = new AnimationCurve();
  private bool volumesBlending;
  private float volumesBlendingTime;
  private float volumesBlendingTimeCountdown;
  private Texture volumesLutBlendTexture = (Texture) null;
  private float volumesBlendAmount = 0.0f;
  private Texture worldLUT = (Texture) null;
  private AmplifyColorVolumeBase currentVolumeLut = (AmplifyColorVolumeBase) null;
  private RenderTexture midBlendLUT = (RenderTexture) null;
  private bool blendingFromMidBlend = false;
  private VolumeEffect worldVolumeEffects = (VolumeEffect) null;
  private VolumeEffect currentVolumeEffects = (VolumeEffect) null;
  private VolumeEffect blendVolumeEffects = (VolumeEffect) null;
  private float worldExposure = 1f;
  private float currentExposure = 1f;
  private float blendExposure = 1f;
  private float effectVolumesBlendAdjust = 0.0f;
  private List<AmplifyColorVolumeBase> enteredVolumes = new List<AmplifyColorVolumeBase>();
  private AmplifyColorTriggerProxyBase actualTriggerProxy = (AmplifyColorTriggerProxyBase) null;
  [HideInInspector]
  public VolumeEffectFlags EffectFlags = new VolumeEffectFlags();
  [SerializeField]
  [HideInInspector]
  private string sharedInstanceID = "";
  private bool silentError = false;

  public Texture2D DefaultLut
  {
    get => (UnityEngine.Object) this.defaultLut == (UnityEngine.Object) null ? this.CreateDefaultLut() : this.defaultLut;
  }

  public bool IsBlending => this.blending;

  private float effectVolumesBlendAdjusted
  {
    get
    {
      return Mathf.Clamp01((double) this.effectVolumesBlendAdjust < 0.99000000953674316 ? (float) (((double) this.volumesBlendAmount - (double) this.effectVolumesBlendAdjust) / (1.0 - (double) this.effectVolumesBlendAdjust)) : 1f);
    }
  }

  public string SharedInstanceID => this.sharedInstanceID;

  public bool WillItBlend
  {
    get
    {
      return (UnityEngine.Object) this.LutTexture != (UnityEngine.Object) null && (UnityEngine.Object) this.LutBlendTexture != (UnityEngine.Object) null && !this.blending;
    }
  }

  public void NewSharedInstanceID() => this.sharedInstanceID = Guid.NewGuid().ToString();

  private void ReportMissingShaders()
  {
    Debug.LogError((object) "[AmplifyColor] Failed to initialize shaders. Please attempt to re-enable the Amplify Color Effect component. If that fails, please reinstall Amplify Color.");
    this.enabled = false;
  }

  private void ReportNotSupported()
  {
    Debug.LogError((object) "[AmplifyColor] This image effect is not supported on this platform. Please make sure your Unity 4 license supports Full-Screen Post-Processing Effects which is usually reserved for Pro licenses. If this is the case, please contact support@amplify.pt");
    this.enabled = false;
  }

  private bool CheckShader(Shader s)
  {
    if ((UnityEngine.Object) s == (UnityEngine.Object) null)
    {
      this.ReportMissingShaders();
      return false;
    }
    if (s.isSupported)
      return true;
    this.ReportNotSupported();
    return false;
  }

  private bool CheckShaders()
  {
    return this.CheckShader(this.shaderBase) && this.CheckShader(this.shaderBlend) && this.CheckShader(this.shaderBlendCache) && this.CheckShader(this.shaderMask) && this.CheckShader(this.shaderMaskBlend) && this.CheckShader(this.shaderProcessOnly);
  }

  private bool CheckSupport()
  {
    if (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures)
      return true;
    this.ReportNotSupported();
    return false;
  }

  private void OnEnable()
  {
    if (SystemInfo.graphicsDeviceName == "Null Device")
    {
      Debug.LogWarning((object) "[AmplifyColor] Null graphics device detected. Skipping effect silently.");
      this.silentError = true;
    }
    else
    {
      if (!this.CheckSupport() || !this.CreateMaterials())
        return;
      Texture2D lutTexture = this.LutTexture as Texture2D;
      Texture2D lutBlendTexture = this.LutBlendTexture as Texture2D;
      if ((!((UnityEngine.Object) lutTexture != (UnityEngine.Object) null) || lutTexture.mipmapCount <= 1) && (!((UnityEngine.Object) lutBlendTexture != (UnityEngine.Object) null) || lutBlendTexture.mipmapCount <= 1))
        return;
      Debug.LogError((object) "[AmplifyColor] Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. Change Texture Type to \"Advanced\" to access Mip settings.");
    }
  }

  private void OnDisable()
  {
    if ((UnityEngine.Object) this.actualTriggerProxy != (UnityEngine.Object) null)
    {
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.actualTriggerProxy.gameObject);
      this.actualTriggerProxy = (AmplifyColorTriggerProxyBase) null;
    }
    this.ReleaseMaterials();
    this.ReleaseTextures();
  }

  private void VolumesBlendTo(Texture blendTargetLUT, float blendTimeInSec)
  {
    this.volumesLutBlendTexture = blendTargetLUT;
    this.volumesBlendAmount = 0.0f;
    this.volumesBlendingTime = blendTimeInSec;
    this.volumesBlendingTimeCountdown = blendTimeInSec;
    this.volumesBlending = true;
  }

  public void BlendTo(Texture blendTargetLUT, float blendTimeInSec, Action onFinishBlend)
  {
    this.LutBlendTexture = blendTargetLUT;
    this.BlendAmount = 0.0f;
    this.onFinishBlend = onFinishBlend;
    this.blendingTime = blendTimeInSec;
    this.blendingTimeCountdown = blendTimeInSec;
    this.blending = true;
  }

  private void CheckCamera()
  {
    if ((UnityEngine.Object) this.ownerCamera == (UnityEngine.Object) null)
      this.ownerCamera = this.GetComponent<Camera>();
    if (!this.UseDepthMask || (this.ownerCamera.depthTextureMode & DepthTextureMode.Depth) != DepthTextureMode.None)
      return;
    this.ownerCamera.depthTextureMode |= DepthTextureMode.Depth;
  }

  private void Start()
  {
    if (this.silentError)
      return;
    this.CheckCamera();
    this.worldLUT = this.LutTexture;
    this.worldVolumeEffects = this.EffectFlags.GenerateEffectData(this);
    this.blendVolumeEffects = this.currentVolumeEffects = this.worldVolumeEffects;
    this.worldExposure = this.Exposure;
    this.blendExposure = this.currentExposure = this.worldExposure;
  }

  private void Update()
  {
    if (this.silentError)
      return;
    this.CheckCamera();
    bool flag = false;
    if (this.volumesBlending)
    {
      this.volumesBlendAmount = (this.volumesBlendingTime - this.volumesBlendingTimeCountdown) / this.volumesBlendingTime;
      this.volumesBlendingTimeCountdown -= Time.smoothDeltaTime;
      if ((double) this.volumesBlendAmount >= 1.0)
      {
        this.volumesBlendAmount = 1f;
        flag = true;
      }
    }
    else
      this.volumesBlendAmount = Mathf.Clamp01(this.volumesBlendAmount);
    if (this.blending)
    {
      this.BlendAmount = (this.blendingTime - this.blendingTimeCountdown) / this.blendingTime;
      this.blendingTimeCountdown -= Time.smoothDeltaTime;
      if ((double) this.BlendAmount >= 1.0)
      {
        this.LutTexture = this.LutBlendTexture;
        this.BlendAmount = 0.0f;
        this.blending = false;
        this.LutBlendTexture = (Texture) null;
        Action onFinishBlend = this.onFinishBlend;
        if (onFinishBlend != null)
          onFinishBlend();
      }
    }
    else
      this.BlendAmount = Mathf.Clamp01(this.BlendAmount);
    if (this.UseVolumes)
    {
      if ((UnityEngine.Object) this.actualTriggerProxy == (UnityEngine.Object) null)
      {
        GameObject gameObject = new GameObject(this.name + "+ACVolumeProxy");
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        this.actualTriggerProxy = (AmplifyColorTriggerProxyBase) gameObject.AddComponent<AmplifyColorTriggerProxy>();
        this.actualTriggerProxy.OwnerEffect = this;
      }
      this.UpdateVolumes();
    }
    else if ((UnityEngine.Object) this.actualTriggerProxy != (UnityEngine.Object) null)
    {
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.actualTriggerProxy.gameObject);
      this.actualTriggerProxy = (AmplifyColorTriggerProxyBase) null;
    }
    if (!flag)
      return;
    this.LutTexture = this.volumesLutBlendTexture;
    this.volumesBlendAmount = 0.0f;
    this.volumesBlending = false;
    this.volumesLutBlendTexture = (Texture) null;
    this.effectVolumesBlendAdjust = 0.0f;
    this.currentVolumeEffects = this.blendVolumeEffects;
    this.currentVolumeEffects.SetValues(this);
    this.currentExposure = this.blendExposure;
    if (this.blendingFromMidBlend && (UnityEngine.Object) this.midBlendLUT != (UnityEngine.Object) null)
      this.midBlendLUT.DiscardContents();
    this.blendingFromMidBlend = false;
  }

  public void EnterVolume(AmplifyColorVolumeBase volume)
  {
    if (this.enteredVolumes.Contains(volume))
      return;
    this.enteredVolumes.Insert(0, volume);
  }

  public void ExitVolume(AmplifyColorVolumeBase volume)
  {
    if (!this.enteredVolumes.Contains(volume))
      return;
    this.enteredVolumes.Remove(volume);
  }

  private void UpdateVolumes()
  {
    if (this.volumesBlending)
      this.currentVolumeEffects.BlendValues(this, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
    if (this.volumesBlending)
      this.Exposure = Mathf.Lerp(this.currentExposure, this.blendExposure, this.effectVolumesBlendAdjusted);
    Transform transform = (UnityEngine.Object) this.TriggerVolumeProxy == (UnityEngine.Object) null ? this.transform : this.TriggerVolumeProxy;
    if ((UnityEngine.Object) this.actualTriggerProxy.transform.parent != (UnityEngine.Object) transform)
    {
      this.actualTriggerProxy.Reference = transform;
      this.actualTriggerProxy.gameObject.layer = transform.gameObject.layer;
    }
    AmplifyColorVolumeBase amplifyColorVolumeBase = (AmplifyColorVolumeBase) null;
    int num = int.MinValue;
    for (int index = 0; index < this.enteredVolumes.Count; ++index)
    {
      AmplifyColorVolumeBase enteredVolume = this.enteredVolumes[index];
      if (enteredVolume.Priority > num)
      {
        amplifyColorVolumeBase = enteredVolume;
        num = enteredVolume.Priority;
      }
    }
    if (!((UnityEngine.Object) amplifyColorVolumeBase != (UnityEngine.Object) this.currentVolumeLut))
      return;
    this.currentVolumeLut = amplifyColorVolumeBase;
    Texture blendTargetLUT = (UnityEngine.Object) amplifyColorVolumeBase == (UnityEngine.Object) null ? this.worldLUT : (Texture) amplifyColorVolumeBase.LutTexture;
    float blendTimeInSec = (UnityEngine.Object) amplifyColorVolumeBase == (UnityEngine.Object) null ? this.ExitVolumeBlendTime : amplifyColorVolumeBase.EnterBlendTime;
    if (this.volumesBlending && !this.blendingFromMidBlend && (UnityEngine.Object) blendTargetLUT == (UnityEngine.Object) this.LutTexture)
    {
      this.LutTexture = this.volumesLutBlendTexture;
      this.volumesLutBlendTexture = blendTargetLUT;
      this.volumesBlendingTimeCountdown = blendTimeInSec * ((this.volumesBlendingTime - this.volumesBlendingTimeCountdown) / this.volumesBlendingTime);
      this.volumesBlendingTime = blendTimeInSec;
      this.currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(this.EffectFlags, this.currentVolumeEffects, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
      this.currentExposure = Mathf.Lerp(this.currentExposure, this.blendExposure, this.effectVolumesBlendAdjusted);
      this.effectVolumesBlendAdjust = 1f - this.volumesBlendAmount;
      this.volumesBlendAmount = 1f - this.volumesBlendAmount;
    }
    else
    {
      if (this.volumesBlending)
      {
        this.materialBlendCache.SetFloat("_LerpAmount", this.volumesBlendAmount);
        if (this.blendingFromMidBlend)
        {
          Graphics.Blit((Texture) this.midBlendLUT, this.blendCacheLut);
          this.materialBlendCache.SetTexture("_RgbTex", (Texture) this.blendCacheLut);
        }
        else
          this.materialBlendCache.SetTexture("_RgbTex", this.LutTexture);
        this.materialBlendCache.SetTexture("_LerpRgbTex", (UnityEngine.Object) this.volumesLutBlendTexture != (UnityEngine.Object) null ? this.volumesLutBlendTexture : (Texture) this.defaultLut);
        Graphics.Blit((Texture) this.midBlendLUT, this.midBlendLUT, this.materialBlendCache);
        this.blendCacheLut.DiscardContents();
        this.midBlendLUT.MarkRestoreExpected();
        this.currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(this.EffectFlags, this.currentVolumeEffects, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
        this.currentExposure = Mathf.Lerp(this.currentExposure, this.blendExposure, this.effectVolumesBlendAdjusted);
        this.effectVolumesBlendAdjust = 0.0f;
        this.blendingFromMidBlend = true;
      }
      this.VolumesBlendTo(blendTargetLUT, blendTimeInSec);
    }
    this.blendVolumeEffects = (UnityEngine.Object) amplifyColorVolumeBase == (UnityEngine.Object) null ? this.worldVolumeEffects : amplifyColorVolumeBase.EffectContainer.FindVolumeEffect(this);
    this.blendExposure = (UnityEngine.Object) amplifyColorVolumeBase == (UnityEngine.Object) null ? this.worldExposure : amplifyColorVolumeBase.Exposure;
    if (this.blendVolumeEffects == null)
      this.blendVolumeEffects = this.worldVolumeEffects;
  }

  private void SetupShader()
  {
    this.colorSpace = QualitySettings.activeColorSpace;
    this.qualityLevel = this.QualityLevel;
    this.shaderBase = Shader.Find("Hidden/Amplify Color/Base");
    this.shaderBlend = Shader.Find("Hidden/Amplify Color/Blend");
    this.shaderBlendCache = Shader.Find("Hidden/Amplify Color/BlendCache");
    this.shaderMask = Shader.Find("Hidden/Amplify Color/Mask");
    this.shaderMaskBlend = Shader.Find("Hidden/Amplify Color/MaskBlend");
    this.shaderDepthMask = Shader.Find("Hidden/Amplify Color/DepthMask");
    this.shaderDepthMaskBlend = Shader.Find("Hidden/Amplify Color/DepthMaskBlend");
    this.shaderProcessOnly = Shader.Find("Hidden/Amplify Color/ProcessOnly");
  }

  private void ReleaseMaterials()
  {
    this.SafeRelease<Material>(ref this.materialBase);
    this.SafeRelease<Material>(ref this.materialBlend);
    this.SafeRelease<Material>(ref this.materialBlendCache);
    this.SafeRelease<Material>(ref this.materialMask);
    this.SafeRelease<Material>(ref this.materialMaskBlend);
    this.SafeRelease<Material>(ref this.materialDepthMask);
    this.SafeRelease<Material>(ref this.materialDepthMaskBlend);
    this.SafeRelease<Material>(ref this.materialProcessOnly);
  }

  private Texture2D CreateDefaultLut()
  {
    Texture2D texture2D = new Texture2D(1024, 32, TextureFormat.RGB24, false, true);
    texture2D.hideFlags = HideFlags.HideAndDontSave;
    this.defaultLut = texture2D;
    this.defaultLut.name = "DefaultLut";
    this.defaultLut.hideFlags = HideFlags.DontSave;
    this.defaultLut.anisoLevel = 1;
    this.defaultLut.filterMode = FilterMode.Bilinear;
    Color32[] colors = new Color32[32768];
    for (int index1 = 0; index1 < 32; ++index1)
    {
      int num1 = index1 * 32;
      for (int index2 = 0; index2 < 32; ++index2)
      {
        int num2 = num1 + index2 * 1024;
        for (int index3 = 0; index3 < 32; ++index3)
        {
          float num3 = (float) index3 / 31f;
          float num4 = (float) index2 / 31f;
          float num5 = (float) index1 / 31f;
          byte r = (byte) ((double) num3 * (double) byte.MaxValue);
          byte g = (byte) ((double) num4 * (double) byte.MaxValue);
          byte b = (byte) ((double) num5 * (double) byte.MaxValue);
          colors[num2 + index3] = new Color32(r, g, b, byte.MaxValue);
        }
      }
    }
    this.defaultLut.SetPixels32(colors);
    this.defaultLut.Apply();
    return this.defaultLut;
  }

  private Texture2D CreateDepthCurveLut()
  {
    this.SafeRelease<Texture2D>(ref this.depthCurveLut);
    Texture2D texture2D = new Texture2D(1024, 1, TextureFormat.Alpha8, false, true);
    texture2D.hideFlags = HideFlags.HideAndDontSave;
    this.depthCurveLut = texture2D;
    this.depthCurveLut.name = "DepthCurveLut";
    this.depthCurveLut.hideFlags = HideFlags.DontSave;
    this.depthCurveLut.anisoLevel = 1;
    this.depthCurveLut.wrapMode = TextureWrapMode.Clamp;
    this.depthCurveLut.filterMode = FilterMode.Bilinear;
    this.depthCurveColors = new Color32[1024];
    return this.depthCurveLut;
  }

  private void UpdateDepthCurveLut()
  {
    if ((UnityEngine.Object) this.depthCurveLut == (UnityEngine.Object) null)
      this.CreateDepthCurveLut();
    float time = 0.0f;
    int index = 0;
    while (index < 1024)
    {
      this.depthCurveColors[index].a = (byte) Mathf.FloorToInt(Mathf.Clamp01(this.DepthMaskCurve.Evaluate(time)) * (float) byte.MaxValue);
      ++index;
      time += 0.0009775171f;
    }
    this.depthCurveLut.SetPixels32(this.depthCurveColors);
    this.depthCurveLut.Apply();
  }

  private void CheckUpdateDepthCurveLut()
  {
    bool flag = false;
    if (this.DepthMaskCurve.length != this.prevDepthMaskCurve.length)
    {
      flag = true;
    }
    else
    {
      float time = 0.0f;
      int num = 0;
      while (num < this.DepthMaskCurve.length)
      {
        if ((double) Mathf.Abs(this.DepthMaskCurve.Evaluate(time) - this.prevDepthMaskCurve.Evaluate(time)) > 1.4012984643248171E-45)
        {
          flag = true;
          break;
        }
        ++num;
        time += 0.0009775171f;
      }
    }
    if (!((UnityEngine.Object) this.depthCurveLut == (UnityEngine.Object) null | flag))
      return;
    this.UpdateDepthCurveLut();
    this.prevDepthMaskCurve = new AnimationCurve(this.DepthMaskCurve.keys);
  }

  private void CreateHelperTextures()
  {
    this.ReleaseTextures();
    RenderTexture renderTexture1 = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
    renderTexture1.hideFlags = HideFlags.HideAndDontSave;
    this.blendCacheLut = renderTexture1;
    this.blendCacheLut.name = "BlendCacheLut";
    this.blendCacheLut.wrapMode = TextureWrapMode.Clamp;
    this.blendCacheLut.useMipMap = false;
    this.blendCacheLut.anisoLevel = 0;
    this.blendCacheLut.Create();
    RenderTexture renderTexture2 = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
    renderTexture2.hideFlags = HideFlags.HideAndDontSave;
    this.midBlendLUT = renderTexture2;
    this.midBlendLUT.name = "MidBlendLut";
    this.midBlendLUT.wrapMode = TextureWrapMode.Clamp;
    this.midBlendLUT.useMipMap = false;
    this.midBlendLUT.anisoLevel = 0;
    this.midBlendLUT.Create();
    this.midBlendLUT.MarkRestoreExpected();
    this.CreateDefaultLut();
    if (!this.UseDepthMask)
      return;
    this.CreateDepthCurveLut();
  }

  private bool CheckMaterialAndShader(Material material, string name)
  {
    if ((UnityEngine.Object) material == (UnityEngine.Object) null || (UnityEngine.Object) material.shader == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) ("[AmplifyColor] Error creating " + name + " material. Effect disabled."));
      this.enabled = false;
    }
    else if (!material.shader.isSupported)
    {
      Debug.LogWarning((object) ("[AmplifyColor] " + name + " shader not supported on this platform. Effect disabled."));
      this.enabled = false;
    }
    else
      material.hideFlags = HideFlags.HideAndDontSave;
    return this.enabled;
  }

  private bool CreateMaterials()
  {
    this.SetupShader();
    if (!this.CheckShaders())
      return false;
    this.ReleaseMaterials();
    this.materialBase = new Material(this.shaderBase);
    this.materialBlend = new Material(this.shaderBlend);
    this.materialBlendCache = new Material(this.shaderBlendCache);
    this.materialMask = new Material(this.shaderMask);
    this.materialMaskBlend = new Material(this.shaderMaskBlend);
    this.materialDepthMask = new Material(this.shaderDepthMask);
    this.materialDepthMaskBlend = new Material(this.shaderDepthMaskBlend);
    this.materialProcessOnly = new Material(this.shaderProcessOnly);
    if (!this.CheckMaterialAndShader(this.materialBase, "BaseMaterial") || !this.CheckMaterialAndShader(this.materialBlend, "BlendMaterial") || !this.CheckMaterialAndShader(this.materialBlendCache, "BlendCacheMaterial") || !this.CheckMaterialAndShader(this.materialMask, "MaskMaterial") || !this.CheckMaterialAndShader(this.materialMaskBlend, "MaskBlendMaterial") || !this.CheckMaterialAndShader(this.materialDepthMask, "DepthMaskMaterial") || !this.CheckMaterialAndShader(this.materialDepthMaskBlend, "DepthMaskBlendMaterial") || !this.CheckMaterialAndShader(this.materialProcessOnly, "ProcessOnlyMaterial"))
      return false;
    this.CreateHelperTextures();
    return true;
  }

  private void SetMaterialKeyword(string keyword, bool state)
  {
    if (state)
      Shader.EnableKeyword(keyword);
    else
      Shader.DisableKeyword(keyword);
  }

  private void SafeRelease<T>(ref T obj) where T : UnityEngine.Object
  {
    if (!((UnityEngine.Object) obj != (UnityEngine.Object) null))
      return;
    if (obj.GetType() == typeof (RenderTexture))
      ((object) obj as RenderTexture).Release();
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) obj);
    obj = default (T);
  }

  private void ReleaseTextures()
  {
    RenderTexture.active = (RenderTexture) null;
    this.SafeRelease<RenderTexture>(ref this.blendCacheLut);
    this.SafeRelease<RenderTexture>(ref this.midBlendLUT);
    this.SafeRelease<Texture2D>(ref this.defaultLut);
    this.SafeRelease<Texture2D>(ref this.depthCurveLut);
  }

  public static bool ValidateLutDimensions(Texture lut)
  {
    bool flag = true;
    if ((UnityEngine.Object) lut != (UnityEngine.Object) null)
    {
      if (lut.width / lut.height != lut.height)
      {
        Debug.LogWarning((object) ("[AmplifyColor] Lut " + lut.name + " has invalid dimensions."));
        flag = false;
      }
      else if (lut.anisoLevel != 0)
        lut.anisoLevel = 0;
    }
    return flag;
  }

  private void UpdatePostEffectParams()
  {
    if (this.UseDepthMask)
      this.CheckUpdateDepthCurveLut();
    this.Exposure = Mathf.Max(this.Exposure, 0.0f);
  }

  private int ComputeShaderPass()
  {
    bool flag1 = this.QualityLevel == Quality.Mobile;
    bool flag2 = this.colorSpace == ColorSpace.Linear;
    bool allowHdr = this.ownerCamera.allowHDR;
    int num = flag1 ? 18 : 0;
    return !allowHdr ? num + (flag2 ? 1 : 0) : (int) (num + 2 + (flag2 ? 8 : 0) + (this.ApplyDithering ? 4 : 0) + this.Tonemapper);
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (this.silentError)
    {
      Graphics.Blit((Texture) source, destination);
    }
    else
    {
      this.BlendAmount = Mathf.Clamp01(this.BlendAmount);
      if (this.colorSpace != QualitySettings.activeColorSpace || this.qualityLevel != this.QualityLevel)
        this.CreateMaterials();
      this.UpdatePostEffectParams();
      bool flag1 = AmplifyColorBase.ValidateLutDimensions(this.LutTexture);
      bool flag2 = AmplifyColorBase.ValidateLutDimensions(this.LutBlendTexture);
      bool flag3 = (UnityEngine.Object) this.LutTexture == (UnityEngine.Object) null && (UnityEngine.Object) this.LutBlendTexture == (UnityEngine.Object) null && (UnityEngine.Object) this.volumesLutBlendTexture == (UnityEngine.Object) null;
      Texture source1 = (UnityEngine.Object) this.LutTexture == (UnityEngine.Object) null ? (Texture) this.defaultLut : this.LutTexture;
      Texture lutBlendTexture = this.LutBlendTexture;
      int shaderPass = this.ComputeShaderPass();
      bool flag4 = (double) this.BlendAmount != 0.0 || this.blending;
      bool flag5 = flag4 || flag4 && (UnityEngine.Object) lutBlendTexture != (UnityEngine.Object) null;
      bool flag6 = flag5;
      bool flag7 = ((!flag1 ? 1 : (!flag2 ? 1 : 0)) | (flag3 ? 1 : 0)) != 0;
      Material mat = !flag7 ? (!flag5 && !this.volumesBlending ? (!this.UseDepthMask ? ((UnityEngine.Object) this.MaskTexture != (UnityEngine.Object) null ? this.materialMask : this.materialBase) : this.materialDepthMask) : (!this.UseDepthMask ? ((UnityEngine.Object) this.MaskTexture != (UnityEngine.Object) null ? this.materialMaskBlend : this.materialBlend) : this.materialDepthMaskBlend)) : this.materialProcessOnly;
      mat.SetFloat("_Exposure", this.Exposure);
      mat.SetFloat("_ShoulderStrength", 0.22f);
      mat.SetFloat("_LinearStrength", 0.3f);
      mat.SetFloat("_LinearAngle", 0.1f);
      mat.SetFloat("_ToeStrength", 0.2f);
      mat.SetFloat("_ToeNumerator", 0.01f);
      mat.SetFloat("_ToeDenominator", 0.3f);
      mat.SetFloat("_LinearWhite", this.LinearWhitePoint);
      mat.SetFloat("_LerpAmount", this.BlendAmount);
      if ((UnityEngine.Object) this.MaskTexture != (UnityEngine.Object) null)
        mat.SetTexture("_MaskTex", this.MaskTexture);
      if (this.UseDepthMask)
        mat.SetTexture("_DepthCurveLut", (Texture) this.depthCurveLut);
      if (!flag7)
      {
        if (this.volumesBlending)
        {
          this.volumesBlendAmount = Mathf.Clamp01(this.volumesBlendAmount);
          this.materialBlendCache.SetFloat("_LerpAmount", this.volumesBlendAmount);
          if (this.blendingFromMidBlend)
            this.materialBlendCache.SetTexture("_RgbTex", (Texture) this.midBlendLUT);
          else
            this.materialBlendCache.SetTexture("_RgbTex", source1);
          this.materialBlendCache.SetTexture("_LerpRgbTex", (UnityEngine.Object) this.volumesLutBlendTexture != (UnityEngine.Object) null ? this.volumesLutBlendTexture : (Texture) this.defaultLut);
          Graphics.Blit(source1, this.blendCacheLut, this.materialBlendCache);
        }
        if (flag6)
        {
          this.materialBlendCache.SetFloat("_LerpAmount", this.BlendAmount);
          RenderTexture renderTexture = (RenderTexture) null;
          if (this.volumesBlending)
          {
            renderTexture = RenderTexture.GetTemporary(this.blendCacheLut.width, this.blendCacheLut.height, this.blendCacheLut.depth, this.blendCacheLut.format, RenderTextureReadWrite.Linear);
            Graphics.Blit((Texture) this.blendCacheLut, renderTexture);
            this.materialBlendCache.SetTexture("_RgbTex", (Texture) renderTexture);
          }
          else
            this.materialBlendCache.SetTexture("_RgbTex", source1);
          this.materialBlendCache.SetTexture("_LerpRgbTex", (UnityEngine.Object) lutBlendTexture != (UnityEngine.Object) null ? lutBlendTexture : (Texture) this.defaultLut);
          Graphics.Blit(source1, this.blendCacheLut, this.materialBlendCache);
          if ((UnityEngine.Object) renderTexture != (UnityEngine.Object) null)
            RenderTexture.ReleaseTemporary(renderTexture);
          mat.SetTexture("_RgbBlendCacheTex", (Texture) this.blendCacheLut);
        }
        else if (this.volumesBlending)
        {
          mat.SetTexture("_RgbBlendCacheTex", (Texture) this.blendCacheLut);
        }
        else
        {
          if ((UnityEngine.Object) source1 != (UnityEngine.Object) null)
            mat.SetTexture("_RgbTex", source1);
          if ((UnityEngine.Object) lutBlendTexture != (UnityEngine.Object) null)
            mat.SetTexture("_LerpRgbTex", lutBlendTexture);
        }
      }
      Graphics.Blit((Texture) source, destination, mat, shaderPass);
      if (!flag6 && !this.volumesBlending)
        return;
      this.blendCacheLut.DiscardContents();
    }
  }
}
