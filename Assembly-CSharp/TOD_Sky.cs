using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

[ExecuteInEditMode]
[RequireComponent(typeof (TOD_Resources))]
[RequireComponent(typeof (TOD_Components))]
public class TOD_Sky : MonoBehaviour
{
  private const float pi = 3.14159274f;
  private const float tau = 6.28318548f;
  private static IList<TOD_Sky> instances = new List<TOD_Sky>();
  private int probeRenderID = -1;
  [Tooltip("Auto: Use the player settings.\nLinear: Force linear color space.\nGamma: Force gamma color space.")]
  public TOD_ColorSpaceType ColorSpace = TOD_ColorSpaceType.Auto;
  [Tooltip("Auto: Use the camera settings.\nHDR: Force high dynamic range.\nLDR: Force low dynamic range.")]
  public TOD_ColorRangeType ColorRange = TOD_ColorRangeType.Auto;
  [Tooltip("Raw: Write color without modifications.\nDithered: Add dithering to reduce banding.")]
  public TOD_ColorOutputType ColorOutput = TOD_ColorOutputType.Dithered;
  [Tooltip("Per Vertex: Calculate sky color per vertex.\nPer Pixel: Calculate sky color per pixel.")]
  public TOD_SkyQualityType SkyQuality = TOD_SkyQualityType.PerVertex;
  [Tooltip("Low: Only recommended for very old mobile devices.\nMedium: Simplified cloud shading.\nHigh: Physically based cloud shading.")]
  public TOD_CloudQualityType CloudQuality = TOD_CloudQualityType.High;
  [Tooltip("Low: Only recommended for very old mobile devices.\nMedium: Simplified mesh geometry.\nHigh: Detailed mesh geometry.")]
  public TOD_MeshQualityType MeshQuality = TOD_MeshQualityType.High;
  [Tooltip("Low: Recommended for most mobile devices.\nMedium: Includes most visible stars.\nHigh: Includes all visible stars.")]
  public TOD_StarQualityType StarQuality = TOD_StarQualityType.High;
  public TOD_CycleParameters Cycle;
  public TOD_WorldParameters World;
  public TOD_AtmosphereParameters Atmosphere;
  public TOD_DayParameters Day;
  public TOD_NightParameters Night;
  public TOD_SunParameters Sun;
  public TOD_MoonParameters Moon;
  public TOD_StarParameters Stars;
  public TOD_CloudParameters Clouds;
  public TOD_LightParameters Light;
  public TOD_FogParameters Fog;
  public TOD_AmbientParameters Ambient;
  public TOD_ReflectionParameters Reflection;
  private float timeSinceLightUpdate = float.MaxValue;
  private float timeSinceAmbientUpdate = float.MaxValue;
  private float timeSinceReflectionUpdate = float.MaxValue;
  private const int TOD_SAMPLES = 2;
  private Vector3 kBetaMie;
  private Vector4 kSun;
  private Vector4 k4PI;
  private Vector4 kRadius;
  private Vector4 kScale;

  private void UpdateScattering()
  {
    if (Atmosphere == null)
      return;
    float num1 = -Atmosphere.Directionality;
    float num2 = num1 * num1;
    kBetaMie.x = (float) (1.5 * ((1.0 - num2) / (2.0 + num2)));
    kBetaMie.y = 1f + num2;
    kBetaMie.z = 2f * num1;
    float num3 = 1f / 500f * Atmosphere.MieMultiplier;
    float num4 = 1f / 500f * Atmosphere.RayleighMultiplier;
    float num5 = (float) (num4 * 40.0 * 5.2701644897460938);
    float num6 = (float) (num4 * 40.0 * 9.4732837677001953);
    float num7 = (float) (num4 * 40.0 * 19.643802642822266);
    float num8 = num3 * 40f;
    kSun.x = num5;
    kSun.y = num6;
    kSun.z = num7;
    kSun.w = num8;
    float num9 = (float) (num4 * 4.0 * 3.1415927410125732 * 5.2701644897460938);
    float num10 = (float) (num4 * 4.0 * 3.1415927410125732 * 9.4732837677001953);
    float num11 = (float) (num4 * 4.0 * 3.1415927410125732 * 19.643802642822266);
    float num12 = (float) (num3 * 4.0 * 3.1415927410125732);
    k4PI.x = num9;
    k4PI.y = num10;
    k4PI.z = num11;
    k4PI.w = num12;
    kRadius.x = 1f;
    kRadius.y = 1f;
    kRadius.z = 1.025f;
    kRadius.w = 1.050625f;
    kScale.x = 40.00004f;
    kScale.y = 0.25f;
    kScale.z = 160.000153f;
    kScale.w = 0.0001f;
  }

