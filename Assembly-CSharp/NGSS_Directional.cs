using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof (Light))]
[ExecuteInEditMode]
public class NGSS_Directional : MonoBehaviourInstance<NGSS_Directional>
{
  [Header("MAIN SETTINGS")]
  [Tooltip("If disabled, NGSS Directional shadows replacement will be removed from Graphics settings when OnDisable is called in this component.")]
  public bool NGSS_KEEP_ONDISABLE = true;
  [Tooltip("Useful if you want to fallback to hard shadows at runtime without having to disable the component.")]
  public bool NGSS_HARD_SHADOWS;
  [Header(" ")]
  [Tooltip("Used to test blocker search and early bail out algorithms.\nRecommended values: Mobile = 16, Consoles = 24, Desktop VR = 32, Desktop High = 64, Desktop Ultra 128")]
  [Range(1f, 128f)]
  public int NGSS_TEST_SAMPLERS = 16;
  [Tooltip("Used for the final filtering of shadows.\nRecommended values: Mobile = 16, Consoles = 24, Desktop VR = 32, Desktop High = 64, Desktop Ultra 128")]
  [Range(1f, 128f)]
  public int NGSS_FILTER_SAMPLERS = 48;
  [Header(" ")]
  [Tooltip("If zero = 100% banding.\nIf one = 100% noise.")]
  [Range(0.0f, 2f)]
  public float NGSS_NOISE = 1f;
  [Header(" ")]
  [Tooltip("Overall softness for all shadows.")]
  [Range(0.0f, 2f)]
  public float NGSS_SOFTNESS = 1f;
  [Header("PCSS")]
  [Tooltip("PCSS Requires inline sampling and SM3.5, only available in Unity 2017.\nIt provides Area Light like soft-shadows.\nDisable it if you are looking for PCF filtering (uniform soft-shadows) which runs with SM3.0.")]
  public bool NGSS_PCSS_ENABLED = true;
  [Tooltip("PCSS softness when shadows is close to caster.")]
  [Range(0.0f, 2f)]
  public float NGSS_PCSS_SOFTNESS_MIN = 1f;
  [Tooltip("PCSS softness when shadows is far from caster.")]
  [Range(0.0f, 2f)]
  public float NGSS_PCSS_SOFTNESS_MAX = 1f;
  [Header(" ")]
  [Header("GLOBAL SETTINGS")]
  [Tooltip("Enable it to let NGSS_Directional control global shadows settings through this component.\nDisable it if you want to manage shadows settings through Unity Quality & Graphics Settings panel.")]
  public bool GLOBAL_SETTINGS_OVERRIDE;
  [Tooltip("Shadows projection.\nRecommeded StableFit as it helps stabilizing shadows as camera moves.")]
  public ShadowProjection GLOBAL_SHADOWS_PROJECTION = ShadowProjection.StableFit;
  [Tooltip("Shadows resolution.\nLow = 1024, Med = 2048, High = 4096, Ultra = 8192, Mega = 16384.")]
  public ShadowsResolution GLOBAL_SHADOWS_RESOLUTION = ShadowsResolution.High;
  [Tooltip("Sets the maximum distance at wich shadows are visible from camera.\nThis option affects your shadow distance in Quality Settings.")]
  public float GLOBAL_SHADOWS_DISTANCE = 150f;
  [Range(1f, 4f)]
  [Tooltip("Number of cascades the shadowmap will have.\nThis value affects your cascade counts in Quality Settings.")]
  public int GLOBAL_CASCADES_COUNT = 4;
  [Range(0.01f, 0.25f)]
  [Tooltip("Used for the cascade stitching algorithm. Compute cascades splits distribution exponentially in a x*2^n form.\nThis value affects your cascade splits in Quality Settings.")]
  public float GLOBAL_CASCADES_SPLIT_VALUE = 0.1f;
  [Header(" ")]
  [Tooltip("Blends cascades at seams intersection.\nAdditional overhead required for this option.")]
  public bool NGSS_CASCADES_BLENDING = true;
  [Tooltip("Tweak this value to adjust the blending transition between cascades.")]
  [Range(0.0f, 2f)]
  public float NGSS_CASCADES_BLENDING_VALUE = 1f;
  [Range(0.0f, 1f)]
  [Tooltip("If one, softness across cascades will be matched using splits distribution, resulting in realistic soft-ness over distance.\nIf zero the softness distribution will be based on cascade index, resulting in blurrier shadows over distance thus less realistic.")]
  public float NGSS_CASCADES_SOFTNESS_NORMALIZATION = 1f;
  private bool isInitialized;
  private bool isGraphicSet;
  private Light _dirLight;

  private Light dirLight
  {
    get
    {
      if (_dirLight == null)
        _dirLight = GetComponent<Light>();
      return _dirLight;
    }
  }

