using System;
using System.Collections.Generic;
using AmplifyColor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

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
  public bool ApplyDithering;
  public Quality QualityLevel = Quality.Standard;
  public float BlendAmount;
  public Texture LutTexture;
  public Texture LutBlendTexture;
  public Texture MaskTexture;
  public bool UseDepthMask;
  public AnimationCurve DepthMaskCurve = new AnimationCurve(new Keyframe(0.0f, 1f), new Keyframe(1f, 1f));
  public bool UseVolumes;
  public float ExitVolumeBlendTime = 1f;
  public Transform TriggerVolumeProxy;
  public LayerMask VolumeCollisionMask = -1;
  private Camera ownerCamera;
  private Shader shaderBase;
  private Shader shaderBlend;
  private Shader shaderBlendCache;
  private Shader shaderMask;
  private Shader shaderMaskBlend;
  private Shader shaderDepthMask;
  private Shader shaderDepthMaskBlend;
  private Shader shaderProcessOnly;
  private RenderTexture blendCacheLut;
  private Texture2D defaultLut;
  private Texture2D depthCurveLut;
  private Color32[] depthCurveColors;
  private ColorSpace colorSpace = ColorSpace.Uninitialized;
  private Quality qualityLevel = Quality.Standard;
  private Material materialBase;
  private Material materialBlend;
  private Material materialBlendCache;
  private Material materialMask;
  private Material materialMaskBlend;
  private Material materialDepthMask;
  private Material materialDepthMaskBlend;
  private Material materialProcessOnly;
  private bool blending;
  private float blendingTime;
  private float blendingTimeCountdown;
  private Action onFinishBlend;
  private AnimationCurve prevDepthMaskCurve = new AnimationCurve();
  private bool volumesBlending;
  private float volumesBlendingTime;
  private float volumesBlendingTimeCountdown;
  private Texture volumesLutBlendTexture;
  private float volumesBlendAmount;
  private Texture worldLUT;
  private AmplifyColorVolumeBase currentVolumeLut;
  private RenderTexture midBlendLUT;
  private bool blendingFromMidBlend;
  private VolumeEffect worldVolumeEffects;
  private VolumeEffect currentVolumeEffects;
  private VolumeEffect blendVolumeEffects;
  private float worldExposure = 1f;
  private float currentExposure = 1f;
  private float blendExposure = 1f;
  private float effectVolumesBlendAdjust;
  private List<AmplifyColorVolumeBase> enteredVolumes = new List<AmplifyColorVolumeBase>();
  private AmplifyColorTriggerProxyBase actualTriggerProxy;
  [HideInInspector]
  public VolumeEffectFlags EffectFlags = new VolumeEffectFlags();
  [SerializeField]
  [HideInInspector]
  private string sharedInstanceID = "";
  private bool silentError;

  public Texture2D DefaultLut
  {
    get => defaultLut == null ? CreateDefaultLut() : defaultLut;
  }

  public bool IsBlending => blending;

  private float effectVolumesBlendAdjusted
  {
    get
    {
      return Mathf.Clamp01(effectVolumesBlendAdjust < 0.99000000953674316 ? (float) ((volumesBlendAmount - (double) effectVolumesBlendAdjust) / (1.0 - effectVolumesBlendAdjust)) : 1f);
    }
  }

  public string SharedInstanceID => sharedInstanceID;

  public bool WillItBlend
  {
    get
    {
      return LutTexture != null && LutBlendTexture != null && !blending;
    }
  }

  public void NewSharedInstanceID() => sharedInstanceID = Guid.NewGuid().ToString();

  private void ReportMissingShaders()
  {
    Debug.LogError("[AmplifyColor] Failed to initialize shaders. Please attempt to re-enable the Amplify Color Effect component. If that fails, please reinstall Amplify Color.");
    enabled = false;
  }

  private void ReportNotSupported()
  {
    Debug.LogError("[AmplifyColor] This image effect is not supported on this platform. Please make sure your Unity 4 license supports Full-Screen Post-Processing Effects which is usually reserved for Pro licenses. If this is the case, please contact support@amplify.pt");
    enabled = false;
  }

  private bool CheckShader(Shader s)
  {
    if (s == null)
    {
      ReportMissingShaders();
      return false;
    }
    if (s.isSupported)
      return true;
    ReportNotSupported();
    return false;
  }

  private bool CheckShaders()
  {
    return CheckShader(shaderBase) && CheckShader(shaderBlend) && CheckShader(shaderBlendCache) && CheckShader(shaderMask) && CheckShader(shaderMaskBlend) && CheckShader(shaderProcessOnly);
  }

  private bool CheckSupport()
  {
    if (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures)
      return true;
    ReportNotSupported();
    return false;
  }

  private void OnEnable()
  {
    if (SystemInfo.graphicsDeviceName == "Null Device")
    {
      Debug.LogWarning("[AmplifyColor] Null graphics device detected. Skipping effect silently.");
      silentError = true;
    }
    else
    {
      if (!CheckSupport() || !CreateMaterials())
        return;
      Texture2D lutTexture = LutTexture as Texture2D;
      Texture2D lutBlendTexture = LutBlendTexture as Texture2D;
      if ((!(lutTexture != null) || lutTexture.mipmapCount <= 1) && (!(lutBlendTexture != null) || lutBlendTexture.mipmapCount <= 1))
        return;
      Debug.LogError("[AmplifyColor] Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. Change Texture Type to \"Advanced\" to access Mip settings.");
    }
  }

  private void OnDisable()
  {
    if (actualTriggerProxy != null)
    {
      DestroyImmediate(actualTriggerProxy.gameObject);
      actualTriggerProxy = null;
    }
    ReleaseMaterials();
    ReleaseTextures();
  }

  private void VolumesBlendTo(Texture blendTargetLUT, float blendTimeInSec)
  {
    volumesLutBlendTexture = blendTargetLUT;
    volumesBlendAmount = 0.0f;
    volumesBlendingTime = blendTimeInSec;
    volumesBlendingTimeCountdown = blendTimeInSec;
    volumesBlending = true;
  }

  public void BlendTo(Texture blendTargetLUT, float blendTimeInSec, Action onFinishBlend)
  {
    LutBlendTexture = blendTargetLUT;
    BlendAmount = 0.0f;
    this.onFinishBlend = onFinishBlend;
    blendingTime = blendTimeInSec;
    blendingTimeCountdown = blendTimeInSec;
    blending = true;
  }

  private void CheckCamera()
  {
    if (ownerCamera == null)
      ownerCamera = GetComponent<Camera>();
    if (!UseDepthMask || (ownerCamera.depthTextureMode & DepthTextureMode.Depth) != DepthTextureMode.None)
      return;
    ownerCamera.depthTextureMode |= DepthTextureMode.Depth;
  }

  private void Start()
  {
    if (silentError)
      return;
    CheckCamera();
    worldLUT = LutTexture;
    worldVolumeEffects = EffectFlags.GenerateEffectData(this);
    blendVolumeEffects = currentVolumeEffects = worldVolumeEffects;
    worldExposure = Exposure;
    blendExposure = currentExposure = worldExposure;
  }

  private void Update()
  {
    if (silentError)
      return;
    CheckCamera();
    bool flag = false;
    if (volumesBlending)
    {
      volumesBlendAmount = (volumesBlendingTime - volumesBlendingTimeCountdown) / volumesBlendingTime;
      volumesBlendingTimeCountdown -= Time.smoothDeltaTime;
      if (volumesBlendAmount >= 1.0)
      {
        volumesBlendAmount = 1f;
        flag = true;
      }
    }
    else
      volumesBlendAmount = Mathf.Clamp01(volumesBlendAmount);
    if (blending)
    {
      BlendAmount = (blendingTime - blendingTimeCountdown) / blendingTime;
      blendingTimeCountdown -= Time.smoothDeltaTime;
      if (BlendAmount >= 1.0)
      {
        LutTexture = LutBlendTexture;
        BlendAmount = 0.0f;
        blending = false;
        LutBlendTexture = null;
        Action onFinishBlend = this.onFinishBlend;
        if (onFinishBlend != null)
          onFinishBlend();
      }
    }
    else
      BlendAmount = Mathf.Clamp01(BlendAmount);
    if (UseVolumes)
    {
      if (actualTriggerProxy == null)
      {
        GameObject gameObject = new GameObject(name + "+ACVolumeProxy");
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        actualTriggerProxy = gameObject.AddComponent<AmplifyColorTriggerProxy>();
        actualTriggerProxy.OwnerEffect = this;
      }
      UpdateVolumes();
    }
    else if (actualTriggerProxy != null)
    {
      DestroyImmediate(actualTriggerProxy.gameObject);
      actualTriggerProxy = null;
    }
    if (!flag)
      return;
    LutTexture = volumesLutBlendTexture;
    volumesBlendAmount = 0.0f;
    volumesBlending = false;
    volumesLutBlendTexture = null;
    effectVolumesBlendAdjust = 0.0f;
    currentVolumeEffects = blendVolumeEffects;
    currentVolumeEffects.SetValues(this);
    currentExposure = blendExposure;
    if (blendingFromMidBlend && midBlendLUT != null)
      midBlendLUT.DiscardContents();
    blendingFromMidBlend = false;
  }

  public void EnterVolume(AmplifyColorVolumeBase volume)
  {
    if (enteredVolumes.Contains(volume))
      return;
    enteredVolumes.Insert(0, volume);
  }

  public void ExitVolume(AmplifyColorVolumeBase volume)
  {
    if (!enteredVolumes.Contains(volume))
      return;
    enteredVolumes.Remove(volume);
  }

  private void UpdateVolumes()
  {
    if (volumesBlending)
      currentVolumeEffects.BlendValues(this, blendVolumeEffects, effectVolumesBlendAdjusted);
    if (volumesBlending)
      Exposure = Mathf.Lerp(currentExposure, blendExposure, effectVolumesBlendAdjusted);
    Transform transform = TriggerVolumeProxy == null ? this.transform : TriggerVolumeProxy;
    if (actualTriggerProxy.transform.parent != transform)
    {
      actualTriggerProxy.Reference = transform;
      actualTriggerProxy.gameObject.layer = transform.gameObject.layer;
    }
    AmplifyColorVolumeBase amplifyColorVolumeBase = null;
    int num = int.MinValue;
    for (int index = 0; index < enteredVolumes.Count; ++index)
    {
      AmplifyColorVolumeBase enteredVolume = enteredVolumes[index];
      if (enteredVolume.Priority > num)
      {
        amplifyColorVolumeBase = enteredVolume;
        num = enteredVolume.Priority;
      }
    }
    if (!(amplifyColorVolumeBase != currentVolumeLut))
      return;
    currentVolumeLut = amplifyColorVolumeBase;
    Texture blendTargetLUT = amplifyColorVolumeBase == null ? worldLUT : amplifyColorVolumeBase.LutTexture;
    float blendTimeInSec = amplifyColorVolumeBase == null ? ExitVolumeBlendTime : amplifyColorVolumeBase.EnterBlendTime;
    if (volumesBlending && !blendingFromMidBlend && blendTargetLUT == LutTexture)
    {
      LutTexture = volumesLutBlendTexture;
      volumesLutBlendTexture = blendTargetLUT;
      volumesBlendingTimeCountdown = blendTimeInSec * ((volumesBlendingTime - volumesBlendingTimeCountdown) / volumesBlendingTime);
      volumesBlendingTime = blendTimeInSec;
      currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(EffectFlags, currentVolumeEffects, blendVolumeEffects, effectVolumesBlendAdjusted);
      currentExposure = Mathf.Lerp(currentExposure, blendExposure, effectVolumesBlendAdjusted);
      effectVolumesBlendAdjust = 1f - volumesBlendAmount;
      volumesBlendAmount = 1f - volumesBlendAmount;
    }
    else
    {
      if (volumesBlending)
      {
        materialBlendCache.SetFloat("_LerpAmount", volumesBlendAmount);
        if (blendingFromMidBlend)
        {
          Graphics.Blit(midBlendLUT, blendCacheLut);
          materialBlendCache.SetTexture("_RgbTex", blendCacheLut);
        }
        else
          materialBlendCache.SetTexture("_RgbTex", LutTexture);
        materialBlendCache.SetTexture("_LerpRgbTex", volumesLutBlendTexture != null ? volumesLutBlendTexture : defaultLut);
        Graphics.Blit(midBlendLUT, midBlendLUT, materialBlendCache);
        blendCacheLut.DiscardContents();
        midBlendLUT.MarkRestoreExpected();
        currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(EffectFlags, currentVolumeEffects, blendVolumeEffects, effectVolumesBlendAdjusted);
        currentExposure = Mathf.Lerp(currentExposure, blendExposure, effectVolumesBlendAdjusted);
        effectVolumesBlendAdjust = 0.0f;
        blendingFromMidBlend = true;
      }
      VolumesBlendTo(blendTargetLUT, blendTimeInSec);
    }
    blendVolumeEffects = amplifyColorVolumeBase == null ? worldVolumeEffects : amplifyColorVolumeBase.EffectContainer.FindVolumeEffect(this);
    blendExposure = amplifyColorVolumeBase == null ? worldExposure : amplifyColorVolumeBase.Exposure;
    if (blendVolumeEffects == null)
      blendVolumeEffects = worldVolumeEffects;
  }

  private void SetupShader()
  {
    colorSpace = QualitySettings.activeColorSpace;
    qualityLevel = QualityLevel;
    shaderBase = Shader.Find("Hidden/Amplify Color/Base");
    shaderBlend = Shader.Find("Hidden/Amplify Color/Blend");
    shaderBlendCache = Shader.Find("Hidden/Amplify Color/BlendCache");
    shaderMask = Shader.Find("Hidden/Amplify Color/Mask");
    shaderMaskBlend = Shader.Find("Hidden/Amplify Color/MaskBlend");
    shaderDepthMask = Shader.Find("Hidden/Amplify Color/DepthMask");
    shaderDepthMaskBlend = Shader.Find("Hidden/Amplify Color/DepthMaskBlend");
    shaderProcessOnly = Shader.Find("Hidden/Amplify Color/ProcessOnly");
  }

  private void ReleaseMaterials()
  {
    SafeRelease(ref materialBase);
    SafeRelease(ref materialBlend);
    SafeRelease(ref materialBlendCache);
    SafeRelease(ref materialMask);
    SafeRelease(ref materialMaskBlend);
    SafeRelease(ref materialDepthMask);
    SafeRelease(ref materialDepthMaskBlend);
    SafeRelease(ref materialProcessOnly);
  }

  private Texture2D CreateDefaultLut()
  {
    Texture2D texture2D = new Texture2D(1024, 32, TextureFormat.RGB24, false, true);
    texture2D.hideFlags = HideFlags.HideAndDontSave;
    defaultLut = texture2D;
    defaultLut.name = "DefaultLut";
    defaultLut.hideFlags = HideFlags.DontSave;
    defaultLut.anisoLevel = 1;
    defaultLut.filterMode = FilterMode.Bilinear;
    Color32[] colors = new Color32[32768];
    for (int index1 = 0; index1 < 32; ++index1)
    {
      int num1 = index1 * 32;
      for (int index2 = 0; index2 < 32; ++index2)
      {
        int num2 = num1 + index2 * 1024;
        for (int index3 = 0; index3 < 32; ++index3)
        {
          float num3 = index3 / 31f;
          float num4 = index2 / 31f;
          float num5 = index1 / 31f;
          byte r = (byte) (num3 * (double) byte.MaxValue);
          byte g = (byte) (num4 * (double) byte.MaxValue);
          byte b = (byte) (num5 * (double) byte.MaxValue);
          colors[num2 + index3] = new Color32(r, g, b, byte.MaxValue);
        }
      }
    }
    defaultLut.SetPixels32(colors);
    defaultLut.Apply();
    return defaultLut;
  }

  private Texture2D CreateDepthCurveLut()
  {
    SafeRelease(ref depthCurveLut);
    Texture2D texture2D = new Texture2D(1024, 1, TextureFormat.Alpha8, false, true);
    texture2D.hideFlags = HideFlags.HideAndDontSave;
    depthCurveLut = texture2D;
    depthCurveLut.name = "DepthCurveLut";
    depthCurveLut.hideFlags = HideFlags.DontSave;
    depthCurveLut.anisoLevel = 1;
    depthCurveLut.wrapMode = TextureWrapMode.Clamp;
    depthCurveLut.filterMode = FilterMode.Bilinear;
    depthCurveColors = new Color32[1024];
    return depthCurveLut;
  }

  private void UpdateDepthCurveLut()
  {
    if (depthCurveLut == null)
      CreateDepthCurveLut();
    float time = 0.0f;
    int index = 0;
    while (index < 1024)
    {
      depthCurveColors[index].a = (byte) Mathf.FloorToInt(Mathf.Clamp01(DepthMaskCurve.Evaluate(time)) * byte.MaxValue);
      ++index;
      time += 0.0009775171f;
    }
    depthCurveLut.SetPixels32(depthCurveColors);
    depthCurveLut.Apply();
  }

  private void CheckUpdateDepthCurveLut()
  {
    bool flag = false;
    if (DepthMaskCurve.length != prevDepthMaskCurve.length)
    {
      flag = true;
    }
    else
    {
      float time = 0.0f;
      int num = 0;
      while (num < DepthMaskCurve.length)
      {
        if (Mathf.Abs(DepthMaskCurve.Evaluate(time) - prevDepthMaskCurve.Evaluate(time)) > 1.4012984643248171E-45)
        {
          flag = true;
          break;
        }
        ++num;
        time += 0.0009775171f;
      }
    }
    if (!(depthCurveLut == null | flag))
      return;
    UpdateDepthCurveLut();
    prevDepthMaskCurve = new AnimationCurve(DepthMaskCurve.keys);
  }

  private void CreateHelperTextures()
  {
    ReleaseTextures();
    RenderTexture renderTexture1 = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
    renderTexture1.hideFlags = HideFlags.HideAndDontSave;
    blendCacheLut = renderTexture1;
    blendCacheLut.name = "BlendCacheLut";
    blendCacheLut.wrapMode = TextureWrapMode.Clamp;
    blendCacheLut.useMipMap = false;
    blendCacheLut.anisoLevel = 0;
    blendCacheLut.Create();
    RenderTexture renderTexture2 = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
    renderTexture2.hideFlags = HideFlags.HideAndDontSave;
    midBlendLUT = renderTexture2;
    midBlendLUT.name = "MidBlendLut";
    midBlendLUT.wrapMode = TextureWrapMode.Clamp;
    midBlendLUT.useMipMap = false;
    midBlendLUT.anisoLevel = 0;
    midBlendLUT.Create();
    midBlendLUT.MarkRestoreExpected();
    CreateDefaultLut();
    if (!UseDepthMask)
      return;
    CreateDepthCurveLut();
  }

  private bool CheckMaterialAndShader(Material material, string name)
  {
    if (material == null || material.shader == null)
    {
      Debug.LogWarning("[AmplifyColor] Error creating " + name + " material. Effect disabled.");
      enabled = false;
    }
    else if (!material.shader.isSupported)
    {
      Debug.LogWarning("[AmplifyColor] " + name + " shader not supported on this platform. Effect disabled.");
      enabled = false;
    }
    else
      material.hideFlags = HideFlags.HideAndDontSave;
    return enabled;
  }

  private bool CreateMaterials()
  {
    SetupShader();
    if (!CheckShaders())
      return false;
    ReleaseMaterials();
    materialBase = new Material(shaderBase);
    materialBlend = new Material(shaderBlend);
    materialBlendCache = new Material(shaderBlendCache);
    materialMask = new Material(shaderMask);
    materialMaskBlend = new Material(shaderMaskBlend);
    materialDepthMask = new Material(shaderDepthMask);
    materialDepthMaskBlend = new Material(shaderDepthMaskBlend);
    materialProcessOnly = new Material(shaderProcessOnly);
    if (!CheckMaterialAndShader(materialBase, "BaseMaterial") || !CheckMaterialAndShader(materialBlend, "BlendMaterial") || !CheckMaterialAndShader(materialBlendCache, "BlendCacheMaterial") || !CheckMaterialAndShader(materialMask, "MaskMaterial") || !CheckMaterialAndShader(materialMaskBlend, "MaskBlendMaterial") || !CheckMaterialAndShader(materialDepthMask, "DepthMaskMaterial") || !CheckMaterialAndShader(materialDepthMaskBlend, "DepthMaskBlendMaterial") || !CheckMaterialAndShader(materialProcessOnly, "ProcessOnlyMaterial"))
      return false;
    CreateHelperTextures();
    return true;
  }

  private void SetMaterialKeyword(string keyword, bool state)
  {
    if (state)
      Shader.EnableKeyword(keyword);
    else
      Shader.DisableKeyword(keyword);
  }

  private void SafeRelease<T>(ref T obj) where T : Object
  {
    if (!(obj != null))
      return;
    if (obj.GetType() == typeof (RenderTexture))
      ((object) obj as RenderTexture).Release();
    DestroyImmediate(obj);
    obj = default (T);
  }

  private void ReleaseTextures()
  {
    RenderTexture.active = null;
    SafeRelease(ref blendCacheLut);
    SafeRelease(ref midBlendLUT);
    SafeRelease(ref defaultLut);
    SafeRelease(ref depthCurveLut);
  }

  public static bool ValidateLutDimensions(Texture lut)
  {
    bool flag = true;
    if (lut != null)
    {
      if (lut.width / lut.height != lut.height)
      {
        Debug.LogWarning("[AmplifyColor] Lut " + lut.name + " has invalid dimensions.");
        flag = false;
      }
      else if (lut.anisoLevel != 0)
        lut.anisoLevel = 0;
    }
    return flag;
  }

  private void UpdatePostEffectParams()
  {
    if (UseDepthMask)
      CheckUpdateDepthCurveLut();
    Exposure = Mathf.Max(Exposure, 0.0f);
  }

  private int ComputeShaderPass()
  {
    bool flag1 = QualityLevel == Quality.Mobile;
    bool flag2 = colorSpace == ColorSpace.Linear;
    bool allowHdr = ownerCamera.allowHDR;
    int num = flag1 ? 18 : 0;
    return !allowHdr ? num + (flag2 ? 1 : 0) : (int) (num + 2 + (flag2 ? 8 : 0) + (ApplyDithering ? 4 : 0) + Tonemapper);
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (silentError)
    {
      Graphics.Blit(source, destination);
    }
    else
    {
      BlendAmount = Mathf.Clamp01(BlendAmount);
      if (colorSpace != QualitySettings.activeColorSpace || qualityLevel != QualityLevel)
        CreateMaterials();
      UpdatePostEffectParams();
      bool flag1 = ValidateLutDimensions(LutTexture);
      bool flag2 = ValidateLutDimensions(LutBlendTexture);
      bool flag3 = LutTexture == null && LutBlendTexture == null && volumesLutBlendTexture == null;
      Texture source1 = LutTexture == null ? defaultLut : LutTexture;
      Texture lutBlendTexture = LutBlendTexture;
      int shaderPass = ComputeShaderPass();
      bool flag4 = BlendAmount != 0.0 || blending;
      bool flag5 = flag4 || flag4 && lutBlendTexture != null;
      bool flag6 = flag5;
      bool flag7 = ((!flag1 ? 1 : (!flag2 ? 1 : 0)) | (flag3 ? 1 : 0)) != 0;
      Material mat = !flag7 ? (!flag5 && !volumesBlending ? (!UseDepthMask ? (MaskTexture != null ? materialMask : materialBase) : materialDepthMask) : (!UseDepthMask ? (MaskTexture != null ? materialMaskBlend : materialBlend) : materialDepthMaskBlend)) : materialProcessOnly;
      mat.SetFloat("_Exposure", Exposure);
      mat.SetFloat("_ShoulderStrength", 0.22f);
      mat.SetFloat("_LinearStrength", 0.3f);
      mat.SetFloat("_LinearAngle", 0.1f);
      mat.SetFloat("_ToeStrength", 0.2f);
      mat.SetFloat("_ToeNumerator", 0.01f);
      mat.SetFloat("_ToeDenominator", 0.3f);
      mat.SetFloat("_LinearWhite", LinearWhitePoint);
      mat.SetFloat("_LerpAmount", BlendAmount);
      if (MaskTexture != null)
        mat.SetTexture("_MaskTex", MaskTexture);
      if (UseDepthMask)
        mat.SetTexture("_DepthCurveLut", depthCurveLut);
      if (!flag7)
      {
        if (volumesBlending)
        {
          volumesBlendAmount = Mathf.Clamp01(volumesBlendAmount);
          materialBlendCache.SetFloat("_LerpAmount", volumesBlendAmount);
          if (blendingFromMidBlend)
            materialBlendCache.SetTexture("_RgbTex", midBlendLUT);
          else
            materialBlendCache.SetTexture("_RgbTex", source1);
          materialBlendCache.SetTexture("_LerpRgbTex", volumesLutBlendTexture != null ? volumesLutBlendTexture : defaultLut);
          Graphics.Blit(source1, blendCacheLut, materialBlendCache);
        }
        if (flag6)
        {
          materialBlendCache.SetFloat("_LerpAmount", BlendAmount);
          RenderTexture renderTexture = null;
          if (volumesBlending)
          {
            renderTexture = RenderTexture.GetTemporary(blendCacheLut.width, blendCacheLut.height, blendCacheLut.depth, blendCacheLut.format, RenderTextureReadWrite.Linear);
            Graphics.Blit(blendCacheLut, renderTexture);
            materialBlendCache.SetTexture("_RgbTex", renderTexture);
          }
          else
            materialBlendCache.SetTexture("_RgbTex", source1);
          materialBlendCache.SetTexture("_LerpRgbTex", lutBlendTexture != null ? lutBlendTexture : defaultLut);
          Graphics.Blit(source1, blendCacheLut, materialBlendCache);
          if (renderTexture != null)
            RenderTexture.ReleaseTemporary(renderTexture);
          mat.SetTexture("_RgbBlendCacheTex", blendCacheLut);
        }
        else if (volumesBlending)
        {
          mat.SetTexture("_RgbBlendCacheTex", blendCacheLut);
        }
        else
        {
          if (source1 != null)
            mat.SetTexture("_RgbTex", source1);
          if (lutBlendTexture != null)
            mat.SetTexture("_LerpRgbTex", lutBlendTexture);
        }
      }
      Graphics.Blit(source, destination, mat, shaderPass);
      if (!flag6 && !volumesBlending)
        return;
      blendCacheLut.DiscardContents();
    }
  }
}