  private void UpdateCelestials()
  {
    if (World == null || Components == null || Components.SpaceTransform == null)
      return;
    float f1 = (float) Math.PI / 180f * World.Latitude;
    float num1 = Mathf.Sin(f1);
    float num2 = Mathf.Cos(f1);
    float longitude = World.Longitude;
    float num3 = 1.57079637f;
    int year = Cycle.Year;
    int month = Cycle.Month;
    int day = Cycle.Day;
    float num4 = Cycle.Hour - World.UTC;
    float num5 = 367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530 + num4 / 24f;
    float num6 = 367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530 + 0.5f;
    float f2 = (float) Math.PI / 180f * (float) (23.439300537109375 - 3.5630000638775527E-07 * num5);
    float num7 = Mathf.Sin(f2);
    float num8 = Mathf.Cos(f2);
    float num9 = (float) (282.94039916992188 + 4.7093501052586362E-05 * num6);
    float num10 = (float) (0.016708999872207642 - 1.1509999620074041E-09 * num6);
    float f3 = (float) Math.PI / 180f * (float) (356.0469970703125 + 0.98560023307800293 * num6);
    float num11 = Mathf.Sin(f3);
    float num12 = Mathf.Cos(f3);
    float f4 = f3 + (float) (num10 * (double) num11 * (1.0 + num10 * (double) num12));
    float num13 = Mathf.Sin(f4);
    float x1 = Mathf.Cos(f4) - num10;
    float y1 = Mathf.Sqrt((float) (1.0 - num10 * (double) num10)) * num13;
    float num14 = 57.29578f * Mathf.Atan2(y1, x1);
    float num15 = Mathf.Sqrt((float) (x1 * (double) x1 + y1 * (double) y1));
    float f5 = (float) Math.PI / 180f * (num14 + num9);
    float num16 = Mathf.Sin(f5);
    float num17 = Mathf.Cos(f5);
    float num18 = num15 * num17;
    float num19 = num15 * num16;
    float x2 = num18;
    float y2 = num19 * num8;
    float y3 = num19 * num7;
    float num20 = 57.29578f * Mathf.Atan2(y2, x2);
    float f6 = Mathf.Atan2(y3, Mathf.Sqrt((float) (x2 * (double) x2 + y2 * (double) y2)));
    float num21 = Mathf.Sin(f6);
    float num22 = Mathf.Cos(f6);
    float num23 = num14 + num9 + 180f;
    float num24 = num20 - num23 - longitude;
    float num25 = 57.29578f * Mathf.Acos((float) ((Mathf.Sin((float) Math.PI / 180f * -6f) - num1 * (double) num21) / (num2 * (double) num22)));
    SunsetTime = (float) ((24.0 + (num24 + (double) num25) / 15.0 % 24.0) % 24.0);
    SunriseTime = (float) ((24.0 + (num24 - (double) num25) / 15.0 % 24.0) % 24.0);
    float num26 = (float) (282.94039916992188 + 4.7093501052586362E-05 * num5);
    float num27 = (float) (0.016708999872207642 - 1.1509999620074041E-09 * num5);
    float f7 = (float) Math.PI / 180f * (float) (356.0469970703125 + 0.98560023307800293 * num5);
    float num28 = Mathf.Sin(f7);
    float num29 = Mathf.Cos(f7);
    float f8 = f7 + (float) (num27 * (double) num28 * (1.0 + num27 * (double) num29));
    float num30 = Mathf.Sin(f8);
    float x3 = Mathf.Cos(f8) - num27;
    float y4 = Mathf.Sqrt((float) (1.0 - num27 * (double) num27)) * num30;
    float num31 = 57.29578f * Mathf.Atan2(y4, x3);
    float num32 = Mathf.Sqrt((float) (x3 * (double) x3 + y4 * (double) y4));
    float f9 = (float) Math.PI / 180f * (num31 + num26);
    float num33 = Mathf.Sin(f9);
    float num34 = Mathf.Cos(f9);
    float num35 = num32 * num34;
    float num36 = num32 * num33;
    float x4 = num35;
    float y5 = num36 * num8;
    float y6 = num36 * num7;
    float num37 = Mathf.Atan2(y5, x4);
    float f10 = Mathf.Atan2(y6, Mathf.Sqrt((float) (x4 * (double) x4 + y5 * (double) y5)));
    float num38 = Mathf.Sin(f10);
    float num39 = Mathf.Cos(f10);
    float num40 = num31 + num26 + 180f + 15f * num4;
    float num41 = (float) (Math.PI / 180.0 * (num40 + (double) longitude));
    LocalSiderealTime = (float) ((num40 + (double) longitude) / 15.0);
    float f11 = num41 - num37;
    float num42 = Mathf.Sin(f11);
    float num43 = Mathf.Cos(f11) * num39;
    float num44 = num42 * num39;
    float num45 = num38;
    float x5 = (float) (num43 * (double) num1 - num45 * (double) num2);
    float y7 = num44;
    float y8 = (float) (num43 * (double) num2 + num45 * (double) num1);
    float num46 = Mathf.Atan2(y7, x5) + 3.14159274f;
    float num47 = Mathf.Atan2(y8, Mathf.Sqrt((float) (x5 * (double) x5 + y7 * (double) y7)));
    float num48 = num3 - num47;
    float num49 = num47;
    float phi1 = num46;
    SunZenith = 57.29578f * num48;
    SunAltitude = 57.29578f * num49;
    SunAzimuth = 57.29578f * phi1;
    float num50;
    float num51;
    float phi2;
    if (Moon.Position == TOD_MoonPositionType.Realistic)
    {
      float num52 = (float) (125.122802734375 - 0.0529538094997406 * num5);
      float num53 = 5.1454f;
      float num54 = (float) (318.06338500976563 + 0.16435731947422028 * num5);
      float num55 = 60.2666f;
      float num56 = 0.0549f;
      float num57 = (float) (115.36540222167969 + 13.064992904663086 * num5);
      float f12 = (float) Math.PI / 180f * num52;
      float num58 = Mathf.Sin(f12);
      float num59 = Mathf.Cos(f12);
      float f13 = (float) Math.PI / 180f * num53;
      float num60 = Mathf.Sin(f13);
      float num61 = Mathf.Cos(f13);
      float f14 = (float) Math.PI / 180f * num57;
      float num62 = Mathf.Sin(f14);
      float num63 = Mathf.Cos(f14);
      float f15 = f14 + (float) (num56 * (double) num62 * (1.0 + num56 * (double) num63));
      float num64 = Mathf.Sin(f15);
      float num65 = Mathf.Cos(f15);
      float x6 = num55 * (num65 - num56);
      float y9 = num55 * (Mathf.Sqrt((float) (1.0 - num56 * (double) num56)) * num64);
      float num66 = 57.29578f * Mathf.Atan2(y9, x6);
      float num67 = Mathf.Sqrt((float) (x6 * (double) x6 + y9 * (double) y9));
      float f16 = (float) Math.PI / 180f * (num66 + num54);
      float num68 = Mathf.Sin(f16);
      float num69 = Mathf.Cos(f16);
      float num70 = num67 * (float) (num59 * (double) num69 - num58 * (double) num68 * num61);
      float num71 = num67 * (float) (num58 * (double) num69 + num59 * (double) num68 * num61);
      float num72 = num67 * (num68 * num60);
      float num73 = num70;
      float num74 = num71;
      float num75 = num72;
      float x7 = num73;
      float y10 = (float) (num74 * (double) num8 - num75 * (double) num7);
      float y11 = (float) (num74 * (double) num7 + num75 * (double) num8);
      float num76 = Mathf.Atan2(y10, x7);
      float f17 = Mathf.Atan2(y11, Mathf.Sqrt((float) (x7 * (double) x7 + y10 * (double) y10)));
      float num77 = Mathf.Sin(f17);
      float num78 = Mathf.Cos(f17);
      float f18 = num41 - num76;
      float num79 = Mathf.Sin(f18);
      float num80 = Mathf.Cos(f18) * num78;
      float num81 = num79 * num78;
      float num82 = num77;
      float x8 = (float) (num80 * (double) num1 - num82 * (double) num2);
      float y12 = num81;
      float y13 = (float) (num80 * (double) num2 + num82 * (double) num1);
      float num83 = Mathf.Atan2(y12, x8) + 3.14159274f;
      float num84 = Mathf.Atan2(y13, Mathf.Sqrt((float) (x8 * (double) x8 + y12 * (double) y12)));
      num50 = num3 - num84;
      num51 = num84;
      phi2 = num83;
    }
    else
    {
      num50 = num48 - 3.14159274f;
      num51 = num49 - 3.14159274f;
      phi2 = phi1;
    }
    MoonZenith = 57.29578f * num50;
    MoonAltitude = 57.29578f * num51;
    MoonAzimuth = 57.29578f * phi2;
    Quaternion quaternion = Quaternion.Euler(90f - World.Latitude, 0.0f, 0.0f) * Quaternion.Euler(0.0f, World.Longitude, 0.0f) * Quaternion.Euler(0.0f, num41 * 57.29578f, 0.0f);
    if (Stars.Position == TOD_StarsPositionType.Rotating)
    {
      Components.SpaceTransform.localRotation = quaternion;
      Components.StarTransform.localRotation = quaternion;
    }
    else
    {
      Components.SpaceTransform.localRotation = Quaternion.identity;
      Components.StarTransform.localRotation = Quaternion.identity;
    }
    Components.SunTransform.localPosition = OrbitalToLocal(num48, phi1);
    Components.SunTransform.LookAt(Components.DomeTransform.position, Components.SunTransform.up);
    Vector3 local = OrbitalToLocal(num50, phi2);
    Vector3 worldUp = quaternion * -Vector3.right;
    Components.MoonTransform.localPosition = local;
    Components.MoonTransform.LookAt(Components.DomeTransform.position, worldUp);
    float num85 = 8f * Mathf.Tan((float) Math.PI / 360f * Sun.MeshSize);
    Components.SunTransform.localScale = new Vector3(num85, num85, num85);
    float num86 = 4f * Mathf.Tan((float) Math.PI / 360f * Moon.MeshSize);
    Components.MoonTransform.localScale = new Vector3(num86, num86, num86);
    bool flag = (1.0 - Atmosphere.Fogginess) * (1.0 - LerpValue) > 0.0;
    Components.SpaceRenderer.enabled = flag;
    Components.StarRenderer.enabled = flag;
    Components.SunRenderer.enabled = Components.SunTransform.localPosition.y > -(double) num85;
    Components.MoonRenderer.enabled = Components.MoonTransform.localPosition.y > -(double) num86;
    Components.AtmosphereRenderer.enabled = true;
    Components.ClearRenderer.enabled = Components.Rays != null;
    Components.CloudRenderer.enabled = Clouds.Coverage > 0.0 && Clouds.Opacity > 0.0;
    LerpValue = Mathf.InverseLerp(105f, 90f, SunZenith);
    float time1 = Mathf.Clamp01(SunZenith / 90f);
    float time2 = Mathf.Clamp01((float) ((SunZenith - 90.0) / 90.0));
    float t = time2 <= 0.0 ? time1 : 1f - time2;
    Atmosphere.RayleighMultiplier = Mathf.Lerp(Day.RayleighMultiplier, Night.RayleighMultiplier, t);
    Atmosphere.MieMultiplier = Mathf.Lerp(Day.MieMultiplier, Night.MieMultiplier, t);
    Atmosphere.Brightness = Mathf.Lerp(Day.Brightness, Night.Brightness, t);
    Atmosphere.Contrast = Mathf.Max(1E-06f, Mathf.Lerp(Day.Contrast, Night.Contrast, t));
    Atmosphere.Directionality = Mathf.Lerp(Day.Directionality, Night.Directionality, t);
    Atmosphere.Fogginess = Mathf.Lerp(Day.Fogginess, Night.Fogginess, t);
    float num87 = Mathf.Clamp01((float) ((LerpValue - 0.10000000149011612) / 0.89999997615814209));
    float num88 = Mathf.Clamp01((float) ((0.10000000149011612 - LerpValue) / 0.10000000149011612));
    float num89 = Mathf.Clamp01((float) ((90.0 - num50 * 57.295780181884766) / 5.0));
    SunVisibility = (1f - Atmosphere.Fogginess) * num87;
    MoonVisibility = (1f - Atmosphere.Fogginess) * num88 * num89;
    SunLightColor = TOD_Util.ApplyAlpha(Day.LightColor.Evaluate(time1));
    MoonLightColor = TOD_Util.ApplyAlpha(Night.LightColor.Evaluate(time2));
    SunRayColor = TOD_Util.ApplyAlpha(Day.RayColor.Evaluate(time1));
    MoonRayColor = TOD_Util.ApplyAlpha(Night.RayColor.Evaluate(time2));
    SunSkyColor = TOD_Util.ApplyAlpha(Day.SkyColor.Evaluate(time1));
    MoonSkyColor = TOD_Util.ApplyAlpha(Night.SkyColor.Evaluate(time2));
    SunMeshColor = TOD_Util.ApplyAlpha(Day.SunColor.Evaluate(time1));
    MoonMeshColor = TOD_Util.ApplyAlpha(Night.MoonColor.Evaluate(time2));
    SunCloudColor = TOD_Util.ApplyAlpha(Day.CloudColor.Evaluate(time1));
    MoonCloudColor = TOD_Util.ApplyAlpha(Night.CloudColor.Evaluate(time2));
    Color b1 = TOD_Util.ApplyAlpha(Day.FogColor.Evaluate(time1));
    FogColor = Color.Lerp(TOD_Util.ApplyAlpha(Night.FogColor.Evaluate(time2)), b1, LerpValue);
    Color b2 = TOD_Util.ApplyAlpha(Day.AmbientColor.Evaluate(time1));
    Color a = TOD_Util.ApplyAlpha(Night.AmbientColor.Evaluate(time2));
    AmbientColor = Color.Lerp(a, b2, LerpValue);
    Color b3 = b2;
    GroundColor = Color.Lerp(a, b3, LerpValue);
    MoonHaloColor = TOD_Util.MulRGB(MoonSkyColor, Moon.HaloBrightness * num89);
    float shadowStrength;
    float num90;
    Color color;
    if (LerpValue > 0.10000000149011612)
    {
      IsDay = true;
      IsNight = false;
      shadowStrength = Day.ShadowStrength;
      num90 = Mathf.Lerp(0.0f, Day.LightIntensity, SunVisibility);
      color = SunLightColor;
      if (Resources.CloudDensityTexture != null)
      {
        float num91 = TOD_Clouds.Instance.LightAttenuation(Resources.CloudDensityTexture, OrbitalToLocal(num48, phi1), 4f * Mathf.Tan((float) Math.PI / 360f * Sun.MeshSize));
        num90 *= (float) (0.25 + num91 * 0.75);
        NGSS_Directional component = Components.LightSource.GetComponent<NGSS_Directional>();
        if (component != null)
          component.NGSS_SOFTNESS = (float) (2.0 - num91 * 1.75);
      }
    }
    else
    {
      IsDay = false;
      IsNight = true;
      shadowStrength = Night.ShadowStrength;
      num90 = Mathf.Lerp(0.0f, Night.LightIntensity, MoonVisibility);
      color = MoonLightColor;
      if (Resources.CloudDensityTexture != null)
      {
        float num92 = TOD_Clouds.Instance.LightAttenuation(Resources.CloudDensityTexture, OrbitalToLocal(num50, phi2), 4f * Mathf.Tan((float) Math.PI / 360f * Moon.MeshSize));
        num90 *= num92;
        NGSS_Directional component = Components.LightSource.GetComponent<NGSS_Directional>();
        if (component != null)
          component.NGSS_SOFTNESS = (float) (2.0 - num92 * 1.75);
      }
    }
    Components.LightSource.color = color;
    Components.LightSource.intensity = num90;
    Components.LightSource.shadowStrength = shadowStrength;
    Components.LightSource.enabled = num90 > 0.0 && color != Color.black;
    if (!Application.isPlaying || timeSinceLightUpdate >= (double) Light.UpdateInterval)
    {
      timeSinceLightUpdate = 0.0f;
      Components.LightTransform.localPosition = IsNight ? OrbitalToLocal(Mathf.Min(num50, (float) ((1.0 - Light.MinimumHeight) * 3.1415927410125732 / 2.0)), phi2) : OrbitalToLocal(Mathf.Min(num48, (float) ((1.0 - Light.MinimumHeight) * 3.1415927410125732 / 2.0)), phi1);
      Components.LightTransform.LookAt(Components.DomeTransform.position);
    }
    else
      timeSinceLightUpdate += Time.deltaTime;
    SunDirection = -Components.SunTransform.forward;
    LocalSunDirection = Components.DomeTransform.InverseTransformDirection(SunDirection);
    MoonDirection = -Components.MoonTransform.forward;
    LocalMoonDirection = Components.DomeTransform.InverseTransformDirection(MoonDirection);
    LightDirection = -Components.LightTransform.forward;
    LocalLightDirection = Components.DomeTransform.InverseTransformDirection(LightDirection);
  }

