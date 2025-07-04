﻿namespace UnityEngine.PostProcessing
{
  public sealed class ColorGradingComponent : PostProcessingComponentRenderTexture<ColorGradingModel>
  {
    private const int k_InternalLogLutSize = 32;
    private const int k_CurvePrecision = 128;
    private const float k_CurveStep = 0.0078125f;
    private Texture2D m_GradingCurves;
    private Color[] m_pixels = new Color[256];

    public override bool active => model.enabled && !context.interrupted;

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

    private Vector3 CalculateColorBalance(float temperature, float tint)
    {
      float num1 = temperature / 55f;
      float num2 = tint / 55f;
      float x = (float) (0.3127099871635437 - num1 * (num1 < 0.0 ? 0.10000000149011612 : 0.05000000074505806));
      float y = StandardIlluminantY(x) + num2 * 0.05f;
      Vector3 vector3 = new Vector3(0.949237f, 1.03542f, 1.08728f);
      Vector3 lms = CIExyToLMS(x, y);
      return new Vector3(vector3.x / lms.x, vector3.y / lms.y, vector3.z / lms.z);
    }

    private static Color NormalizeColor(Color c)
    {
      float a = (float) ((c.r + (double) c.g + c.b) / 3.0);
      if (Mathf.Approximately(a, 0.0f))
        return new Color(1f, 1f, 1f, c.a);
      return new Color {
        r = c.r / a,
        g = c.g / a,
        b = c.b / a,
        a = c.a
      };
    }

    private static Vector3 ClampVector(Vector3 v, float min, float max)
    {
      return new Vector3(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
    }

    public static Vector3 GetLiftValue(Color lift)
    {
      Color color = NormalizeColor(lift);
      float num = (float) ((color.r + (double) color.g + color.b) / 3.0);
      return ClampVector(new Vector3((float) ((color.r - (double) num) * 0.10000000149011612) + lift.a, (float) ((color.g - (double) num) * 0.10000000149011612) + lift.a, (float) ((color.b - (double) num) * 0.10000000149011612) + lift.a), -1f, 1f);
    }

    public static Vector3 GetGammaValue(Color gamma)
    {
      Color color = NormalizeColor(gamma);
      float num = (float) ((color.r + (double) color.g + color.b) / 3.0);
      gamma.a *= gamma.a < 0.0 ? 0.8f : 5f;
      float b1 = Mathf.Pow(2f, (float) ((color.r - (double) num) * 0.5)) + gamma.a;
      float b2 = Mathf.Pow(2f, (float) ((color.g - (double) num) * 0.5)) + gamma.a;
      float b3 = Mathf.Pow(2f, (float) ((color.b - (double) num) * 0.5)) + gamma.a;
      return ClampVector(new Vector3(1f / Mathf.Max(0.01f, b1), 1f / Mathf.Max(0.01f, b2), 1f / Mathf.Max(0.01f, b3)), 0.0f, 5f);
    }

    public static Vector3 GetGainValue(Color gain)
    {
      Color color = NormalizeColor(gain);
      float num = (float) ((color.r + (double) color.g + color.b) / 3.0);
      gain.a *= gain.a > 0.0 ? 3f : 1f;
      return ClampVector(new Vector3(Mathf.Pow(2f, (float) ((color.r - (double) num) * 0.5)) + gain.a, Mathf.Pow(2f, (float) ((color.g - (double) num) * 0.5)) + gain.a, Mathf.Pow(2f, (float) ((color.b - (double) num) * 0.5)) + gain.a), 0.0f, 4f);
    }

    public static void CalculateLiftGammaGain(
      Color lift,
      Color gamma,
      Color gain,
      out Vector3 outLift,
      out Vector3 outGamma,
      out Vector3 outGain)
    {
      outLift = GetLiftValue(lift);
      outGamma = GetGammaValue(gamma);
      outGain = GetGainValue(gain);
    }

    public static Vector3 GetSlopeValue(Color slope)
    {
      Color color = NormalizeColor(slope);
      float num = (float) ((color.r + (double) color.g + color.b) / 3.0);
      slope.a *= 0.5f;
      return ClampVector(new Vector3((float) ((color.r - (double) num) * 0.10000000149011612 + slope.a + 1.0), (float) ((color.g - (double) num) * 0.10000000149011612 + slope.a + 1.0), (float) ((color.b - (double) num) * 0.10000000149011612 + slope.a + 1.0)), 0.0f, 2f);
    }

    public static Vector3 GetPowerValue(Color power)
    {
      Color color = NormalizeColor(power);
      float num = (float) ((color.r + (double) color.g + color.b) / 3.0);
      power.a *= 0.5f;
      float b1 = (float) ((color.r - (double) num) * 0.10000000149011612 + power.a + 1.0);
      float b2 = (float) ((color.g - (double) num) * 0.10000000149011612 + power.a + 1.0);
      float b3 = (float) ((color.b - (double) num) * 0.10000000149011612 + power.a + 1.0);
      return ClampVector(new Vector3(1f / Mathf.Max(0.01f, b1), 1f / Mathf.Max(0.01f, b2), 1f / Mathf.Max(0.01f, b3)), 0.5f, 2.5f);
    }

    public static Vector3 GetOffsetValue(Color offset)
    {
      Color color = NormalizeColor(offset);
      float num = (float) ((color.r + (double) color.g + color.b) / 3.0);
      offset.a *= 0.5f;
      return ClampVector(new Vector3((float) ((color.r - (double) num) * 0.05000000074505806) + offset.a, (float) ((color.g - (double) num) * 0.05000000074505806) + offset.a, (float) ((color.b - (double) num) * 0.05000000074505806) + offset.a), -0.8f, 0.8f);
    }

    public static void CalculateSlopePowerOffset(
      Color slope,
      Color power,
      Color offset,
      out Vector3 outSlope,
      out Vector3 outPower,
      out Vector3 outOffset)
    {
      outSlope = GetSlopeValue(slope);
      outPower = GetPowerValue(power);
      outOffset = GetOffsetValue(offset);
    }

    private Texture2D GetCurveTexture()
    {
      if (m_GradingCurves == null)
      {
        Texture2D texture2D = new Texture2D(128, 2, TextureFormat.RGBAHalf, false, true);
        texture2D.name = "Internal Curves Texture";
        texture2D.hideFlags = HideFlags.DontSave;
        texture2D.anisoLevel = 0;
        texture2D.wrapMode = TextureWrapMode.Clamp;
        texture2D.filterMode = FilterMode.Bilinear;
        m_GradingCurves = texture2D;
      }
      ColorGradingModel.CurvesSettings curves = model.settings.curves;
      curves.hueVShue.Cache();
      curves.hueVSsat.Cache();
      for (int index = 0; index < 128; ++index)
      {
        float t = index * (1f / 128f);
        float r1 = curves.hueVShue.Evaluate(t);
        float g1 = curves.hueVSsat.Evaluate(t);
        float b1 = curves.satVSsat.Evaluate(t);
        float a1 = curves.lumVSsat.Evaluate(t);
        m_pixels[index] = new Color(r1, g1, b1, a1);
        float a2 = curves.master.Evaluate(t);
        float r2 = curves.red.Evaluate(t);
        float g2 = curves.green.Evaluate(t);
        float b2 = curves.blue.Evaluate(t);
        m_pixels[index + 128] = new Color(r2, g2, b2, a2);
      }
      m_GradingCurves.SetPixels(m_pixels);
      m_GradingCurves.Apply(false, false);
      return m_GradingCurves;
    }

    private bool IsLogLutValid(RenderTexture lut)
    {
      return lut != null && lut.IsCreated() && lut.height == 32;
    }

    private void GenerateLut()
    {
      ColorGradingModel.Settings settings = this.model.settings;
      if (!IsLogLutValid(this.model.bakedLut))
      {
        GraphicsUtils.Destroy(this.model.bakedLut);
        ColorGradingModel model = this.model;
        RenderTexture renderTexture = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGBHalf);
        renderTexture.name = "Color Grading Log LUT";
        renderTexture.hideFlags = HideFlags.DontSave;
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.anisoLevel = 0;
        model.bakedLut = renderTexture;
      }
      Material mat = context.materialFactory.Get("Hidden/Post FX/Lut Generator");
      mat.SetVector(Uniforms._LutParams, new Vector4(32f, 0.00048828125f, 1f / 64f, 1.032258f));
      mat.shaderKeywords = null;
      ColorGradingModel.TonemappingSettings tonemapping = settings.tonemapping;
      switch (tonemapping.tonemapper)
      {
        case ColorGradingModel.Tonemapper.ACES:
          mat.EnableKeyword("TONEMAPPING_FILMIC");
          break;
        case ColorGradingModel.Tonemapper.Neutral:
          mat.EnableKeyword("TONEMAPPING_NEUTRAL");
          float num1 = (float) (tonemapping.neutralBlackIn * 20.0 + 1.0);
          float num2 = (float) (tonemapping.neutralBlackOut * 10.0 + 1.0);
          float num3 = tonemapping.neutralWhiteIn / 20f;
          float num4 = (float) (1.0 - tonemapping.neutralWhiteOut / 20.0);
          float t1 = num1 / num2;
          float t2 = num3 / num4;
          float y = Mathf.Max(0.0f, Mathf.LerpUnclamped(0.57f, 0.37f, t1));
          float z = Mathf.LerpUnclamped(0.01f, 0.24f, t2);
          float w = Mathf.Max(0.0f, Mathf.LerpUnclamped(0.02f, 0.2f, t1));
          mat.SetVector(Uniforms._NeutralTonemapperParams1, new Vector4(0.2f, y, z, w));
          mat.SetVector(Uniforms._NeutralTonemapperParams2, new Vector4(0.02f, 0.3f, tonemapping.neutralWhiteLevel, tonemapping.neutralWhiteClip / 10f));
          break;
      }
      mat.SetFloat(Uniforms._HueShift, settings.basic.hueShift / 360f);
      mat.SetFloat(Uniforms._Saturation, settings.basic.saturation);
      mat.SetFloat(Uniforms._Contrast, settings.basic.contrast);
      mat.SetVector(Uniforms._Balance, CalculateColorBalance(settings.basic.temperature, settings.basic.tint));
      CalculateLiftGammaGain(settings.colorWheels.linear.lift, settings.colorWheels.linear.gamma, settings.colorWheels.linear.gain, out Vector3 outLift, out Vector3 outGamma, out Vector3 outGain);
      mat.SetVector(Uniforms._Lift, outLift);
      mat.SetVector(Uniforms._InvGamma, outGamma);
      mat.SetVector(Uniforms._Gain, outGain);
      CalculateSlopePowerOffset(settings.colorWheels.log.slope, settings.colorWheels.log.power, settings.colorWheels.log.offset, out Vector3 outSlope, out Vector3 outPower, out Vector3 outOffset);
      mat.SetVector(Uniforms._Slope, outSlope);
      mat.SetVector(Uniforms._Power, outPower);
      mat.SetVector(Uniforms._Offset, outOffset);
      mat.SetVector(Uniforms._ChannelMixerRed, settings.channelMixer.red);
      mat.SetVector(Uniforms._ChannelMixerGreen, settings.channelMixer.green);
      mat.SetVector(Uniforms._ChannelMixerBlue, settings.channelMixer.blue);
      mat.SetTexture(Uniforms._Curves, GetCurveTexture());
      Graphics.Blit(null, this.model.bakedLut, mat, 0);
    }

    public override void Prepare(Material uberMaterial)
    {
      if (model.isDirty || !IsLogLutValid(model.bakedLut))
      {
        GenerateLut();
        model.isDirty = false;
      }
      uberMaterial.EnableKeyword(context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.PreGradingLog) ? "COLOR_GRADING_LOG_VIEW" : "COLOR_GRADING");
      RenderTexture bakedLut = model.bakedLut;
      uberMaterial.SetTexture(Uniforms._LogLut, bakedLut);
      uberMaterial.SetVector(Uniforms._LogLut_Params, new Vector3(1f / bakedLut.width, 1f / bakedLut.height, bakedLut.height - 1f));
      float num = Mathf.Exp(model.settings.basic.postExposure * 0.6931472f);
      uberMaterial.SetFloat(Uniforms._ExposureEV, num);
    }

    public void OnGUI()
    {
      RenderTexture bakedLut = model.bakedLut;
      GUI.DrawTexture(new Rect((float) (context.viewport.x * (double) Screen.width + 8.0), 8f, bakedLut.width, bakedLut.height), bakedLut);
    }

    public override void OnDisable()
    {
      GraphicsUtils.Destroy(m_GradingCurves);
      GraphicsUtils.Destroy(model.bakedLut);
      m_GradingCurves = null;
      model.bakedLut = null;
    }

    private static class Uniforms
    {
      internal static readonly int _LutParams = Shader.PropertyToID(nameof (_LutParams));
      internal static readonly int _NeutralTonemapperParams1 = Shader.PropertyToID(nameof (_NeutralTonemapperParams1));
      internal static readonly int _NeutralTonemapperParams2 = Shader.PropertyToID(nameof (_NeutralTonemapperParams2));
      internal static readonly int _HueShift = Shader.PropertyToID(nameof (_HueShift));
      internal static readonly int _Saturation = Shader.PropertyToID(nameof (_Saturation));
      internal static readonly int _Contrast = Shader.PropertyToID(nameof (_Contrast));
      internal static readonly int _Balance = Shader.PropertyToID(nameof (_Balance));
      internal static readonly int _Lift = Shader.PropertyToID(nameof (_Lift));
      internal static readonly int _InvGamma = Shader.PropertyToID(nameof (_InvGamma));
      internal static readonly int _Gain = Shader.PropertyToID(nameof (_Gain));
      internal static readonly int _Slope = Shader.PropertyToID(nameof (_Slope));
      internal static readonly int _Power = Shader.PropertyToID(nameof (_Power));
      internal static readonly int _Offset = Shader.PropertyToID(nameof (_Offset));
      internal static readonly int _ChannelMixerRed = Shader.PropertyToID(nameof (_ChannelMixerRed));
      internal static readonly int _ChannelMixerGreen = Shader.PropertyToID(nameof (_ChannelMixerGreen));
      internal static readonly int _ChannelMixerBlue = Shader.PropertyToID(nameof (_ChannelMixerBlue));
      internal static readonly int _Curves = Shader.PropertyToID(nameof (_Curves));
      internal static readonly int _LogLut = Shader.PropertyToID(nameof (_LogLut));
      internal static readonly int _LogLut_Params = Shader.PropertyToID(nameof (_LogLut_Params));
      internal static readonly int _ExposureEV = Shader.PropertyToID(nameof (_ExposureEV));
    }
  }
}
