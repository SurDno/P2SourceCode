// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CinematicEffects.TonemappingColorGrading
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Cinematic/Tonemapping and Color Grading")]
  [ImageEffectAllowedInSceneView]
  public class TonemappingColorGrading : MonoBehaviour
  {
    [SerializeField]
    [TonemappingColorGrading.SettingsGroup]
    private TonemappingColorGrading.EyeAdaptationSettings m_EyeAdaptation = TonemappingColorGrading.EyeAdaptationSettings.defaultSettings;
    [SerializeField]
    [TonemappingColorGrading.SettingsGroup]
    private TonemappingColorGrading.TonemappingSettings m_Tonemapping = TonemappingColorGrading.TonemappingSettings.defaultSettings;
    [SerializeField]
    [TonemappingColorGrading.SettingsGroup]
    private TonemappingColorGrading.ColorGradingSettings m_ColorGrading = TonemappingColorGrading.ColorGradingSettings.defaultSettings;
    [SerializeField]
    [TonemappingColorGrading.SettingsGroup]
    private TonemappingColorGrading.LUTSettings m_Lut = TonemappingColorGrading.LUTSettings.defaultSettings;
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

    public TonemappingColorGrading.EyeAdaptationSettings eyeAdaptation
    {
      get => this.m_EyeAdaptation;
      set => this.m_EyeAdaptation = value;
    }

    public TonemappingColorGrading.TonemappingSettings tonemapping
    {
      get => this.m_Tonemapping;
      set
      {
        this.m_Tonemapping = value;
        this.SetTonemapperDirty();
      }
    }

    public TonemappingColorGrading.ColorGradingSettings colorGrading
    {
      get => this.m_ColorGrading;
      set
      {
        this.m_ColorGrading = value;
        this.SetDirty();
      }
    }

    public TonemappingColorGrading.LUTSettings lut
    {
      get => this.m_Lut;
      set => this.m_Lut = value;
    }

    private Texture2D identityLut
    {
      get
      {
        if ((UnityEngine.Object) this.m_IdentityLut == (UnityEngine.Object) null || this.m_IdentityLut.height != this.lutSize)
        {
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_IdentityLut);
          this.m_IdentityLut = TonemappingColorGrading.GenerateIdentityLut(this.lutSize);
        }
        return this.m_IdentityLut;
      }
    }

    private RenderTexture internalLutRt
    {
      get
      {
        if ((UnityEngine.Object) this.m_InternalLut == (UnityEngine.Object) null || !this.m_InternalLut.IsCreated() || this.m_InternalLut.height != this.lutSize)
        {
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_InternalLut);
          RenderTexture renderTexture = new RenderTexture(this.lutSize * this.lutSize, this.lutSize, 0, RenderTextureFormat.ARGB32);
          renderTexture.name = "Internal LUT";
          renderTexture.filterMode = FilterMode.Bilinear;
          renderTexture.anisoLevel = 0;
          renderTexture.hideFlags = HideFlags.DontSave;
          this.m_InternalLut = renderTexture;
        }
        return this.m_InternalLut;
      }
    }

    private Texture2D curveTexture
    {
      get
      {
        if ((UnityEngine.Object) this.m_CurveTexture == (UnityEngine.Object) null)
        {
          Texture2D texture2D = new Texture2D(256, 1, TextureFormat.ARGB32, false, true);
          texture2D.name = "Curve texture";
          texture2D.wrapMode = TextureWrapMode.Clamp;
          texture2D.filterMode = FilterMode.Bilinear;
          texture2D.anisoLevel = 0;
          texture2D.hideFlags = HideFlags.DontSave;
          this.m_CurveTexture = texture2D;
        }
        return this.m_CurveTexture;
      }
    }

    private Texture2D tonemapperCurve
    {
      get
      {
        if ((UnityEngine.Object) this.m_TonemapperCurve == (UnityEngine.Object) null)
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
          this.m_TonemapperCurve = texture2D;
        }
        return this.m_TonemapperCurve;
      }
    }

    public Shader shader
    {
      get
      {
        if ((UnityEngine.Object) this.m_Shader == (UnityEngine.Object) null)
          this.m_Shader = Shader.Find("Hidden/TonemappingColorGrading");
        return this.m_Shader;
      }
    }

    public Material material
    {
      get
      {
        if ((UnityEngine.Object) this.m_Material == (UnityEngine.Object) null)
          this.m_Material = ImageEffectHelper.CheckShaderAndCreateMaterial(this.shader);
        return this.m_Material;
      }
    }

    public bool isGammaColorSpace => UnityEngine.QualitySettings.activeColorSpace == ColorSpace.Gamma;

    public int lutSize => (int) this.colorGrading.precision;

    public bool validRenderTextureFormat { get; private set; }

    public bool validUserLutSize { get; private set; }

    public void SetDirty() => this.m_Dirty = true;

    public void SetTonemapperDirty() => this.m_TonemapperDirty = true;

    private void OnEnable()
    {
      if (!ImageEffectHelper.IsSupported(this.shader, false, true, (MonoBehaviour) this))
      {
        this.enabled = false;
      }
      else
      {
        this.SetDirty();
        this.SetTonemapperDirty();
      }
    }

    private void OnDisable()
    {
      if ((UnityEngine.Object) this.m_Material != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_Material);
      if ((UnityEngine.Object) this.m_IdentityLut != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_IdentityLut);
      if ((UnityEngine.Object) this.m_InternalLut != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.internalLutRt);
      if ((UnityEngine.Object) this.m_SmallAdaptiveRt != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_SmallAdaptiveRt);
      if ((UnityEngine.Object) this.m_CurveTexture != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_CurveTexture);
      if ((UnityEngine.Object) this.m_TonemapperCurve != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_TonemapperCurve);
      this.m_Material = (Material) null;
      this.m_IdentityLut = (Texture2D) null;
      this.m_InternalLut = (RenderTexture) null;
      this.m_SmallAdaptiveRt = (RenderTexture) null;
      this.m_CurveTexture = (Texture2D) null;
      this.m_TonemapperCurve = (Texture2D) null;
    }

    private void OnValidate()
    {
      this.SetDirty();
      this.SetTonemapperDirty();
    }

    private static Texture2D GenerateIdentityLut(int dim)
    {
      Color[] colors = new Color[dim * dim * dim];
      float num = (float) (1.0 / ((double) dim - 1.0));
      for (int index1 = 0; index1 < dim; ++index1)
      {
        for (int index2 = 0; index2 < dim; ++index2)
        {
          for (int index3 = 0; index3 < dim; ++index3)
            colors[index1 + index2 * dim + index3 * dim * dim] = new Color((float) index1 * num, Mathf.Abs((float) index3 * num), (float) index2 * num, 1f);
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
      return (float) (2.869999885559082 * (double) x - 3.0 * (double) x * (double) x - 0.27509507536888123);
    }

    private Vector3 CIExyToLMS(float x, float y)
    {
      float num1 = 1f;
      float num2 = num1 * x / y;
      float num3 = num1 * (1f - x - y) / y;
      return new Vector3((float) (0.73280000686645508 * (double) num2 + 0.42960000038146973 * (double) num1 - 0.1624000072479248 * (double) num3), (float) (-0.70359998941421509 * (double) num2 + 1.6974999904632568 * (double) num1 + 0.0060999998822808266 * (double) num3), (float) (3.0 / 1000.0 * (double) num2 + 0.01360000018030405 * (double) num1 + 0.98339998722076416 * (double) num3));
    }

    private Vector3 GetWhiteBalance()
    {
      float temperatureShift = this.colorGrading.basics.temperatureShift;
      float tint = this.colorGrading.basics.tint;
      float x = (float) (0.3127099871635437 - (double) temperatureShift * ((double) temperatureShift < 0.0 ? 0.10000000149011612 : 0.05000000074505806));
      float y = this.StandardIlluminantY(x) + tint * 0.05f;
      Vector3 vector3 = new Vector3(0.949237f, 1.03542f, 1.08728f);
      Vector3 lms = this.CIExyToLMS(x, y);
      return new Vector3(vector3.x / lms.x, vector3.y / lms.y, vector3.z / lms.z);
    }

    private static Color NormalizeColor(Color c)
    {
      float a = (float) (((double) c.r + (double) c.g + (double) c.b) / 3.0);
      if (Mathf.Approximately(a, 0.0f))
        return new Color(1f, 1f, 1f, 1f);
      return new Color()
      {
        r = c.r / a,
        g = c.g / a,
        b = c.b / a,
        a = 1f
      };
    }

    private void GenerateLiftGammaGain(out Color lift, out Color gamma, out Color gain)
    {
      Color color1 = TonemappingColorGrading.NormalizeColor(this.colorGrading.colorWheels.shadows);
      Color color2 = TonemappingColorGrading.NormalizeColor(this.colorGrading.colorWheels.midtones);
      Color color3 = TonemappingColorGrading.NormalizeColor(this.colorGrading.colorWheels.highlights);
      float num1 = (float) (((double) color1.r + (double) color1.g + (double) color1.b) / 3.0);
      float num2 = (float) (((double) color2.r + (double) color2.g + (double) color2.b) / 3.0);
      float num3 = (float) (((double) color3.r + (double) color3.g + (double) color3.b) / 3.0);
      float r1 = (float) (((double) color1.r - (double) num1) * 0.10000000149011612);
      float g1 = (float) (((double) color1.g - (double) num1) * 0.10000000149011612);
      float b1 = (float) (((double) color1.b - (double) num1) * 0.10000000149011612);
      float b2 = Mathf.Pow(2f, (float) (((double) color2.r - (double) num2) * 0.5));
      float b3 = Mathf.Pow(2f, (float) (((double) color2.g - (double) num2) * 0.5));
      float b4 = Mathf.Pow(2f, (float) (((double) color2.b - (double) num2) * 0.5));
      float r2 = Mathf.Pow(2f, (float) (((double) color3.r - (double) num3) * 0.5));
      float g2 = Mathf.Pow(2f, (float) (((double) color3.g - (double) num3) * 0.5));
      float b5 = Mathf.Pow(2f, (float) (((double) color3.b - (double) num3) * 0.5));
      float r3 = 1f / Mathf.Max(0.01f, b2);
      float g3 = 1f / Mathf.Max(0.01f, b3);
      float b6 = 1f / Mathf.Max(0.01f, b4);
      lift = new Color(r1, g1, b1);
      gamma = new Color(r3, g3, b6);
      gain = new Color(r2, g2, b5);
    }

    private void GenCurveTexture()
    {
      AnimationCurve master = this.colorGrading.curves.master;
      AnimationCurve red = this.colorGrading.curves.red;
      AnimationCurve green = this.colorGrading.curves.green;
      AnimationCurve blue = this.colorGrading.curves.blue;
      Color[] colors = new Color[256];
      for (float time = 0.0f; (double) time <= 1.0; time += 0.003921569f)
      {
        float a = Mathf.Clamp(master.Evaluate(time), 0.0f, 1f);
        float r = Mathf.Clamp(red.Evaluate(time), 0.0f, 1f);
        float g = Mathf.Clamp(green.Evaluate(time), 0.0f, 1f);
        float b = Mathf.Clamp(blue.Evaluate(time), 0.0f, 1f);
        colors[(int) Mathf.Floor(time * (float) byte.MaxValue)] = new Color(r, g, b, a);
      }
      this.curveTexture.SetPixels(colors);
      this.curveTexture.Apply();
    }

    private bool CheckUserLut()
    {
      this.validUserLutSize = this.lut.texture.height == (int) Mathf.Sqrt((float) this.lut.texture.width);
      return this.validUserLutSize;
    }

    private bool CheckSmallAdaptiveRt()
    {
      if ((UnityEngine.Object) this.m_SmallAdaptiveRt != (UnityEngine.Object) null)
        return false;
      this.m_AdaptiveRtFormat = RenderTextureFormat.ARGBHalf;
      if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf))
        this.m_AdaptiveRtFormat = RenderTextureFormat.RGHalf;
      this.m_SmallAdaptiveRt = new RenderTexture(1, 1, 0, this.m_AdaptiveRtFormat);
      this.m_SmallAdaptiveRt.hideFlags = HideFlags.DontSave;
      return true;
    }

    private void OnGUI()
    {
      if (Event.current.type != EventType.Repaint)
        return;
      int y = 0;
      if ((UnityEngine.Object) this.m_InternalLut != (UnityEngine.Object) null && this.colorGrading.enabled && this.colorGrading.showDebug)
      {
        Graphics.DrawTexture(new Rect(0.0f, (float) y, (float) (this.lutSize * this.lutSize), (float) this.lutSize), (Texture) this.internalLutRt);
        y += this.lutSize;
      }
      if (!((UnityEngine.Object) this.m_SmallAdaptiveRt != (UnityEngine.Object) null) || !this.eyeAdaptation.enabled || !this.eyeAdaptation.showDebug)
        return;
      this.m_Material.SetPass(12);
      Graphics.DrawTexture(new Rect(0.0f, (float) y, 256f, 16f), (Texture) this.m_SmallAdaptiveRt, this.m_Material);
    }

    [ImageEffectTransformsToLDR]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      this.material.shaderKeywords = (string[]) null;
      RenderTexture renderTexture = (RenderTexture) null;
      RenderTexture[] renderTextureArray = (RenderTexture[]) null;
      if (this.eyeAdaptation.enabled)
      {
        bool flag = this.CheckSmallAdaptiveRt();
        int num1 = source.width < source.height ? source.width : source.height;
        int num2 = num1 | num1 >> 1;
        int num3 = num2 | num2 >> 2;
        int num4 = num3 | num3 >> 4;
        int num5 = num4 | num4 >> 8;
        int num6 = num5 | num5 >> 16;
        int num7 = num6 - (num6 >> 1);
        renderTexture = RenderTexture.GetTemporary(num7, num7, 0, this.m_AdaptiveRtFormat);
        Graphics.Blit((Texture) source, renderTexture);
        int length = (int) Mathf.Log((float) renderTexture.width, 2f);
        int num8 = 2;
        renderTextureArray = new RenderTexture[length];
        for (int index = 0; index < length; ++index)
        {
          renderTextureArray[index] = RenderTexture.GetTemporary(renderTexture.width / num8, renderTexture.width / num8, 0, this.m_AdaptiveRtFormat);
          num8 <<= 1;
        }
        RenderTexture source1 = renderTextureArray[length - 1];
        Graphics.Blit((Texture) renderTexture, renderTextureArray[0], this.material, 1);
        for (int index = 0; index < length - 1; ++index)
        {
          Graphics.Blit((Texture) renderTextureArray[index], renderTextureArray[index + 1]);
          source1 = renderTextureArray[index + 1];
        }
        this.m_SmallAdaptiveRt.MarkRestoreExpected();
        this.material.SetFloat("_AdaptationSpeed", Mathf.Max(this.eyeAdaptation.speed, 1f / 1000f));
        Graphics.Blit((Texture) source1, this.m_SmallAdaptiveRt, this.material, flag ? 3 : 2);
        this.material.SetFloat("_MiddleGrey", this.eyeAdaptation.middleGrey);
        this.material.SetFloat("_AdaptationMin", Mathf.Pow(2f, this.eyeAdaptation.min));
        this.material.SetFloat("_AdaptationMax", Mathf.Pow(2f, this.eyeAdaptation.max));
        this.material.SetTexture("_LumTex", (Texture) this.m_SmallAdaptiveRt);
        this.material.EnableKeyword("ENABLE_EYE_ADAPTATION");
      }
      int pass = 4;
      if (this.tonemapping.enabled)
      {
        if (this.tonemapping.tonemapper == TonemappingColorGrading.Tonemapper.Curve)
        {
          if (this.m_TonemapperDirty)
          {
            float num9 = 1f;
            if (this.tonemapping.curve.length > 0)
            {
              num9 = this.tonemapping.curve[this.tonemapping.curve.length - 1].time;
              for (float num10 = 0.0f; (double) num10 <= 1.0; num10 += 0.003921569f)
              {
                float num11 = this.tonemapping.curve.Evaluate(num10 * num9);
                this.tonemapperCurve.SetPixel(Mathf.FloorToInt(num10 * (float) byte.MaxValue), 0, new Color(num11, num11, num11));
              }
              this.tonemapperCurve.Apply();
            }
            this.m_TonemapperCurveRange = 1f / num9;
            this.m_TonemapperDirty = false;
          }
          this.material.SetFloat("_ToneCurveRange", this.m_TonemapperCurveRange);
          this.material.SetTexture("_ToneCurve", (Texture) this.tonemapperCurve);
        }
        else if (this.tonemapping.tonemapper == TonemappingColorGrading.Tonemapper.Neutral)
        {
          float num12 = (float) ((double) this.tonemapping.neutralBlackIn * 20.0 + 1.0);
          float num13 = (float) ((double) this.tonemapping.neutralBlackOut * 10.0 + 1.0);
          float num14 = this.tonemapping.neutralWhiteIn / 20f;
          float num15 = (float) (1.0 - (double) this.tonemapping.neutralWhiteOut / 20.0);
          float t1 = num12 / num13;
          float t2 = num14 / num15;
          this.material.SetVector("_NeutralTonemapperParams1", new Vector4(0.2f, Mathf.Max(0.0f, Mathf.LerpUnclamped(0.57f, 0.37f, t1)), Mathf.LerpUnclamped(0.01f, 0.24f, t2), Mathf.Max(0.0f, Mathf.LerpUnclamped(0.02f, 0.2f, t1))));
          this.material.SetVector("_NeutralTonemapperParams2", new Vector4(0.02f, 0.3f, this.tonemapping.neutralWhiteLevel, this.tonemapping.neutralWhiteClip / 10f));
        }
        this.material.SetFloat("_Exposure", this.tonemapping.exposure);
        pass += (int) (this.tonemapping.tonemapper + 1);
      }
      if (this.colorGrading.enabled)
      {
        if (this.m_Dirty || !this.m_InternalLut.IsCreated())
        {
          Color lift;
          Color gamma;
          Color gain;
          this.GenerateLiftGammaGain(out lift, out gamma, out gain);
          this.GenCurveTexture();
          this.material.SetVector("_WhiteBalance", (Vector4) this.GetWhiteBalance());
          this.material.SetVector("_Lift", (Vector4) lift);
          this.material.SetVector("_Gamma", (Vector4) gamma);
          this.material.SetVector("_Gain", (Vector4) gain);
          this.material.SetVector("_ContrastGainGamma", (Vector4) new Vector3(this.colorGrading.basics.contrast, this.colorGrading.basics.gain, 1f / this.colorGrading.basics.gamma));
          this.material.SetFloat("_Vibrance", this.colorGrading.basics.vibrance);
          this.material.SetVector("_HSV", new Vector4(this.colorGrading.basics.hue, this.colorGrading.basics.saturation, this.colorGrading.basics.value));
          this.material.SetVector("_ChannelMixerRed", (Vector4) this.colorGrading.channelMixer.channels[0]);
          this.material.SetVector("_ChannelMixerGreen", (Vector4) this.colorGrading.channelMixer.channels[1]);
          this.material.SetVector("_ChannelMixerBlue", (Vector4) this.colorGrading.channelMixer.channels[2]);
          this.material.SetTexture("_CurveTex", (Texture) this.curveTexture);
          this.internalLutRt.MarkRestoreExpected();
          Graphics.Blit((Texture) this.identityLut, this.internalLutRt, this.material, 0);
          this.m_Dirty = false;
        }
        this.material.EnableKeyword("ENABLE_COLOR_GRADING");
        if (this.colorGrading.useDithering)
          this.material.EnableKeyword("ENABLE_DITHERING");
        this.material.SetTexture("_InternalLutTex", (Texture) this.internalLutRt);
        this.material.SetVector("_InternalLutParams", (Vector4) new Vector3(1f / (float) this.internalLutRt.width, 1f / (float) this.internalLutRt.height, (float) this.internalLutRt.height - 1f));
      }
      if (this.lut.enabled && (UnityEngine.Object) this.lut.texture != (UnityEngine.Object) null && this.CheckUserLut())
      {
        this.material.SetTexture("_UserLutTex", this.lut.texture);
        this.material.SetVector("_UserLutParams", new Vector4(1f / (float) this.lut.texture.width, 1f / (float) this.lut.texture.height, (float) this.lut.texture.height - 1f, this.lut.contribution));
        this.material.EnableKeyword("ENABLE_USER_LUT");
      }
      Graphics.Blit((Texture) source, destination, this.material, pass);
      if (!this.eyeAdaptation.enabled)
        return;
      for (int index = 0; index < renderTextureArray.Length; ++index)
        RenderTexture.ReleaseTemporary(renderTextureArray[index]);
      RenderTexture.ReleaseTemporary(renderTexture);
    }

    public Texture2D BakeLUT()
    {
      Texture2D texture2D = new Texture2D(this.internalLutRt.width, this.internalLutRt.height, TextureFormat.RGB24, false, true);
      RenderTexture.active = this.internalLutRt;
      texture2D.ReadPixels(new Rect(0.0f, 0.0f, (float) texture2D.width, (float) texture2D.height), 0, 0);
      RenderTexture.active = (RenderTexture) null;
      return texture2D;
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SettingsGroup : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class IndentedGroup : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ChannelMixer : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
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

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class Curve : PropertyAttribute
    {
      public Color color = Color.white;

      public Curve()
      {
      }

      public Curve(float r, float g, float b, float a) => this.color = new Color(r, g, b, a);
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

      public static TonemappingColorGrading.EyeAdaptationSettings defaultSettings
      {
        get
        {
          return new TonemappingColorGrading.EyeAdaptationSettings()
          {
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
      public TonemappingColorGrading.Tonemapper tonemapper;
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

      public static TonemappingColorGrading.TonemappingSettings defaultSettings
      {
        get
        {
          return new TonemappingColorGrading.TonemappingSettings()
          {
            enabled = false,
            tonemapper = TonemappingColorGrading.Tonemapper.Neutral,
            exposure = 1f,
            curve = TonemappingColorGrading.CurvesSettings.defaultCurve,
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

      public static TonemappingColorGrading.LUTSettings defaultSettings
      {
        get
        {
          return new TonemappingColorGrading.LUTSettings()
          {
            enabled = false,
            texture = (Texture) null,
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

      public static TonemappingColorGrading.ColorWheelsSettings defaultSettings
      {
        get
        {
          return new TonemappingColorGrading.ColorWheelsSettings()
          {
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

      public static TonemappingColorGrading.BasicsSettings defaultSettings
      {
        get
        {
          return new TonemappingColorGrading.BasicsSettings()
          {
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

      public static TonemappingColorGrading.ChannelMixerSettings defaultSettings
      {
        get
        {
          return new TonemappingColorGrading.ChannelMixerSettings()
          {
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
      [TonemappingColorGrading.Curve]
      public AnimationCurve master;
      [TonemappingColorGrading.Curve(1f, 0.0f, 0.0f, 1f)]
      public AnimationCurve red;
      [TonemappingColorGrading.Curve(0.0f, 1f, 0.0f, 1f)]
      public AnimationCurve green;
      [TonemappingColorGrading.Curve(0.0f, 1f, 1f, 1f)]
      public AnimationCurve blue;

      public static TonemappingColorGrading.CurvesSettings defaultSettings
      {
        get
        {
          return new TonemappingColorGrading.CurvesSettings()
          {
            master = TonemappingColorGrading.CurvesSettings.defaultCurve,
            red = TonemappingColorGrading.CurvesSettings.defaultCurve,
            green = TonemappingColorGrading.CurvesSettings.defaultCurve,
            blue = TonemappingColorGrading.CurvesSettings.defaultCurve
          };
        }
      }

      public static AnimationCurve defaultCurve
      {
        get
        {
          return new AnimationCurve(new Keyframe[2]
          {
            new Keyframe(0.0f, 0.0f, 1f, 1f),
            new Keyframe(1f, 1f, 1f, 1f)
          });
        }
      }
    }

    public enum ColorGradingPrecision
    {
      Normal = 16, // 0x00000010
      High = 32, // 0x00000020
    }

    [Serializable]
    public struct ColorGradingSettings
    {
      public bool enabled;
      [Tooltip("Internal LUT precision. \"Normal\" is 256x16, \"High\" is 1024x32. Prefer \"Normal\" on mobile devices.")]
      public TonemappingColorGrading.ColorGradingPrecision precision;
      [Space]
      [TonemappingColorGrading.ColorWheelGroup]
      public TonemappingColorGrading.ColorWheelsSettings colorWheels;
      [Space]
      [TonemappingColorGrading.IndentedGroup]
      public TonemappingColorGrading.BasicsSettings basics;
      [Space]
      [TonemappingColorGrading.ChannelMixer]
      public TonemappingColorGrading.ChannelMixerSettings channelMixer;
      [Space]
      [TonemappingColorGrading.IndentedGroup]
      public TonemappingColorGrading.CurvesSettings curves;
      [Space]
      [Tooltip("Use dithering to try and minimize color banding in dark areas.")]
      public bool useDithering;
      [Tooltip("Displays the generated LUT in the top left corner of the GameView.")]
      public bool showDebug;

      public static TonemappingColorGrading.ColorGradingSettings defaultSettings
      {
        get
        {
          return new TonemappingColorGrading.ColorGradingSettings()
          {
            enabled = false,
            useDithering = false,
            showDebug = false,
            precision = TonemappingColorGrading.ColorGradingPrecision.Normal,
            colorWheels = TonemappingColorGrading.ColorWheelsSettings.defaultSettings,
            basics = TonemappingColorGrading.BasicsSettings.defaultSettings,
            channelMixer = TonemappingColorGrading.ChannelMixerSettings.defaultSettings,
            curves = TonemappingColorGrading.CurvesSettings.defaultSettings
          };
        }
      }

      internal void Reset() => this.curves = TonemappingColorGrading.CurvesSettings.defaultSettings;
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