  public static IList<TOD_Sky> Instances => instances;

  public static TOD_Sky Instance => instances.Count == 0 ? null : instances[instances.Count - 1];

  public bool Initialized { get; private set; }

  public bool Headless => Camera.allCamerasCount == 0;

  public TOD_Components Components { get; private set; }

  public TOD_Resources Resources { get; private set; }

  public bool IsDay { get; private set; }

  public bool IsNight { get; private set; }

  public float Radius => Components.DomeTransform.lossyScale.y;

  public float Diameter => Components.DomeTransform.lossyScale.y * 2f;

  public float LerpValue { get; private set; }

  public float SunZenith { get; private set; }

  public float SunAltitude { get; private set; }

  public float SunAzimuth { get; private set; }

  public float MoonZenith { get; private set; }

  public float MoonAltitude { get; private set; }

  public float MoonAzimuth { get; private set; }

  public float SunsetTime { get; private set; }

  public float SunriseTime { get; private set; }

  public float LocalSiderealTime { get; private set; }

  public float LightZenith => Mathf.Min(SunZenith, MoonZenith);

  public float LightIntensity => Components.LightSource.intensity;

  public float SunVisibility { get; private set; }

  public float MoonVisibility { get; private set; }

  public Vector3 SunDirection { get; private set; }

