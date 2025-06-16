using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Cinematic/Tonemapping and Color Grading")]
  [ImageEffectAllowedInSceneView]
  public class TonemappingColorGrading : MonoBehaviour
  {
    [SerializeField]
    [SettingsGroup]
    private EyeAdaptationSettings m_EyeAdaptation = EyeAdaptationSettings.defaultSettings;
    [SerializeField]
    [SettingsGroup]
    private TonemappingSettings m_Tonemapping = TonemappingSettings.defaultSettings;
    [SerializeField]
    [SettingsGroup]
    private ColorGradingSettings m_ColorGrading = ColorGradingSettings.defaultSettings;
    [SerializeField]
    [SettingsGroup]
    private LUTSettings m_Lut = LUTSettings.defaultSettings;
    private Texture2D m_IdentityLut;
    private RenderTexture m_InternalLut;
    private Texture2D m_CurveTexture;
    private Texture2D m_TonemapperCurve;
    private float m_TonemapperCurveRange;
    [SerializeField]
    private Shader m_Shader;
    private Material m_Material;
    private bool m_Dirty = true;
    private bool m_TonemapperDirty = true;
    private RenderTexture m_SmallAdaptiveRt;
    private RenderTextureFormat m_AdaptiveRtFormat;

    public EyeAdaptationSettings eyeAdaptation
    {
      get => m_EyeAdaptation;
      set => m_EyeAdaptation = value;
    }

    public TonemappingSettings tonemapping
    {
      get => m_Tonemapping;
      set
      {
        m_Tonemapping = value;
        SetTonemapperDirty();
      }
    }

    public ColorGradingSettings colorGrading
    {
      get => m_ColorGrading;
      set
      {
        m_ColorGrading = value;
        SetDirty();
      }
    }

    public LUTSettings lut
    {
      get => m_Lut;
      set => m_Lut = value;
    }

    private Texture2D identityLut
    {
      get
      {
        if (m_IdentityLut == null || m_IdentityLut.height != lutSize)
        {
          DestroyImmediate(m_IdentityLut);
          m_IdentityLut = GenerateIdentityLut(lutSize);
        }
        return m_IdentityLut;
      }
    }

    private RenderTexture internalLutRt
    {
      get
      {
        if (m_InternalLut == null || !m_InternalLut.IsCreated() || m_InternalLut.height != lutSize)
        {
          DestroyImmediate(m_InternalLut);
          RenderTexture renderTexture = new RenderTexture(lutSize * lutSize, lutSize, 0, RenderTextureFormat.ARGB32);
          renderTexture.name = "Internal LUT";
          renderTexture.filterMode = FilterMode.Bilinear;
          renderTexture.anisoLevel = 0;
          renderTexture.hideFlags = HideFlags.DontSave;
          m_InternalLut = renderTexture;
        }
        return m_InternalLut;
      }
    }

    private Texture2D curveTexture
    {
      get
      {
        if (m_CurveTexture == null)
        {
          Texture2D texture2D = new Texture2D(256, 1, TextureFormat.ARGB32, false, true);
          texture2D.name = "Curve texture";
          texture2D.wrapMode = TextureWrapMode.Clamp;
          texture2D.filterMode = FilterMode.Bilinear;
          texture2D.anisoLevel = 0;
          texture2D.hideFlags = HideFlags.DontSave;
          m_CurveTexture = texture2D;
        }
        return m_CurveTexture;
      }
    }

    private Texture2D tonemapperCurve
    {
      get
      {
        if (m_TonemapperCurve == null)
        {
          TextureFormat textureFormat = TextureFormat.RGB24;
          if (SystemInfo.SupportsTextureFormat(TextureFormat.RFloat))
            textureFormat = TextureFormat.RFloat;
          else if (SystemInfo.SupportsTextureFormat(TextureFormat.RHalf))
            textureFormat = TextureFormat.RHalf;
          Texture2D texture2D = new Texture2D(256, 1, textureFormat, false, true);
          texture2D.name = "Tonemapper curve texture";
          texture2D.wrapMode = TextureWrapMode.Clamp;
          texture2D.filterMode = FilterMode.Bilinear;
          texture2D.anisoLevel = 0;
          texture2D.hideFlags = HideFlags.DontSave;
          m_TonemapperCurve = texture2D;
        }
        return m_TonemapperCurve;
      }
    }

    public Shader shader
    {
      get
      {
        if (m_Shader == null)
          m_Shader = Shader.Find("Hidden/TonemappingColorGrading");
        return m_Shader;
      }
    }

    public Material material
    {
      get
      {
        if (m_Material == null)
          m_Material = ImageEffectHelper.CheckShaderAndCreateMaterial(shader);
        return m_Material;
      }
    }

    public bool isGammaColorSpace => QualitySettings.activeColorSpace == ColorSpace.Gamma;

    public int lutSize => (int) colorGrading.precision;

    public bool validRenderTextureFormat { get; private set; }

    public bool validUserLutSize { get; private set; }

    public void SetDirty() => m_Dirty = true;

    public void SetTonemapperDirty() => m_TonemapperDirty = true;

    private void OnEnable()
    {
      if (!ImageEffectHelper.IsSupported(shader, false, true, this))
      {
        enabled = false;
      }
      else
      {
        SetDirty();
        SetTonemapperDirty();
      }
    }

    private void OnDisable()
    {
      if (m_Material != null)
        DestroyImmediate(m_Material);
      if (m_IdentityLut != null)
        DestroyImmediate(m_IdentityLut);
      if (m_InternalLut != null)
        DestroyImmediate(internalLutRt);
      if (m_SmallAdaptiveRt != null)
        DestroyImmediate(m_SmallAdaptiveRt);
      if (m_CurveTexture != null)
        DestroyImmediate(m_CurveTexture);
      if (m_TonemapperCurve != null)
        DestroyImmediate(m_TonemapperCurve);
      m_Material = null;
      m_IdentityLut = null;
      m_InternalLut = null;
      m_SmallAdaptiveRt = null;
      m_CurveTexture = null;
      m_TonemapperCurve = null;
    }

    private void OnValidate()
    {
      SetDirty();
      SetTonemapperDirty();
    }

    private static Texture2D GenerateIdentityLut(int dim)
    {
      Color[] colors = new Color[dim * dim * dim];
      float num = (float) (1.0 / (dim - 1.0));
      for (int index1 = 0; index1 < dim; ++index1)
      {
        for (int index2 = 0; index2 < dim; ++index2)
        {
          for (int index3 = 0; index3 < dim; ++index3)
            colors[index1 + index2 * dim + index3 * dim * dim] = new Color(index1 * num, Mathf.Abs(index3 * num), index2 * num, 1f);
        }
      }
      Texture2D texture2D = new Texture2D(dim * dim, dim, TextureFormat.RGB24, false, true);
      texture2D.name = "Identity LUT";
      texture2D.filterMode = FilterMode.Bilinear;
      texture2D.anisoLevel = 0;
      texture2D.hideFlags = HideFlags.DontSave;
      Texture2D identityLut = texture2D;
      identityLut.SetPixels(colors);
      identityLut.Apply();
      return identityLut;
    }

    private float StandardIlluminantY(float x)
    {
      return (float) (2.869999885559082 * x - 3.0 * x * x - 0.27509507536888123);
    }

    private Vector3 CIExyToLMS(float x, float y)
    {
      float num1 = 1f;
      float num2 = num1 * x / y;
      float num3 = num1 * (1f - x - y) / y;
      return new Vector3((float) (0.73280000686645508 * num2 + 0.42960000038146973 * num1 - 0.1624000072479248 * num3), (float) (-0.70359998941421509 * num2 + 1.6974999904632568 * num1 + 0.0060999998822808266 * num3), (float) (3.0 / 1000.0 * num2 + 0.01360000018030405 * num1 + 0.98339998722076416 * num3));
    }

    private Vector3 GetWhiteBalance()
    {
      float temperatureShift = colorGrading.basics.temperatureShift;
      float tint = colorGrading.basics.tint;
      float x = (float) (0.3127099871635437 - temperatureShift * (temperatureShift < 0.0 ? 0.10000000149011612 : 0.05000000074505806));
      float y = StandardIlluminantY(x) + tint * 0.05f;
      Vector3 vector3 = new Vector3(0.949237f, 1.03542f, 1.08728f);
      Vector3 lms = CIExyToLMS(x, y);
      return new Vector3(vector3.x / lms.x, vector3.y / lms.y, vector3.z / lms.z);
    }

    private static Color NormalizeColor(Color c)
    {
      float a = (float) ((c.r + (double) c.g + c.b) / 3.0);
      if (Mathf.Approximately(a, 0.0f))
        return new Color(1f, 1f, 1f, 1f);
      return new Color {
        r = c.r / a,
        g = c.g / a,
        b = c.b / a,
        a = 1f
      };
    }

    private void GenerateLiftGammaGain(out Color lift, out Color gamma, out Color gain)
    {
      Color color1 = NormalizeColor(colorGrading.colorWheels.shadows);
      Color color2 = NormalizeColor(colorGrading.colorWheels.midtones);
      Color color3 = NormalizeColor(colorGrading.colorWheels.highlights);
      float num1 = (float) ((color1.r + (double) color1.g + color1.b) / 3.0);
      float num2 = (float) ((color2.r + (double) color2.g + color2.b) / 3.0);
      float num3 = (float) ((color3.r + (double) color3.g + color3.b) / 3.0);
      float r1 = (float) ((color1.r - (double) num1) * 0.10000000149011612);
      float g1 = (float) ((color1.g - (double) num1) * 0.10000000149011612);
      float b1 = (float) ((color1.b - (double) num1) * 0.10000000149011612);
      float b2 = Mathf.Pow(2f, (float) ((color2.r - (double) num2) * 0.5));
      float b3 = Mathf.Pow(2f, (float) ((color2.g - (double) num2) * 0.5));
      float b4 = Mathf.Pow(2f, (float) ((color2.b - (double) num2) * 0.5));
      float r2 = Mathf.Pow(2f, (float) ((color3.r - (double) num3) * 0.5));
      float g2 = Mathf.Pow(2f, (float) ((color3.g - (double) num3) * 0.5));
      float b5 = Mathf.Pow(2f, (float) ((color3.b - (double) num3) * 0.5));
      float r3 = 1f / Mathf.Max(0.01f, b2);
      float g3 = 1f / Mathf.Max(0.01f, b3);
      float b6 = 1f / Mathf.Max(0.01f, b4);
      lift = new Color(r1, g1, b1);
      gamma = new Color(r3, g3, b6);
      gain = new Color(r2, g2, b5);
    }

    private void GenCurveTexture()
    {
      AnimationCurve master = colorGrading.curves.master;
      AnimationCurve red = colorGrading.curves.red;
      AnimationCurve green = colorGrading.curves.green;
      AnimationCurve blue = colorGrading.curves.blue;
      Color[] colors = new Color[256];
      for (float time = 0.0f; time <= 1.0; time += 0.003921569f)
      {
        float a = Mathf.Clamp(master.Evaluate(time), 0.0f, 1f);
        float r = Mathf.Clamp(red.Evaluate(time), 0.0f, 1f);
        float g = Mathf.Clamp(green.Evaluate(time), 0.0f, 1f);
        float b = Mathf.Clamp(blue.Evaluate(time), 0.0f, 1f);
        colors[(int) Mathf.Floor(time * byte.MaxValue)] = new Color(r, g, b, a);
      }
      curveTexture.SetPixels(colors);
      curveTexture.Apply();
    }

    private bool CheckUserLut()
    {
      validUserLutSize = lut.texture.height == (int) Mathf.Sqrt(lut.texture.width);
      return validUserLutSize;
    }

    private bool CheckSmallAdaptiveRt()
    {
      if (m_SmallAdaptiveRt != null)
        return false;
      m_AdaptiveRtFormat = RenderTextureFormat.ARGBHalf;
      if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf))
        m_AdaptiveRtFormat = RenderTextureFormat.RGHalf;
      m_SmallAdaptiveRt = new RenderTexture(1, 1, 0, m_AdaptiveRtFormat);
      m_SmallAdaptiveRt.hideFlags = HideFlags.DontSave;
      return true;
    }

    private void OnGUI()
    {
      if (Event.current.type != EventType.Repaint)
        return;
      int y = 0;
      if (m_InternalLut != null && colorGrading.enabled && colorGrading.showDebug)
      {
        Graphics.DrawTexture(new Rect(0.0f, y, lutSize * lutSize, lutSize), internalLutRt);
        y += lutSize;
      }
      if (!(m_SmallAdaptiveRt != null) || !eyeAdaptation.enabled || !eyeAdaptation.showDebug)
        return;
      m_Material.SetPass(12);
      Graphics.DrawTexture(new Rect(0.0f, y, 256f, 16f), m_SmallAdaptiveRt, m_Material);
    }

    [ImageEffectTransformsToLDR]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      material.shaderKeywords = null;
      RenderTexture renderTexture = null;
      RenderTexture[] renderTextureArray = null;
      if (eyeAdaptation.enabled)
      {
        bool flag = CheckSmallAdaptiveRt();
        int num1 = source.width < source.height ? source.width : source.height;
        int num2 = num1 | num1 >> 1;
        int num3 = num2 | num2 >> 2;
        int num4 = num3 | num3 >> 4;
        int num5 = num4 | num4 >> 8;
        int num6 = num5 | num5 >> 16;
        int num7 = num6 - (num6 >> 1);
        renderTexture = RenderTexture.GetTemporary(num7, num7, 0, m_AdaptiveRtFormat);
        Graphics.Blit(source, renderTexture);
        int length = (int) Mathf.Log(renderTexture.width, 2f);
        int num8 = 2;
        renderTextureArray = new RenderTexture[length];
        for (int index = 0; index < length; ++index)
        {
          renderTextureArray[index] = RenderTexture.GetTemporary(renderTexture.width / num8, renderTexture.width / num8, 0, m_AdaptiveRtFormat);
          num8 <<= 1;
        }
        RenderTexture source1 = renderTextureArray[length - 1];
        Graphics.Blit(renderTexture, renderTextureArray[0], material, 1);
        for (int index = 0; index < length - 1; ++index)
        {
          Graphics.Blit(renderTextureArray[index], renderTextureArray[index + 1]);
          source1 = renderTextureArray[index + 1];
        }
        m_SmallAdaptiveRt.MarkRestoreExpected();
        material.SetFloat("_AdaptationSpeed", Mathf.Max(eyeAdaptation.speed, 1f / 1000f));
        Graphics.Blit(source1, m_SmallAdaptiveRt, material, flag ? 3 : 2);
        material.SetFloat("_MiddleGrey", eyeAdaptation.middleGrey);
        material.SetFloat("_AdaptationMin", Mathf.Pow(2f, eyeAdaptation.min));
        material.SetFloat("_AdaptationMax", Mathf.Pow(2f, eyeAdaptation.max));
        material.SetTexture("_LumTex", m_SmallAdaptiveRt);
        material.EnableKeyword("ENABLE_EYE_ADAPTATION");
      }
      int pass = 4;
      if (tonemapping.enabled)
      {
        if (tonemapping.tonemapper == Tonemapper.Curve)
        {
          if (m_TonemapperDirty)
          {
            float num9 = 1f;
            if (tonemapping.curve.length > 0)
            {
              num9 = tonemapping.curve[tonemapping.curve.length - 1].time;
              for (float num10 = 0.0f; num10 <= 1.0; num10 += 0.003921569f)
              {
                float num11 = tonemapping.curve.Evaluate(num10 * num9);
                tonemapperCurve.SetPixel(Mathf.FloorToInt(num10 * byte.MaxValue), 0, new Color(num11, num11, num11));
              }
              tonemapperCurve.Apply();
            }
            m_TonemapperCurveRange = 1f / num9;
            m_TonemapperDirty = false;
          }
          material.SetFloat("_ToneCurveRange", m_TonemapperCurveRange);
          material.SetTexture("_ToneCurve", tonemapperCurve);
        }
        else if (tonemapping.tonemapper == Tonemapper.Neutral)
        {
          float num12 = (float) (tonemapping.neutralBlackIn * 20.0 + 1.0);
          float num13 = (float) (tonemapping.neutralBlackOut * 10.0 + 1.0);
          float num14 = tonemapping.neutralWhiteIn / 20f;
          float num15 = (float) (1.0 - tonemapping.neutralWhiteOut / 20.0);
          float t1 = num12 / num13;
          float t2 = num14 / num15;
          material.SetVector("_NeutralTonemapperParams1", new Vector4(0.2f, Mathf.Max(0.0f, Mathf.LerpUnclamped(0.57f, 0.37f, t1)), Mathf.LerpUnclamped(0.01f, 0.24f, t2), Mathf.Max(0.0f, Mathf.LerpUnclamped(0.02f, 0.2f, t1))));
          material.SetVector("_NeutralTonemapperParams2", new Vector4(0.02f, 0.3f, tonemapping.neutralWhiteLevel, tonemapping.neutralWhiteClip / 10f));
        }
        material.SetFloat("_Exposure", tonemapping.exposure);
        pass += (int) (tonemapping.tonemapper + 1);
      }
      if (colorGrading.enabled)
      {
        if (m_Dirty || !m_InternalLut.IsCreated())
        {
          Color lift;
          Color gamma;
          Color gain;
          GenerateLiftGammaGain(out lift, out gamma, out gain);
          GenCurveTexture();
          material.SetVector("_WhiteBalance", GetWhiteBalance());
          material.SetVector("_Lift", lift);
          material.SetVector("_Gamma", gamma);
          material.SetVector("_Gain", gain);
          material.SetVector("_ContrastGainGamma", new Vector3(colorGrading.basics.contrast, colorGrading.basics.gain, 1f / colorGrading.basics.gamma));
          material.SetFloat("_Vibrance", colorGrading.basics.vibrance);
          material.SetVector("_HSV", new Vector4(colorGrading.basics.hue, colorGrading.basics.saturation, colorGrading.basics.value));
          material.SetVector("_ChannelMixerRed", colorGrading.channelMixer.channels[0]);
          material.SetVector("_ChannelMixerGreen", colorGrading.channelMixer.channels[1]);
          material.SetVector("_ChannelMixerBlue", colorGrading.channelMixer.channels[2]);
          material.SetTexture("_CurveTex", curveTexture);
          internalLutRt.MarkRestoreExpected();
          Graphics.Blit(identityLut, internalLutRt, material, 0);
          m_Dirty = false;
        }
        material.EnableKeyword("ENABLE_COLOR_GRADING");
        if (colorGrading.useDithering)
          material.EnableKeyword("ENABLE_DITHERING");
        material.SetTexture("_InternalLutTex", internalLutRt);
        material.SetVector("_InternalLutParams", new Vector3(1f / internalLutRt.width, 1f / internalLutRt.height, internalLutRt.height - 1f));
      }
      if (lut.enabled && lut.texture != null && CheckUserLut())
      {
        material.SetTexture("_UserLutTex", lut.texture);
        material.SetVector("_UserLutParams", new Vector4(1f / lut.texture.width, 1f / lut.texture.height, lut.texture.height - 1f, lut.contribution));
        material.EnableKeyword("ENABLE_USER_LUT");
      }
      Graphics.Blit(source, destination, material, pass);
      if (!eyeAdaptation.enabled)
        return;
      for (int index = 0; index < renderTextureArray.Length; ++index)
        RenderTexture.ReleaseTemporary(renderTextureArray[index]);
      RenderTexture.ReleaseTemporary(renderTexture);
    }

    public Texture2D BakeLUT()
    {
      Texture2D texture2D = new Texture2D(internalLutRt.width, internalLutRt.height, TextureFormat.RGB24, false, true);
      RenderTexture.active = internalLutRt;
      texture2D.ReadPixels(new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), 0, 0);
      RenderTexture.active = null;
      return texture2D;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingsGroup : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class IndentedGroup : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ChannelMixer : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ColorWheelGroup : PropertyAttribute
    {
      public int minSizePerWheel = 60;
      public int maxSizePerWheel = 150;

      public ColorWheelGroup()
      {
      }

      public ColorWheelGroup(int minSizePerWheel, int maxSizePerWheel)
      {
        this.minSizePerWheel = minSizePerWheel;
        this.maxSizePerWheel = maxSizePerWheel;
      }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class Curve : PropertyAttribute
    {
      public Color color = Color.white;

      public Curve()
      {
      }

      public Curve(float r, float g, float b, float a) => color = new Color(r, g, b, a);
    }

    [Serializable]
    public struct EyeAdaptationSettings
    {
      public bool enabled;
      [Min(0.0f)]
      [Tooltip("Midpoint Adjustment.")]
      public float middleGrey;
      [Tooltip("The lowest possible exposure value; adjust this value to modify the brightest areas of your level.")]
      public float min;
      [Tooltip("The highest possible exposure value; adjust this value to modify the darkest areas of your level.")]
      public float max;
      [Min(0.0f)]
      [Tooltip("Speed of linear adaptation. Higher is faster.")]
      public float speed;
      [Tooltip("Displays a luminosity helper in the GameView.")]
      public bool showDebug;

      public static EyeAdaptationSettings defaultSettings
      {
        get
        {
          return new EyeAdaptationSettings {
            enabled = false,
            showDebug = false,
            middleGrey = 0.5f,
            min = -3f,
            max = 3f,
            speed = 1.5f
          };
        }
      }
    }

    public enum Tonemapper
    {
      ACES,
      Curve,
      Hable,
      HejlDawson,
      Photographic,
      Reinhard,
      Neutral,
    }

    [Serializable]
    public struct TonemappingSettings
    {
      public bool enabled;
      [Tooltip("Tonemapping technique to use. ACES is the recommended one.")]
      public Tonemapper tonemapper;
      [Min(0.0f)]
      [Tooltip("Adjusts the overall exposure of the scene.")]
      public float exposure;
      [Tooltip("Custom tonemapping curve.")]
      public AnimationCurve curve;
      [Range(-0.1f, 0.1f)]
      public float neutralBlackIn;
      [Range(1f, 20f)]
      public float neutralWhiteIn;
      [Range(-0.09f, 0.1f)]
      public float neutralBlackOut;
      [Range(1f, 19f)]
      public float neutralWhiteOut;
      [Range(0.1f, 20f)]
      public float neutralWhiteLevel;
      [Range(1f, 10f)]
      public float neutralWhiteClip;

      public static TonemappingSettings defaultSettings
      {
        get
        {
          return new TonemappingSettings {
            enabled = false,
            tonemapper = Tonemapper.Neutral,
            exposure = 1f,
            curve = CurvesSettings.defaultCurve,
            neutralBlackIn = 0.02f,
            neutralWhiteIn = 10f,
            neutralBlackOut = 0.0f,
            neutralWhiteOut = 10f,
            neutralWhiteLevel = 5.3f,
            neutralWhiteClip = 10f
          };
        }
      }
    }

    [Serializable]
    public struct LUTSettings
    {
      public bool enabled;
      [Tooltip("Custom lookup texture (strip format, e.g. 256x16).")]
      public Texture texture;
      [Range(0.0f, 1f)]
      [Tooltip("Blending factor.")]
      public float contribution;

      public static LUTSettings defaultSettings
      {
        get
        {
          return new LUTSettings {
            enabled = false,
            texture = null,
            contribution = 1f
          };
        }
      }
    }

    [Serializable]
    public struct ColorWheelsSettings
    {
      [ColorUsage(false)]
      public Color shadows;
      [ColorUsage(false)]
      public Color midtones;
      [ColorUsage(false)]
      public Color highlights;

      public static ColorWheelsSettings defaultSettings
      {
        get
        {
          return new ColorWheelsSettings {
            shadows = Color.white,
            midtones = Color.white,
            highlights = Color.white
          };
        }
      }
    }

    [Serializable]
    public struct BasicsSettings
    {
      [Range(-2f, 2f)]
      [Tooltip("Sets the white balance to a custom color temperature.")]
      public float temperatureShift;
      [Range(-2f, 2f)]
      [Tooltip("Sets the white balance to compensate for a green or magenta tint.")]
      public float tint;
      [Space]
      [Range(-0.5f, 0.5f)]
      [Tooltip("Shift the hue of all colors.")]
      public float hue;
      [Range(0.0f, 2f)]
      [Tooltip("Pushes the intensity of all colors.")]
      public float saturation;
      [Range(-1f, 1f)]
      [Tooltip("Adjusts the saturation so that clipping is minimized as colors approach full saturation.")]
      public float vibrance;
      [Range(0.0f, 10f)]
      [Tooltip("Brightens or darkens all colors.")]
      public float value;
      [Space]
      [Range(0.0f, 2f)]
      [Tooltip("Expands or shrinks the overall range of tonal values.")]
      public float contrast;
      [Range(0.01f, 5f)]
      [Tooltip("Contrast gain curve. Controls the steepness of the curve.")]
      public float gain;
      [Range(0.01f, 5f)]
      [Tooltip("Applies a pow function to the source.")]
      public float gamma;

      public static BasicsSettings defaultSettings
      {
        get
        {
          return new BasicsSettings {
            temperatureShift = 0.0f,
            tint = 0.0f,
            contrast = 1f,
            hue = 0.0f,
            saturation = 1f,
            value = 1f,
            vibrance = 0.0f,
            gain = 1f,
            gamma = 1f
          };
        }
      }
    }

    [Serializable]
    public struct ChannelMixerSettings
    {
      public int currentChannel;
      public Vector3[] channels;

      public static ChannelMixerSettings defaultSettings
      {
        get
        {
          return new ChannelMixerSettings {
            currentChannel = 0,
            channels = new Vector3[3]
            {
              new Vector3(1f, 0.0f, 0.0f),
              new Vector3(0.0f, 1f, 0.0f),
              new Vector3(0.0f, 0.0f, 1f)
            }
          };
        }
      }
    }

    [Serializable]
    public struct CurvesSettings
    {
      [Curve]
      public AnimationCurve master;
      [Curve(1f, 0.0f, 0.0f, 1f)]
      public AnimationCurve red;
      [Curve(0.0f, 1f, 0.0f, 1f)]
      public AnimationCurve green;
      [Curve(0.0f, 1f, 1f, 1f)]
      public AnimationCurve blue;

      public static CurvesSettings defaultSettings
      {
        get
        {
          return new CurvesSettings {
            master = defaultCurve,
            red = defaultCurve,
            green = defaultCurve,
            blue = defaultCurve
          };
        }
      }

      public static AnimationCurve defaultCurve
      {
        get
        {
          return new AnimationCurve(new Keyframe(0.0f, 0.0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));
        }
      }
    }

    public enum ColorGradingPrecision
    {
      Normal = 16,
      High = 32,
    }

    [Serializable]
    public struct ColorGradingSettings
    {
      public bool enabled;
      [Tooltip("Internal LUT precision. \"Normal\" is 256x16, \"High\" is 1024x32. Prefer \"Normal\" on mobile devices.")]
      public ColorGradingPrecision precision;
      [Space]
      [ColorWheelGroup]
      public ColorWheelsSettings colorWheels;
      [Space]
      [IndentedGroup]
      public BasicsSettings basics;
      [Space]
      [ChannelMixer]
      public ChannelMixerSettings channelMixer;
      [Space]
      [IndentedGroup]
      public CurvesSettings curves;
      [Space]
      [Tooltip("Use dithering to try and minimize color banding in dark areas.")]
      public bool useDithering;
      [Tooltip("Displays the generated LUT in the top left corner of the GameView.")]
      public bool showDebug;

      public static ColorGradingSettings defaultSettings
      {
        get
        {
          return new ColorGradingSettings {
            enabled = false,
            useDithering = false,
            showDebug = false,
            precision = ColorGradingPrecision.Normal,
            colorWheels = ColorWheelsSettings.defaultSettings,
            basics = BasicsSettings.defaultSettings,
            channelMixer = ChannelMixerSettings.defaultSettings,
            curves = CurvesSettings.defaultSettings
          };
        }
      }

      internal void Reset() => curves = CurvesSettings.defaultSettings;
    }

    private enum Pass
    {
      LutGen,
      AdaptationLog,
      AdaptationExpBlend,
      AdaptationExp,
      TonemappingOff,
      TonemappingACES,
      TonemappingCurve,
      TonemappingHable,
      TonemappingHejlDawson,
      TonemappingPhotographic,
      TonemappingReinhard,
      TonemappingNeutral,
      AdaptationDebug,
    }
  }
}