  private void OnDisable()
  {
    isInitialized = false;
    if (NGSS_KEEP_ONDISABLE || !isGraphicSet)
      return;
    isGraphicSet = false;
    GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, Shader.Find("Hidden/Internal-ScreenSpaceShadows"));
    GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.UseBuiltin);
  }

  private void OnEnable()
  {
    if (IsNotSupported())
    {
      Debug.LogWarning("Unsupported graphics API, NGSS requires at least SM3.0 or higher and DX9 is not supported.", this);
      enabled = false;
    }
    else
      Init();
  }

  private void Init()
  {
    if (isInitialized)
      return;
    if (!isGraphicSet)
    {
      GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.UseCustom);
      GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, Shader.Find("Hidden/NGSS_Directional"));
      isGraphicSet = true;
    }
    isInitialized = true;
  }

  private bool IsNotSupported()
  {
    return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.PlayStationVita || SystemInfo.graphicsDeviceType == GraphicsDeviceType.N3DS;
  }

  private void Update()
  {
    if (NGSS_HARD_SHADOWS)
    {
      Shader.EnableKeyword("NGSS_HARD_SHADOWS_DIR");
      dirLight.shadows = LightShadows.Hard;
    }
    else
    {
      Shader.DisableKeyword("NGSS_HARD_SHADOWS_DIR");
      dirLight.shadows = LightShadows.Soft;
      Shader.SetGlobalFloat("NGSS_TEST_SAMPLERS", NGSS_TEST_SAMPLERS);
      Shader.SetGlobalFloat("NGSS_FILTER_SAMPLERS", NGSS_FILTER_SAMPLERS);
      double num1 = NGSS_SOFTNESS / (QualitySettings.shadowDistance * 0.6600000262260437);
      double num2;
      switch (QualitySettings.shadowCascades)
      {
        case 2:
          num2 = 1.5;
          break;
        case 4:
          num2 = 1.0;
          break;
        default:
          num2 = 0.25;
          break;
      }
      Shader.SetGlobalFloat("NGSS_GLOBAL_SOFTNESS", (float) (num1 * num2));
      Shader.SetGlobalFloat("NGSS_BANDING_TO_NOISE_RATIO_DIR", NGSS_NOISE);
      if (NGSS_PCSS_ENABLED)
      {
        float num3 = NGSS_PCSS_SOFTNESS_MIN * 0.1f;
        float num4 = NGSS_PCSS_SOFTNESS_MAX * 0.25f;
        Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MIN", num3 > (double) num4 ? num4 : num3);
        Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MAX", num4 < (double) num3 ? num3 : num4);
        Shader.EnableKeyword("NGSS_PCSS_FILTER_DIR");
      }
      else
        Shader.DisableKeyword("NGSS_PCSS_FILTER_DIR");
      GLOBAL_CASCADES_COUNT = GLOBAL_CASCADES_COUNT == 3 ? 4 : GLOBAL_CASCADES_COUNT;
      GLOBAL_SHADOWS_DISTANCE = Mathf.Clamp(GLOBAL_SHADOWS_DISTANCE, 0.0f, GLOBAL_SHADOWS_DISTANCE);
      if (GLOBAL_SETTINGS_OVERRIDE)
      {
        QualitySettings.shadowDistance = GLOBAL_SHADOWS_DISTANCE;
        QualitySettings.shadowProjection = GLOBAL_SHADOWS_PROJECTION;
        dirLight.shadowCustomResolution = (int) GLOBAL_SHADOWS_RESOLUTION;
        if (GLOBAL_CASCADES_COUNT > 1)
        {
          QualitySettings.shadowCascades = GLOBAL_CASCADES_COUNT;
          QualitySettings.shadowCascade4Split = new Vector3(GLOBAL_CASCADES_SPLIT_VALUE, GLOBAL_CASCADES_SPLIT_VALUE * 2f, (float) (GLOBAL_CASCADES_SPLIT_VALUE * 2.0 * 2.0));
          QualitySettings.shadowCascade2Split = GLOBAL_CASCADES_SPLIT_VALUE * 2f;
        }
      }
      if (GLOBAL_CASCADES_COUNT > 1)
      {
        Shader.SetGlobalFloat("NGSS_CASCADES_SOFTNESS_NORMALIZATION", NGSS_CASCADES_SOFTNESS_NORMALIZATION);
        Shader.SetGlobalFloat("NGSS_CASCADES_COUNT", QualitySettings.shadowCascades);
        Shader.SetGlobalVector("NGSS_CASCADES_SPLITS", QualitySettings.shadowCascades == 2 ? new Vector4(QualitySettings.shadowCascade2Split, 1f, 1f, 1f) : new Vector4(QualitySettings.shadowCascade4Split.x, QualitySettings.shadowCascade4Split.y, QualitySettings.shadowCascade4Split.z, 1f));
      }
      if (NGSS_CASCADES_BLENDING && GLOBAL_CASCADES_COUNT > 1)
      {
        Shader.EnableKeyword("NGSS_USE_CASCADE_BLENDING");
        Shader.SetGlobalFloat("NGSS_CASCADE_BLEND_DISTANCE", NGSS_CASCADES_BLENDING_VALUE * 0.125f);
      }
      else
        Shader.DisableKeyword("NGSS_USE_CASCADE_BLENDING");
    }
  }

  public enum ShadowsResolution
  {
    Low = 1024,
    Med = 2048,
    High = 4096,
    Ultra = 8192,
    Mega = 16384,
  }
}