  public Vector3 MoonDirection { get; private set; }

  public Vector3 LightDirection { get; private set; }

  public Vector3 LocalSunDirection { get; private set; }

  public Vector3 LocalMoonDirection { get; private set; }

  public Vector3 LocalLightDirection { get; private set; }

  public Color SunLightColor { get; private set; }

  public Color MoonLightColor { get; private set; }

  public Color LightColor => Components.LightSource.color;

  public Color SunRayColor { get; private set; }

  public Color MoonRayColor { get; private set; }

  public Color SunSkyColor { get; private set; }

  public Color MoonSkyColor { get; private set; }

  public Color SunMeshColor { get; private set; }

  public Color MoonMeshColor { get; private set; }

  public Color SunCloudColor { get; private set; }

  public Color MoonCloudColor { get; private set; }

  public Color FogColor { get; private set; }

  public Color GroundColor { get; private set; }

  public Color AmbientColor { get; private set; }

  public Color MoonHaloColor { get; private set; }

  public ReflectionProbe Probe { get; private set; }

  public Vector3 OrbitalToUnity(float radius, float theta, float phi)
  {
    float num1 = Mathf.Sin(theta);
    float num2 = Mathf.Cos(theta);
    float num3 = Mathf.Sin(phi);
    float num4 = Mathf.Cos(phi);
    Vector3 unity;
    unity.z = radius * num1 * num4;
    unity.y = radius * num2;
    unity.x = radius * num1 * num3;
    return unity;
  }

  public Vector3 OrbitalToLocal(float theta, float phi)
  {
    float num1 = Mathf.Sin(theta);
    float num2 = Mathf.Cos(theta);
    float num3 = Mathf.Sin(phi);
    float num4 = Mathf.Cos(phi);
    Vector3 local;
    local.z = num1 * num4;
    local.y = num2;
    local.x = num1 * num3;
    return local;
  }

  public Color SampleAtmosphere(Vector3 direction, bool directLight = true)
  {
    return TOD_LINEAR2GAMMA(TOD_HDR2LDR(ShaderScatteringColor(Components.DomeTransform.InverseTransformDirection(direction), directLight)));
  }

  public SphericalHarmonicsL2 RenderToSphericalHarmonics()
  {
    SphericalHarmonicsL2 sphericalHarmonics = new SphericalHarmonicsL2();
    bool directLight = false;
    Color color1 = TOD_Util.ChangeSaturation(AmbientColor.linear, Ambient.Saturation);
    Vector3 vector3 = new Vector3(0.612372458f, 0.5f, 0.612372458f);
    Vector3 up = Vector3.up;
    Color linear = SampleAtmosphere(up, directLight).linear;
    sphericalHarmonics.AddDirectionalLight(up, linear, 0.428571433f);
    Vector3 direction1 = new Vector3(-vector3.x, vector3.y, -vector3.z);
    Color color2 = TOD_Util.ChangeSaturation(SampleAtmosphere(direction1, directLight).linear, Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(direction1, color2, 0.2857143f);
    Vector3 direction2 = new Vector3(vector3.x, vector3.y, -vector3.z);
    Color color3 = TOD_Util.ChangeSaturation(SampleAtmosphere(direction2, directLight).linear, Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(direction2, color3, 0.2857143f);
    Vector3 direction3 = new Vector3(-vector3.x, vector3.y, vector3.z);
    Color color4 = TOD_Util.ChangeSaturation(SampleAtmosphere(direction3, directLight).linear, Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(direction3, color4, 0.2857143f);
    Vector3 direction4 = new Vector3(vector3.x, vector3.y, vector3.z);
    Color color5 = TOD_Util.ChangeSaturation(SampleAtmosphere(direction4, directLight).linear, Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(direction4, color5, 0.2857143f);
    Vector3 left = Vector3.left;
    Color color6 = TOD_Util.ChangeSaturation(SampleAtmosphere(left, directLight).linear, Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(left, color6, 0.142857149f);
    Vector3 right = Vector3.right;
    Color color7 = TOD_Util.ChangeSaturation(SampleAtmosphere(right, directLight).linear, Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(right, color7, 0.142857149f);
    Vector3 back = Vector3.back;
    Color color8 = TOD_Util.ChangeSaturation(SampleAtmosphere(back, directLight).linear, Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(back, color8, 0.142857149f);
    Vector3 forward = Vector3.forward;
    Color color9 = TOD_Util.ChangeSaturation(SampleAtmosphere(forward, directLight).linear, Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(forward, color9, 0.142857149f);
    Vector3 direction5 = new Vector3(-vector3.x, -vector3.y, -vector3.z);
    sphericalHarmonics.AddDirectionalLight(direction5, color1, 0.2857143f);
    Vector3 direction6 = new Vector3(vector3.x, -vector3.y, -vector3.z);
    sphericalHarmonics.AddDirectionalLight(direction6, color1, 0.2857143f);
    Vector3 direction7 = new Vector3(-vector3.x, -vector3.y, vector3.z);
    sphericalHarmonics.AddDirectionalLight(direction7, color1, 0.2857143f);
    Vector3 direction8 = new Vector3(vector3.x, -vector3.y, vector3.z);
    sphericalHarmonics.AddDirectionalLight(direction8, color1, 0.2857143f);
    Vector3 down = Vector3.down;
    sphericalHarmonics.AddDirectionalLight(down, color1, 0.428571433f);
    return sphericalHarmonics;
  }

  public void RenderToCubemap(RenderTexture targetTexture = null)
  {
    if (!(bool) (Object) Probe)
    {
      Probe = new GameObject().AddComponent<ReflectionProbe>();
      Probe.name = gameObject.name + " Reflection Probe";
      Probe.mode = ReflectionProbeMode.Realtime;
    }
    if (probeRenderID >= 0 && !Probe.IsFinishedRendering(probeRenderID))
      return;
    float maxValue = float.MaxValue;
    Probe.transform.position = Components.DomeTransform.position;
    Probe.size = new Vector3(maxValue, maxValue, maxValue);
    Probe.intensity = RenderSettings.reflectionIntensity;
    Probe.clearFlags = Reflection.ClearFlags;
    Probe.cullingMask = Reflection.CullingMask;
    Probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
    Probe.timeSlicingMode = Reflection.TimeSlicing;
    Probe.farClipPlane = Reflection.FarClipPlane;
    Probe.nearClipPlane = Reflection.NearClipPlane;
    probeRenderID = Probe.RenderProbe(targetTexture);
  }

  public Color SampleFogColor(bool directLight = true)
  {
    Vector3 a = Vector3.forward;
    if (Components.Camera != null)
      a = Quaternion.Euler(0.0f, Components.Camera.transform.rotation.eulerAngles.y, 0.0f) * a;
    Color color = SampleAtmosphere(Vector3.Lerp(a, Vector3.up, Fog.HeightBias).normalized, directLight);
    return new Color(color.r, color.g, color.b, 1f);
  }

  public Color SampleSkyColor()
  {
    Vector3 sunDirection = SunDirection;
    sunDirection.y = Mathf.Abs(sunDirection.y);
    Color color = SampleAtmosphere(sunDirection.normalized, false);
    return new Color(color.r, color.g, color.b, 1f);
  }

  public Color SampleEquatorColor()
  {
    Color color = SampleAtmosphere((SunDirection with
    {
      y = 0.0f
    }).normalized, false);
    return new Color(color.r, color.g, color.b, 1f);
  }

  public void UpdateFog()
  {
    if (Fog == null)
      return;
    switch (Fog.Mode)
    {
      case TOD_FogType.Color:
        RenderSettings.fogColor = SampleFogColor(false);
        break;
      case TOD_FogType.Directional:
        RenderSettings.fogColor = SampleFogColor();
        break;
    }
    if (Components.Camera == null)
      return;
    TOD_Scattering scattering = Components.Scattering;
    if (scattering == null)
      return;
    scattering.GlobalDensity = Fog.GlobalDensity;
    scattering.StartDistance = Fog.StartDistance;
    scattering.TransparentDensityMultiplier = Fog.TransparentDensityMultiplier;
  }

  public void UpdateAmbient()
  {
    if (Ambient == null)
      return;
    float saturation = Ambient.Saturation;
    float num = Mathf.Lerp(Night.AmbientMultiplier, Day.AmbientMultiplier, LerpValue);
    Color color1 = TOD_Util.ChangeSaturation(AmbientColor, Ambient.Saturation);
    switch (Ambient.Mode)
    {
      case TOD_AmbientType.Color:
        Shader.SetGlobalInt("Pathologic_Ambient", 0);
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = color1;
        RenderSettings.ambientIntensity = num;
        break;
      case TOD_AmbientType.Gradient:
        Shader.SetGlobalInt("Pathologic_Ambient", 1);
        Color color2 = (TOD_Util.ChangeSaturation(SampleEquatorColor(), saturation) + color1) * 0.5f;
        Color color3 = TOD_Util.ChangeSaturation(SampleSkyColor(), saturation);
        RenderSettings.ambientMode = AmbientMode.Flat;
        Shader.SetGlobalColor("Pathologic_AmbientSkyColor", color3 * num);
        Shader.SetGlobalColor("Pathologic_AmbientEquatorColor", color2 * num);
        Shader.SetGlobalColor("Pathologic_AmbientGroundColor", color1 * num);
        RenderSettings.ambientLight = Color.blue;
        RenderSettings.ambientIntensity = 1f;
        break;
      case TOD_AmbientType.Spherical:
        Shader.SetGlobalInt("Pathologic_Ambient", 0);
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.ambientLight = color1;
        RenderSettings.ambientIntensity = num;
        RenderSettings.ambientProbe = RenderToSphericalHarmonics();
        break;
    }
  }

  public void UpdateReflection()
  {
    if (Reflection.Mode != TOD_ReflectionType.Cubemap)
      return;
    float num = Mathf.Lerp(Night.ReflectionMultiplier, Day.ReflectionMultiplier, LerpValue);
    RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
    RenderSettings.reflectionIntensity = num;
    if (!Application.isPlaying)
      return;
    RenderToCubemap();
  }

  public void LoadParameters(string xml)
  {
    (new XmlSerializer(typeof (TOD_Parameters)).Deserialize(new XmlTextReader(new StringReader(xml))) as TOD_Parameters).ToSky(this);
  }

  private void UpdateQualitySettings()
  {
    if (Headless)
      return;
    Mesh mesh1 = null;
    Mesh mesh2 = null;
    Mesh mesh3 = null;
    Mesh mesh4 = null;
    Mesh mesh5 = null;
    Mesh mesh6 = null;
    switch (MeshQuality)
    {
      case TOD_MeshQualityType.Low:
        mesh1 = Resources.SkyLOD2;
        mesh2 = Resources.SkyLOD2;
        mesh3 = Resources.SkyLOD2;
        mesh4 = Resources.CloudsLOD2;
        mesh5 = Resources.MoonLOD2;
        break;
      case TOD_MeshQualityType.Medium:
        mesh1 = Resources.SkyLOD1;
        mesh2 = Resources.SkyLOD1;
        mesh3 = Resources.SkyLOD2;
        mesh4 = Resources.CloudsLOD1;
        mesh5 = Resources.MoonLOD1;
        break;
      case TOD_MeshQualityType.High:
        mesh1 = Resources.SkyLOD0;
        mesh2 = Resources.SkyLOD0;
        mesh3 = Resources.SkyLOD2;
        mesh4 = Resources.CloudsLOD0;
        mesh5 = Resources.MoonLOD0;
        break;
    }
    switch (StarQuality)
    {
      case TOD_StarQualityType.Low:
        mesh6 = Resources.StarsLOD2;
        break;
      case TOD_StarQualityType.Medium:
        mesh6 = Resources.StarsLOD1;
        break;
      case TOD_StarQualityType.High:
        mesh6 = Resources.StarsLOD0;
        break;
    }
    if ((bool) (Object) Components.SpaceMeshFilter && Components.SpaceMeshFilter.sharedMesh != mesh1)
      Components.SpaceMeshFilter.mesh = mesh1;
    if ((bool) (Object) Components.MoonMeshFilter && Components.MoonMeshFilter.sharedMesh != mesh5)
      Components.MoonMeshFilter.mesh = mesh5;
    if ((bool) (Object) Components.AtmosphereMeshFilter && Components.AtmosphereMeshFilter.sharedMesh != mesh2)
      Components.AtmosphereMeshFilter.mesh = mesh2;
    if ((bool) (Object) Components.ClearMeshFilter && Components.ClearMeshFilter.sharedMesh != mesh3)
      Components.ClearMeshFilter.mesh = mesh3;
    if ((bool) (Object) Components.CloudMeshFilter && Components.CloudMeshFilter.sharedMesh != mesh4)
      Components.CloudMeshFilter.mesh = mesh4;
    if (!(bool) (Object) Components.StarMeshFilter || !(Components.StarMeshFilter.sharedMesh != mesh6))
      return;
    Components.StarMeshFilter.mesh = mesh6;
  }

  private void UpdateRenderSettings()
  {
    if (Headless)
      return;
    UpdateFog();
    if (!Application.isPlaying || timeSinceAmbientUpdate >= (double) Ambient.UpdateInterval)
    {
      timeSinceAmbientUpdate = 0.0f;
      UpdateAmbient();
    }
    else
      timeSinceAmbientUpdate += Time.deltaTime;
    if (!Application.isPlaying || timeSinceReflectionUpdate >= (double) Reflection.UpdateInterval)
    {
      timeSinceReflectionUpdate = 0.0f;
      UpdateReflection();
    }
    else
      timeSinceReflectionUpdate += Time.deltaTime;
  }

  private void UpdateShaderKeywords()
  {
    if (Headless)
      return;
    switch (ColorSpace)
    {
      case TOD_ColorSpaceType.Auto:
        if (QualitySettings.activeColorSpace == UnityEngine.ColorSpace.Linear)
        {
          Shader.EnableKeyword("TOD_OUTPUT_LINEAR");
          break;
        }
        Shader.DisableKeyword("TOD_OUTPUT_LINEAR");
        break;
      case TOD_ColorSpaceType.Linear:
        Shader.EnableKeyword("TOD_OUTPUT_LINEAR");
        break;
      case TOD_ColorSpaceType.Gamma:
        Shader.DisableKeyword("TOD_OUTPUT_LINEAR");
        break;
    }
    switch (ColorRange)
    {
      case TOD_ColorRangeType.Auto:
        if ((bool) (Object) Components.Camera && Components.Camera.HDR)
        {
          Shader.EnableKeyword("TOD_OUTPUT_HDR");
          break;
        }
        Shader.DisableKeyword("TOD_OUTPUT_HDR");
        break;
      case TOD_ColorRangeType.HDR:
        Shader.EnableKeyword("TOD_OUTPUT_HDR");
        break;
      case TOD_ColorRangeType.LDR:
        Shader.DisableKeyword("TOD_OUTPUT_HDR");
        break;
    }
    switch (ColorOutput)
    {
      case TOD_ColorOutputType.Raw:
        Shader.DisableKeyword("TOD_OUTPUT_DITHERING");
        break;
      case TOD_ColorOutputType.Dithered:
        Shader.EnableKeyword("TOD_OUTPUT_DITHERING");
        break;
    }
    switch (SkyQuality)
    {
      case TOD_SkyQualityType.PerVertex:
        Shader.DisableKeyword("TOD_SCATTERING_PER_PIXEL");
        break;
      case TOD_SkyQualityType.PerPixel:
        Shader.EnableKeyword("TOD_SCATTERING_PER_PIXEL");
        break;
    }
    switch (CloudQuality)
    {
      case TOD_CloudQualityType.Low:
        Shader.DisableKeyword("TOD_CLOUDS_DENSITY");
        Shader.DisableKeyword("TOD_CLOUDS_BUMPED");
        break;
      case TOD_CloudQualityType.Medium:
        Shader.EnableKeyword("TOD_CLOUDS_DENSITY");
        Shader.DisableKeyword("TOD_CLOUDS_BUMPED");
        break;
      case TOD_CloudQualityType.High:
        Shader.EnableKeyword("TOD_CLOUDS_DENSITY");
        Shader.EnableKeyword("TOD_CLOUDS_BUMPED");
        break;
    }
  }

  private void UpdateShaderProperties()
  {
    if (Headless)
      return;
    Shader.SetGlobalColor(Resources.ID_SunLightColor, SunLightColor);
    Shader.SetGlobalColor(Resources.ID_MoonLightColor, MoonLightColor);
    Shader.SetGlobalColor(Resources.ID_SunSkyColor, SunSkyColor);
    Shader.SetGlobalColor(Resources.ID_MoonSkyColor, MoonSkyColor);
    Shader.SetGlobalColor(Resources.ID_SunMeshColor, SunMeshColor);
    Shader.SetGlobalColor(Resources.ID_MoonMeshColor, MoonMeshColor);
    Shader.SetGlobalColor(Resources.ID_SunCloudColor, SunCloudColor);
    Shader.SetGlobalColor(Resources.ID_MoonCloudColor, MoonCloudColor);
    Shader.SetGlobalColor(Resources.ID_FogColor, FogColor);
    Shader.SetGlobalColor(Resources.ID_GroundColor, GroundColor);
    Shader.SetGlobalColor(Resources.ID_AmbientColor, AmbientColor);
    Shader.SetGlobalVector(Resources.ID_SunDirection, SunDirection);
    Shader.SetGlobalVector(Resources.ID_MoonDirection, MoonDirection);
    Shader.SetGlobalVector(Resources.ID_LightDirection, LightDirection);
    Shader.SetGlobalVector(Resources.ID_LocalSunDirection, LocalSunDirection);
    Shader.SetGlobalVector(Resources.ID_LocalMoonDirection, LocalMoonDirection);
    Shader.SetGlobalVector(Resources.ID_LocalLightDirection, LocalLightDirection);
    Shader.SetGlobalFloat(Resources.ID_Contrast, Atmosphere.Contrast);
    Shader.SetGlobalFloat(Resources.ID_Brightness, Atmosphere.Brightness);
    Shader.SetGlobalFloat(Resources.ID_Fogginess, Atmosphere.Fogginess);
    Shader.SetGlobalFloat(Resources.ID_Directionality, Atmosphere.Directionality);
    Shader.SetGlobalFloat(Resources.ID_MoonHaloPower, 1f / Moon.HaloSize);
    Shader.SetGlobalColor(Resources.ID_MoonHaloColor, MoonHaloColor);
    float num1 = Mathf.Lerp(0.8f, 0.0f, Clouds.Coverage);
    float num2 = Mathf.Lerp(3f, 9f, Clouds.Sharpness);
    float num3 = Mathf.Lerp(0.0f, 1f, Clouds.Attenuation);
    float num4 = Mathf.Lerp(0.0f, 2f, Clouds.Saturation);
    Shader.SetGlobalFloat(Resources.ID_CloudOpacity, Clouds.Opacity);
    Shader.SetGlobalFloat(Resources.ID_CloudCoverage, num1);
    Shader.SetGlobalFloat(Resources.ID_CloudSharpness, 1f / num2);
    Shader.SetGlobalFloat(Resources.ID_CloudDensity, num2);
    Shader.SetGlobalFloat(Resources.ID_CloudAttenuation, num3);
    Shader.SetGlobalFloat(Resources.ID_CloudSaturation, num4);
    Shader.SetGlobalFloat(Resources.ID_CloudScattering, Clouds.Scattering);
    Shader.SetGlobalFloat(Resources.ID_CloudBrightness, Clouds.Brightness);
    Shader.SetGlobalVector(Resources.ID_CloudOffset, Components.Animation.OffsetUV);
    Shader.SetGlobalVector(Resources.ID_CloudWind, Components.Animation.CloudUV);
    Shader.SetGlobalVector(Resources.ID_CloudSize, new Vector3(Clouds.Size * 2f, Clouds.Size, Clouds.Size * 2f));
    Shader.SetGlobalFloat(Resources.ID_StarSize, Stars.Size);
    Shader.SetGlobalFloat(Resources.ID_StarBrightness, Stars.Brightness);
    Shader.SetGlobalFloat(Resources.ID_StarVisibility, (float) ((1.0 - Atmosphere.Fogginess) * (1.0 - LerpValue)));
    Shader.SetGlobalFloat(Resources.ID_SunMeshContrast, 1f / Mathf.Max(1f / 1000f, Sun.MeshContrast));
    Shader.SetGlobalFloat(Resources.ID_SunMeshBrightness, Sun.MeshBrightness * (1f - Atmosphere.Fogginess));
    Shader.SetGlobalFloat(Resources.ID_MoonMeshContrast, 1f / Mathf.Max(1f / 1000f, Moon.MeshContrast));
    Shader.SetGlobalFloat(Resources.ID_MoonMeshBrightness, Moon.MeshBrightness * (1f - Atmosphere.Fogginess));
    Shader.SetGlobalVector(Resources.ID_kBetaMie, kBetaMie);
    Shader.SetGlobalVector(Resources.ID_kSun, kSun);
    Shader.SetGlobalVector(Resources.ID_k4PI, k4PI);
    Shader.SetGlobalVector(Resources.ID_kRadius, kRadius);
    Shader.SetGlobalVector(Resources.ID_kScale, kScale);
    Shader.SetGlobalMatrix(Resources.ID_World2Sky, Components.DomeTransform.worldToLocalMatrix);
    Shader.SetGlobalMatrix(Resources.ID_Sky2World, Components.DomeTransform.localToWorldMatrix);
  }

  private float ShaderScale(float inCos)
  {
    float num = 1f - inCos;
    return 0.25f * Mathf.Exp((float) (num * (0.45899999141693115 + num * (3.8299999237060547 + num * (num * 5.25 - 6.8000001907348633))) - 0.0028699999675154686));
  }

  private float ShaderMiePhase(float eyeCos, float eyeCos2)
  {
    return kBetaMie.x * (1f + eyeCos2) / Mathf.Pow(kBetaMie.y + kBetaMie.z * eyeCos, 1.5f);
  }

  private float ShaderRayleighPhase(float eyeCos2) => (float) (0.75 + 0.75 * eyeCos2);

  private Color ShaderNightSkyColor(Vector3 dir)
  {
    dir.y = Mathf.Max(0.0f, dir.y);
    return MoonSkyColor * (float) (1.0 - 0.75 * dir.y);
  }

  private Color ShaderMoonHaloColor(Vector3 dir)
  {
    return MoonHaloColor * Mathf.Pow(Mathf.Max(0.0f, Vector3.Dot(dir, LocalMoonDirection)), 1f / Moon.MeshSize);
  }

  private Color TOD_HDR2LDR(Color color)
  {
    return new Color(1f - Mathf.Pow(2f, -Atmosphere.Brightness * color.r), 1f - Mathf.Pow(2f, -Atmosphere.Brightness * color.g), 1f - Mathf.Pow(2f, -Atmosphere.Brightness * color.b), color.a);
  }

  private Color TOD_GAMMA2LINEAR(Color color)
  {
    return new Color(color.r * color.r, color.g * color.g, color.b * color.b, color.a);
  }

  private Color TOD_LINEAR2GAMMA(Color color)
  {
    return new Color(Mathf.Sqrt(color.r), Mathf.Sqrt(color.g), Mathf.Sqrt(color.b), color.a);
  }

  private Color ShaderScatteringColor(Vector3 dir, bool directLight = true)
  {
    dir.y = Mathf.Max(0.0f, dir.y);
    float x1 = kRadius.x;
    float y1 = kRadius.y;
    float w1 = kRadius.w;
    float x2 = kScale.x;
    float z1 = kScale.z;
    float w2 = kScale.w;
    float x3 = k4PI.x;
    float y2 = k4PI.y;
    float z2 = k4PI.z;
    float w3 = k4PI.w;
    float x4 = kSun.x;
    float y3 = kSun.y;
    float z3 = kSun.z;
    float w4 = kSun.w;
    Vector3 rhs1 = new Vector3(0.0f, x1 + w2, 0.0f);
    float num1 = Mathf.Sqrt(w1 + y1 * dir.y * dir.y - y1) - x1 * dir.y;
    float num2 = Mathf.Exp(z1 * -w2) * ShaderScale(Vector3.Dot(dir, rhs1) / (x1 + w2));
    float num3 = num1 / 2f;
    float num4 = num3 * x2;
    Vector3 vector3 = dir * num3;
    Vector3 rhs2 = rhs1 + vector3 * 0.5f;
    float num5 = 0.0f;
    float num6 = 0.0f;
    float num7 = 0.0f;
    for (int index = 0; index < 2; ++index)
    {
      float magnitude = rhs2.magnitude;
      float num8 = 1f / magnitude;
      float num9 = Mathf.Exp(z1 * (x1 - magnitude));
      float num10 = num9 * num4;
      float inCos1 = Vector3.Dot(dir, rhs2) * num8;
      float inCos2 = Vector3.Dot(LocalSunDirection, rhs2) * num8;
      float num11 = num2 + num9 * (ShaderScale(inCos2) - ShaderScale(inCos1));
      float num12 = Mathf.Exp((float) (-(double) num11 * (x3 + (double) w3)));
      float num13 = Mathf.Exp((float) (-(double) num11 * (y2 + (double) w3)));
      float num14 = Mathf.Exp((float) (-(double) num11 * (z2 + (double) w3)));
      num5 += num12 * num10;
      num6 += num13 * num10;
      num7 += num14 * num10;
      rhs2 += vector3;
    }
    float num15 = SunSkyColor.r * num5 * x4;
    float num16 = SunSkyColor.g * num6 * y3;
    float num17 = SunSkyColor.b * num7 * z3;
    float num18 = SunSkyColor.r * num5 * w4;
    float num19 = SunSkyColor.g * num6 * w4;
    float num20 = SunSkyColor.b * num7 * w4;
    float num21 = 0.0f;
    float num22 = 0.0f;
    float num23 = 0.0f;
    float eyeCos = Vector3.Dot(LocalSunDirection, dir);
    float eyeCos2 = eyeCos * eyeCos;
    float num24 = ShaderRayleighPhase(eyeCos2);
    float num25 = num21 + num24 * num15;
    float num26 = num22 + num24 * num16;
    float num27 = num23 + num24 * num17;
    if (directLight)
    {
      float num28 = ShaderMiePhase(eyeCos, eyeCos2);
      num25 += num28 * num18;
      num26 += num28 * num19;
      num27 += num28 * num20;
    }
    Color color1 = ShaderNightSkyColor(dir);
    float a1 = num25 + color1.r;
    float a2 = num26 + color1.g;
    float a3 = num27 + color1.b;
    if (directLight)
    {
      Color color2 = ShaderMoonHaloColor(dir);
      a1 += color2.r;
      a2 += color2.g;
      a3 += color2.b;
    }
    float num29 = Mathf.Lerp(a1, FogColor.r, Atmosphere.Fogginess);
    float num30 = Mathf.Lerp(a2, FogColor.g, Atmosphere.Fogginess);
    float num31 = Mathf.Lerp(a3, FogColor.b, Atmosphere.Fogginess);
    return new Color(Mathf.Pow(num29 * Atmosphere.Brightness, Atmosphere.Contrast), Mathf.Pow(num30 * Atmosphere.Brightness, Atmosphere.Contrast), Mathf.Pow(num31 * Atmosphere.Brightness, Atmosphere.Contrast), 1f);
  }

  private void Initialize()
  {
    Components = GetComponent<TOD_Components>();
    Components.Initialize();
    Resources = GetComponent<TOD_Resources>();
    Resources.Initialize();
    instances.Add(this);
    Initialized = true;
  }

  private void Cleanup()
  {
    if ((bool) (Object) Probe)
      Destroy(Probe.gameObject);
    Shader.SetGlobalInt("Pathologic_Ambient", 0);
    instances.Remove(this);
    Initialized = false;
  }

  protected void OnEnable() => LateUpdate();

  protected void OnDisable() => Cleanup();

  protected void LateUpdate()
  {
    if (!Initialized)
      Initialize();
    UpdateCelestials();
    UpdateScattering();
    UpdateQualitySettings();
    UpdateRenderSettings();
    UpdateShaderKeywords();
    UpdateShaderProperties();
  }

  protected void OnValidate()
  {
    if (Cycle == null)
      return;
    Cycle.DateTime = Cycle.DateTime;
  }
}
