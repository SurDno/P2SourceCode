// Decompiled with JetBrains decompiler
// Type: TOD_Sky
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
[ExecuteInEditMode]
[RequireComponent(typeof (TOD_Resources))]
[RequireComponent(typeof (TOD_Components))]
public class TOD_Sky : MonoBehaviour
{
  private const float pi = 3.14159274f;
  private const float tau = 6.28318548f;
  private static IList<TOD_Sky> instances = (IList<TOD_Sky>) new List<TOD_Sky>();
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
    if (this.Atmosphere == null)
      return;
    float num1 = -this.Atmosphere.Directionality;
    float num2 = num1 * num1;
    this.kBetaMie.x = (float) (1.5 * ((1.0 - (double) num2) / (2.0 + (double) num2)));
    this.kBetaMie.y = 1f + num2;
    this.kBetaMie.z = 2f * num1;
    float num3 = 1f / 500f * this.Atmosphere.MieMultiplier;
    float num4 = 1f / 500f * this.Atmosphere.RayleighMultiplier;
    float num5 = (float) ((double) num4 * 40.0 * 5.2701644897460938);
    float num6 = (float) ((double) num4 * 40.0 * 9.4732837677001953);
    float num7 = (float) ((double) num4 * 40.0 * 19.643802642822266);
    float num8 = num3 * 40f;
    this.kSun.x = num5;
    this.kSun.y = num6;
    this.kSun.z = num7;
    this.kSun.w = num8;
    float num9 = (float) ((double) num4 * 4.0 * 3.1415927410125732 * 5.2701644897460938);
    float num10 = (float) ((double) num4 * 4.0 * 3.1415927410125732 * 9.4732837677001953);
    float num11 = (float) ((double) num4 * 4.0 * 3.1415927410125732 * 19.643802642822266);
    float num12 = (float) ((double) num3 * 4.0 * 3.1415927410125732);
    this.k4PI.x = num9;
    this.k4PI.y = num10;
    this.k4PI.z = num11;
    this.k4PI.w = num12;
    this.kRadius.x = 1f;
    this.kRadius.y = 1f;
    this.kRadius.z = 1.025f;
    this.kRadius.w = 1.050625f;
    this.kScale.x = 40.00004f;
    this.kScale.y = 0.25f;
    this.kScale.z = 160.000153f;
    this.kScale.w = 0.0001f;
  }

  private void UpdateCelestials()
  {
    if (this.World == null || (UnityEngine.Object) this.Components == (UnityEngine.Object) null || (UnityEngine.Object) this.Components.SpaceTransform == (UnityEngine.Object) null)
      return;
    float f1 = (float) Math.PI / 180f * this.World.Latitude;
    float num1 = Mathf.Sin(f1);
    float num2 = Mathf.Cos(f1);
    float longitude = this.World.Longitude;
    float num3 = 1.57079637f;
    int year = this.Cycle.Year;
    int month = this.Cycle.Month;
    int day = this.Cycle.Day;
    float num4 = this.Cycle.Hour - this.World.UTC;
    float num5 = (float) (367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530) + num4 / 24f;
    float num6 = (float) (367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530) + 0.5f;
    float f2 = (float) Math.PI / 180f * (float) (23.439300537109375 - 3.5630000638775527E-07 * (double) num5);
    float num7 = Mathf.Sin(f2);
    float num8 = Mathf.Cos(f2);
    float num9 = (float) (282.94039916992188 + 4.7093501052586362E-05 * (double) num6);
    float num10 = (float) (0.016708999872207642 - 1.1509999620074041E-09 * (double) num6);
    float f3 = (float) Math.PI / 180f * (float) (356.0469970703125 + 0.98560023307800293 * (double) num6);
    float num11 = Mathf.Sin(f3);
    float num12 = Mathf.Cos(f3);
    float f4 = f3 + (float) ((double) num10 * (double) num11 * (1.0 + (double) num10 * (double) num12));
    float num13 = Mathf.Sin(f4);
    float x1 = Mathf.Cos(f4) - num10;
    float y1 = Mathf.Sqrt((float) (1.0 - (double) num10 * (double) num10)) * num13;
    float num14 = 57.29578f * Mathf.Atan2(y1, x1);
    float num15 = Mathf.Sqrt((float) ((double) x1 * (double) x1 + (double) y1 * (double) y1));
    float f5 = (float) Math.PI / 180f * (num14 + num9);
    float num16 = Mathf.Sin(f5);
    float num17 = Mathf.Cos(f5);
    float num18 = num15 * num17;
    float num19 = num15 * num16;
    float x2 = num18;
    float y2 = num19 * num8;
    float y3 = num19 * num7;
    float num20 = 57.29578f * Mathf.Atan2(y2, x2);
    float f6 = Mathf.Atan2(y3, Mathf.Sqrt((float) ((double) x2 * (double) x2 + (double) y2 * (double) y2)));
    float num21 = Mathf.Sin(f6);
    float num22 = Mathf.Cos(f6);
    float num23 = num14 + num9 + 180f;
    float num24 = num20 - num23 - longitude;
    float num25 = 57.29578f * Mathf.Acos((float) (((double) Mathf.Sin((float) Math.PI / 180f * -6f) - (double) num1 * (double) num21) / ((double) num2 * (double) num22)));
    this.SunsetTime = (float) ((24.0 + ((double) num24 + (double) num25) / 15.0 % 24.0) % 24.0);
    this.SunriseTime = (float) ((24.0 + ((double) num24 - (double) num25) / 15.0 % 24.0) % 24.0);
    float num26 = (float) (282.94039916992188 + 4.7093501052586362E-05 * (double) num5);
    float num27 = (float) (0.016708999872207642 - 1.1509999620074041E-09 * (double) num5);
    float f7 = (float) Math.PI / 180f * (float) (356.0469970703125 + 0.98560023307800293 * (double) num5);
    float num28 = Mathf.Sin(f7);
    float num29 = Mathf.Cos(f7);
    float f8 = f7 + (float) ((double) num27 * (double) num28 * (1.0 + (double) num27 * (double) num29));
    float num30 = Mathf.Sin(f8);
    float x3 = Mathf.Cos(f8) - num27;
    float y4 = Mathf.Sqrt((float) (1.0 - (double) num27 * (double) num27)) * num30;
    float num31 = 57.29578f * Mathf.Atan2(y4, x3);
    float num32 = Mathf.Sqrt((float) ((double) x3 * (double) x3 + (double) y4 * (double) y4));
    float f9 = (float) Math.PI / 180f * (num31 + num26);
    float num33 = Mathf.Sin(f9);
    float num34 = Mathf.Cos(f9);
    float num35 = num32 * num34;
    float num36 = num32 * num33;
    float x4 = num35;
    float y5 = num36 * num8;
    float y6 = num36 * num7;
    float num37 = Mathf.Atan2(y5, x4);
    float f10 = Mathf.Atan2(y6, Mathf.Sqrt((float) ((double) x4 * (double) x4 + (double) y5 * (double) y5)));
    float num38 = Mathf.Sin(f10);
    float num39 = Mathf.Cos(f10);
    float num40 = num31 + num26 + 180f + 15f * num4;
    float num41 = (float) (Math.PI / 180.0 * ((double) num40 + (double) longitude));
    this.LocalSiderealTime = (float) (((double) num40 + (double) longitude) / 15.0);
    float f11 = num41 - num37;
    float num42 = Mathf.Sin(f11);
    float num43 = Mathf.Cos(f11) * num39;
    float num44 = num42 * num39;
    float num45 = num38;
    float x5 = (float) ((double) num43 * (double) num1 - (double) num45 * (double) num2);
    float y7 = num44;
    float y8 = (float) ((double) num43 * (double) num2 + (double) num45 * (double) num1);
    float num46 = Mathf.Atan2(y7, x5) + 3.14159274f;
    float num47 = Mathf.Atan2(y8, Mathf.Sqrt((float) ((double) x5 * (double) x5 + (double) y7 * (double) y7)));
    float num48 = num3 - num47;
    float num49 = num47;
    float phi1 = num46;
    this.SunZenith = 57.29578f * num48;
    this.SunAltitude = 57.29578f * num49;
    this.SunAzimuth = 57.29578f * phi1;
    float num50;
    float num51;
    float phi2;
    if (this.Moon.Position == TOD_MoonPositionType.Realistic)
    {
      float num52 = (float) (125.122802734375 - 0.0529538094997406 * (double) num5);
      float num53 = 5.1454f;
      float num54 = (float) (318.06338500976563 + 0.16435731947422028 * (double) num5);
      float num55 = 60.2666f;
      float num56 = 0.0549f;
      float num57 = (float) (115.36540222167969 + 13.064992904663086 * (double) num5);
      float f12 = (float) Math.PI / 180f * num52;
      float num58 = Mathf.Sin(f12);
      float num59 = Mathf.Cos(f12);
      float f13 = (float) Math.PI / 180f * num53;
      float num60 = Mathf.Sin(f13);
      float num61 = Mathf.Cos(f13);
      float f14 = (float) Math.PI / 180f * num57;
      float num62 = Mathf.Sin(f14);
      float num63 = Mathf.Cos(f14);
      float f15 = f14 + (float) ((double) num56 * (double) num62 * (1.0 + (double) num56 * (double) num63));
      float num64 = Mathf.Sin(f15);
      float num65 = Mathf.Cos(f15);
      float x6 = num55 * (num65 - num56);
      float y9 = num55 * (Mathf.Sqrt((float) (1.0 - (double) num56 * (double) num56)) * num64);
      float num66 = 57.29578f * Mathf.Atan2(y9, x6);
      float num67 = Mathf.Sqrt((float) ((double) x6 * (double) x6 + (double) y9 * (double) y9));
      float f16 = (float) Math.PI / 180f * (num66 + num54);
      float num68 = Mathf.Sin(f16);
      float num69 = Mathf.Cos(f16);
      float num70 = num67 * (float) ((double) num59 * (double) num69 - (double) num58 * (double) num68 * (double) num61);
      float num71 = num67 * (float) ((double) num58 * (double) num69 + (double) num59 * (double) num68 * (double) num61);
      float num72 = num67 * (num68 * num60);
      float num73 = num70;
      float num74 = num71;
      float num75 = num72;
      float x7 = num73;
      float y10 = (float) ((double) num74 * (double) num8 - (double) num75 * (double) num7);
      float y11 = (float) ((double) num74 * (double) num7 + (double) num75 * (double) num8);
      float num76 = Mathf.Atan2(y10, x7);
      float f17 = Mathf.Atan2(y11, Mathf.Sqrt((float) ((double) x7 * (double) x7 + (double) y10 * (double) y10)));
      float num77 = Mathf.Sin(f17);
      float num78 = Mathf.Cos(f17);
      float f18 = num41 - num76;
      float num79 = Mathf.Sin(f18);
      float num80 = Mathf.Cos(f18) * num78;
      float num81 = num79 * num78;
      float num82 = num77;
      float x8 = (float) ((double) num80 * (double) num1 - (double) num82 * (double) num2);
      float y12 = num81;
      float y13 = (float) ((double) num80 * (double) num2 + (double) num82 * (double) num1);
      float num83 = Mathf.Atan2(y12, x8) + 3.14159274f;
      float num84 = Mathf.Atan2(y13, Mathf.Sqrt((float) ((double) x8 * (double) x8 + (double) y12 * (double) y12)));
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
    this.MoonZenith = 57.29578f * num50;
    this.MoonAltitude = 57.29578f * num51;
    this.MoonAzimuth = 57.29578f * phi2;
    Quaternion quaternion = Quaternion.Euler(90f - this.World.Latitude, 0.0f, 0.0f) * Quaternion.Euler(0.0f, this.World.Longitude, 0.0f) * Quaternion.Euler(0.0f, num41 * 57.29578f, 0.0f);
    if (this.Stars.Position == TOD_StarsPositionType.Rotating)
    {
      this.Components.SpaceTransform.localRotation = quaternion;
      this.Components.StarTransform.localRotation = quaternion;
    }
    else
    {
      this.Components.SpaceTransform.localRotation = Quaternion.identity;
      this.Components.StarTransform.localRotation = Quaternion.identity;
    }
    this.Components.SunTransform.localPosition = this.OrbitalToLocal(num48, phi1);
    this.Components.SunTransform.LookAt(this.Components.DomeTransform.position, this.Components.SunTransform.up);
    Vector3 local = this.OrbitalToLocal(num50, phi2);
    Vector3 worldUp = quaternion * -Vector3.right;
    this.Components.MoonTransform.localPosition = local;
    this.Components.MoonTransform.LookAt(this.Components.DomeTransform.position, worldUp);
    float num85 = 8f * Mathf.Tan((float) Math.PI / 360f * this.Sun.MeshSize);
    this.Components.SunTransform.localScale = new Vector3(num85, num85, num85);
    float num86 = 4f * Mathf.Tan((float) Math.PI / 360f * this.Moon.MeshSize);
    this.Components.MoonTransform.localScale = new Vector3(num86, num86, num86);
    bool flag = (1.0 - (double) this.Atmosphere.Fogginess) * (1.0 - (double) this.LerpValue) > 0.0;
    this.Components.SpaceRenderer.enabled = flag;
    this.Components.StarRenderer.enabled = flag;
    this.Components.SunRenderer.enabled = (double) this.Components.SunTransform.localPosition.y > -(double) num85;
    this.Components.MoonRenderer.enabled = (double) this.Components.MoonTransform.localPosition.y > -(double) num86;
    this.Components.AtmosphereRenderer.enabled = true;
    this.Components.ClearRenderer.enabled = (UnityEngine.Object) this.Components.Rays != (UnityEngine.Object) null;
    this.Components.CloudRenderer.enabled = (double) this.Clouds.Coverage > 0.0 && (double) this.Clouds.Opacity > 0.0;
    this.LerpValue = Mathf.InverseLerp(105f, 90f, this.SunZenith);
    float time1 = Mathf.Clamp01(this.SunZenith / 90f);
    float time2 = Mathf.Clamp01((float) (((double) this.SunZenith - 90.0) / 90.0));
    float t = (double) time2 <= 0.0 ? time1 : 1f - time2;
    this.Atmosphere.RayleighMultiplier = Mathf.Lerp(this.Day.RayleighMultiplier, this.Night.RayleighMultiplier, t);
    this.Atmosphere.MieMultiplier = Mathf.Lerp(this.Day.MieMultiplier, this.Night.MieMultiplier, t);
    this.Atmosphere.Brightness = Mathf.Lerp(this.Day.Brightness, this.Night.Brightness, t);
    this.Atmosphere.Contrast = Mathf.Max(1E-06f, Mathf.Lerp(this.Day.Contrast, this.Night.Contrast, t));
    this.Atmosphere.Directionality = Mathf.Lerp(this.Day.Directionality, this.Night.Directionality, t);
    this.Atmosphere.Fogginess = Mathf.Lerp(this.Day.Fogginess, this.Night.Fogginess, t);
    float num87 = Mathf.Clamp01((float) (((double) this.LerpValue - 0.10000000149011612) / 0.89999997615814209));
    float num88 = Mathf.Clamp01((float) ((0.10000000149011612 - (double) this.LerpValue) / 0.10000000149011612));
    float num89 = Mathf.Clamp01((float) ((90.0 - (double) num50 * 57.295780181884766) / 5.0));
    this.SunVisibility = (1f - this.Atmosphere.Fogginess) * num87;
    this.MoonVisibility = (1f - this.Atmosphere.Fogginess) * num88 * num89;
    this.SunLightColor = TOD_Util.ApplyAlpha(this.Day.LightColor.Evaluate(time1));
    this.MoonLightColor = TOD_Util.ApplyAlpha(this.Night.LightColor.Evaluate(time2));
    this.SunRayColor = TOD_Util.ApplyAlpha(this.Day.RayColor.Evaluate(time1));
    this.MoonRayColor = TOD_Util.ApplyAlpha(this.Night.RayColor.Evaluate(time2));
    this.SunSkyColor = TOD_Util.ApplyAlpha(this.Day.SkyColor.Evaluate(time1));
    this.MoonSkyColor = TOD_Util.ApplyAlpha(this.Night.SkyColor.Evaluate(time2));
    this.SunMeshColor = TOD_Util.ApplyAlpha(this.Day.SunColor.Evaluate(time1));
    this.MoonMeshColor = TOD_Util.ApplyAlpha(this.Night.MoonColor.Evaluate(time2));
    this.SunCloudColor = TOD_Util.ApplyAlpha(this.Day.CloudColor.Evaluate(time1));
    this.MoonCloudColor = TOD_Util.ApplyAlpha(this.Night.CloudColor.Evaluate(time2));
    Color b1 = TOD_Util.ApplyAlpha(this.Day.FogColor.Evaluate(time1));
    this.FogColor = Color.Lerp(TOD_Util.ApplyAlpha(this.Night.FogColor.Evaluate(time2)), b1, this.LerpValue);
    Color b2 = TOD_Util.ApplyAlpha(this.Day.AmbientColor.Evaluate(time1));
    Color a = TOD_Util.ApplyAlpha(this.Night.AmbientColor.Evaluate(time2));
    this.AmbientColor = Color.Lerp(a, b2, this.LerpValue);
    Color b3 = b2;
    this.GroundColor = Color.Lerp(a, b3, this.LerpValue);
    this.MoonHaloColor = TOD_Util.MulRGB(this.MoonSkyColor, this.Moon.HaloBrightness * num89);
    float shadowStrength;
    float num90;
    Color color;
    if ((double) this.LerpValue > 0.10000000149011612)
    {
      this.IsDay = true;
      this.IsNight = false;
      shadowStrength = this.Day.ShadowStrength;
      num90 = Mathf.Lerp(0.0f, this.Day.LightIntensity, this.SunVisibility);
      color = this.SunLightColor;
      if ((UnityEngine.Object) this.Resources.CloudDensityTexture != (UnityEngine.Object) null)
      {
        float num91 = TOD_Clouds.Instance.LightAttenuation(this.Resources.CloudDensityTexture, this.OrbitalToLocal(num48, phi1), 4f * Mathf.Tan((float) Math.PI / 360f * this.Sun.MeshSize));
        num90 *= (float) (0.25 + (double) num91 * 0.75);
        NGSS_Directional component = this.Components.LightSource.GetComponent<NGSS_Directional>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.NGSS_SOFTNESS = (float) (2.0 - (double) num91 * 1.75);
      }
    }
    else
    {
      this.IsDay = false;
      this.IsNight = true;
      shadowStrength = this.Night.ShadowStrength;
      num90 = Mathf.Lerp(0.0f, this.Night.LightIntensity, this.MoonVisibility);
      color = this.MoonLightColor;
      if ((UnityEngine.Object) this.Resources.CloudDensityTexture != (UnityEngine.Object) null)
      {
        float num92 = TOD_Clouds.Instance.LightAttenuation(this.Resources.CloudDensityTexture, this.OrbitalToLocal(num50, phi2), 4f * Mathf.Tan((float) Math.PI / 360f * this.Moon.MeshSize));
        num90 *= num92;
        NGSS_Directional component = this.Components.LightSource.GetComponent<NGSS_Directional>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.NGSS_SOFTNESS = (float) (2.0 - (double) num92 * 1.75);
      }
    }
    this.Components.LightSource.color = color;
    this.Components.LightSource.intensity = num90;
    this.Components.LightSource.shadowStrength = shadowStrength;
    this.Components.LightSource.enabled = (double) num90 > 0.0 && color != Color.black;
    if (!Application.isPlaying || (double) this.timeSinceLightUpdate >= (double) this.Light.UpdateInterval)
    {
      this.timeSinceLightUpdate = 0.0f;
      this.Components.LightTransform.localPosition = this.IsNight ? this.OrbitalToLocal(Mathf.Min(num50, (float) ((1.0 - (double) this.Light.MinimumHeight) * 3.1415927410125732 / 2.0)), phi2) : this.OrbitalToLocal(Mathf.Min(num48, (float) ((1.0 - (double) this.Light.MinimumHeight) * 3.1415927410125732 / 2.0)), phi1);
      this.Components.LightTransform.LookAt(this.Components.DomeTransform.position);
    }
    else
      this.timeSinceLightUpdate += Time.deltaTime;
    this.SunDirection = -this.Components.SunTransform.forward;
    this.LocalSunDirection = this.Components.DomeTransform.InverseTransformDirection(this.SunDirection);
    this.MoonDirection = -this.Components.MoonTransform.forward;
    this.LocalMoonDirection = this.Components.DomeTransform.InverseTransformDirection(this.MoonDirection);
    this.LightDirection = -this.Components.LightTransform.forward;
    this.LocalLightDirection = this.Components.DomeTransform.InverseTransformDirection(this.LightDirection);
  }

  public static IList<TOD_Sky> Instances => TOD_Sky.instances;

  public static TOD_Sky Instance
  {
    get
    {
      return TOD_Sky.instances.Count == 0 ? (TOD_Sky) null : TOD_Sky.instances[TOD_Sky.instances.Count - 1];
    }
  }

  public bool Initialized { get; private set; }

  public bool Headless => Camera.allCamerasCount == 0;

  public TOD_Components Components { get; private set; }

  public TOD_Resources Resources { get; private set; }

  public bool IsDay { get; private set; }

  public bool IsNight { get; private set; }

  public float Radius => this.Components.DomeTransform.lossyScale.y;

  public float Diameter => this.Components.DomeTransform.lossyScale.y * 2f;

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

  public float LightZenith => Mathf.Min(this.SunZenith, this.MoonZenith);

  public float LightIntensity => this.Components.LightSource.intensity;

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

  public Color LightColor => this.Components.LightSource.color;

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
    return this.TOD_LINEAR2GAMMA(this.TOD_HDR2LDR(this.ShaderScatteringColor(this.Components.DomeTransform.InverseTransformDirection(direction), directLight)));
  }

  public SphericalHarmonicsL2 RenderToSphericalHarmonics()
  {
    SphericalHarmonicsL2 sphericalHarmonics = new SphericalHarmonicsL2();
    bool directLight = false;
    Color color1 = TOD_Util.ChangeSaturation(this.AmbientColor.linear, this.Ambient.Saturation);
    Vector3 vector3 = new Vector3(0.612372458f, 0.5f, 0.612372458f);
    Vector3 up = Vector3.up;
    Color linear = this.SampleAtmosphere(up, directLight).linear;
    sphericalHarmonics.AddDirectionalLight(up, linear, 0.428571433f);
    Vector3 direction1 = new Vector3(-vector3.x, vector3.y, -vector3.z);
    Color color2 = TOD_Util.ChangeSaturation(this.SampleAtmosphere(direction1, directLight).linear, this.Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(direction1, color2, 0.2857143f);
    Vector3 direction2 = new Vector3(vector3.x, vector3.y, -vector3.z);
    Color color3 = TOD_Util.ChangeSaturation(this.SampleAtmosphere(direction2, directLight).linear, this.Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(direction2, color3, 0.2857143f);
    Vector3 direction3 = new Vector3(-vector3.x, vector3.y, vector3.z);
    Color color4 = TOD_Util.ChangeSaturation(this.SampleAtmosphere(direction3, directLight).linear, this.Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(direction3, color4, 0.2857143f);
    Vector3 direction4 = new Vector3(vector3.x, vector3.y, vector3.z);
    Color color5 = TOD_Util.ChangeSaturation(this.SampleAtmosphere(direction4, directLight).linear, this.Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(direction4, color5, 0.2857143f);
    Vector3 left = Vector3.left;
    Color color6 = TOD_Util.ChangeSaturation(this.SampleAtmosphere(left, directLight).linear, this.Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(left, color6, 0.142857149f);
    Vector3 right = Vector3.right;
    Color color7 = TOD_Util.ChangeSaturation(this.SampleAtmosphere(right, directLight).linear, this.Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(right, color7, 0.142857149f);
    Vector3 back = Vector3.back;
    Color color8 = TOD_Util.ChangeSaturation(this.SampleAtmosphere(back, directLight).linear, this.Ambient.Saturation);
    sphericalHarmonics.AddDirectionalLight(back, color8, 0.142857149f);
    Vector3 forward = Vector3.forward;
    Color color9 = TOD_Util.ChangeSaturation(this.SampleAtmosphere(forward, directLight).linear, this.Ambient.Saturation);
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
    if (!(bool) (UnityEngine.Object) this.Probe)
    {
      this.Probe = new GameObject().AddComponent<ReflectionProbe>();
      this.Probe.name = this.gameObject.name + " Reflection Probe";
      this.Probe.mode = ReflectionProbeMode.Realtime;
    }
    if (this.probeRenderID >= 0 && !this.Probe.IsFinishedRendering(this.probeRenderID))
      return;
    float maxValue = float.MaxValue;
    this.Probe.transform.position = this.Components.DomeTransform.position;
    this.Probe.size = new Vector3(maxValue, maxValue, maxValue);
    this.Probe.intensity = RenderSettings.reflectionIntensity;
    this.Probe.clearFlags = this.Reflection.ClearFlags;
    this.Probe.cullingMask = (int) this.Reflection.CullingMask;
    this.Probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
    this.Probe.timeSlicingMode = this.Reflection.TimeSlicing;
    this.Probe.farClipPlane = this.Reflection.FarClipPlane;
    this.Probe.nearClipPlane = this.Reflection.NearClipPlane;
    this.probeRenderID = this.Probe.RenderProbe(targetTexture);
  }

  public Color SampleFogColor(bool directLight = true)
  {
    Vector3 a = Vector3.forward;
    if ((UnityEngine.Object) this.Components.Camera != (UnityEngine.Object) null)
      a = Quaternion.Euler(0.0f, this.Components.Camera.transform.rotation.eulerAngles.y, 0.0f) * a;
    Color color = this.SampleAtmosphere(Vector3.Lerp(a, Vector3.up, this.Fog.HeightBias).normalized, directLight);
    return new Color(color.r, color.g, color.b, 1f);
  }

  public Color SampleSkyColor()
  {
    Vector3 sunDirection = this.SunDirection;
    sunDirection.y = Mathf.Abs(sunDirection.y);
    Color color = this.SampleAtmosphere(sunDirection.normalized, false);
    return new Color(color.r, color.g, color.b, 1f);
  }

  public Color SampleEquatorColor()
  {
    Color color = this.SampleAtmosphere((this.SunDirection with
    {
      y = 0.0f
    }).normalized, false);
    return new Color(color.r, color.g, color.b, 1f);
  }

  public void UpdateFog()
  {
    if (this.Fog == null)
      return;
    switch (this.Fog.Mode)
    {
      case TOD_FogType.Color:
        RenderSettings.fogColor = this.SampleFogColor(false);
        break;
      case TOD_FogType.Directional:
        RenderSettings.fogColor = this.SampleFogColor();
        break;
    }
    if ((UnityEngine.Object) this.Components.Camera == (UnityEngine.Object) null)
      return;
    TOD_Scattering scattering = this.Components.Scattering;
    if ((UnityEngine.Object) scattering == (UnityEngine.Object) null)
      return;
    scattering.GlobalDensity = this.Fog.GlobalDensity;
    scattering.StartDistance = this.Fog.StartDistance;
    scattering.TransparentDensityMultiplier = this.Fog.TransparentDensityMultiplier;
  }

  public void UpdateAmbient()
  {
    if (this.Ambient == null)
      return;
    float saturation = this.Ambient.Saturation;
    float num = Mathf.Lerp(this.Night.AmbientMultiplier, this.Day.AmbientMultiplier, this.LerpValue);
    Color color1 = TOD_Util.ChangeSaturation(this.AmbientColor, this.Ambient.Saturation);
    switch (this.Ambient.Mode)
    {
      case TOD_AmbientType.Color:
        Shader.SetGlobalInt("Pathologic_Ambient", 0);
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = color1;
        RenderSettings.ambientIntensity = num;
        break;
      case TOD_AmbientType.Gradient:
        Shader.SetGlobalInt("Pathologic_Ambient", 1);
        Color color2 = (TOD_Util.ChangeSaturation(this.SampleEquatorColor(), saturation) + color1) * 0.5f;
        Color color3 = TOD_Util.ChangeSaturation(this.SampleSkyColor(), saturation);
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
        RenderSettings.ambientProbe = this.RenderToSphericalHarmonics();
        break;
    }
  }

  public void UpdateReflection()
  {
    if (this.Reflection.Mode != TOD_ReflectionType.Cubemap)
      return;
    float num = Mathf.Lerp(this.Night.ReflectionMultiplier, this.Day.ReflectionMultiplier, this.LerpValue);
    RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
    RenderSettings.reflectionIntensity = num;
    if (!Application.isPlaying)
      return;
    this.RenderToCubemap();
  }

  public void LoadParameters(string xml)
  {
    (new XmlSerializer(typeof (TOD_Parameters)).Deserialize((XmlReader) new XmlTextReader((TextReader) new StringReader(xml))) as TOD_Parameters).ToSky(this);
  }

  private void UpdateQualitySettings()
  {
    if (this.Headless)
      return;
    Mesh mesh1 = (Mesh) null;
    Mesh mesh2 = (Mesh) null;
    Mesh mesh3 = (Mesh) null;
    Mesh mesh4 = (Mesh) null;
    Mesh mesh5 = (Mesh) null;
    Mesh mesh6 = (Mesh) null;
    switch (this.MeshQuality)
    {
      case TOD_MeshQualityType.Low:
        mesh1 = this.Resources.SkyLOD2;
        mesh2 = this.Resources.SkyLOD2;
        mesh3 = this.Resources.SkyLOD2;
        mesh4 = this.Resources.CloudsLOD2;
        mesh5 = this.Resources.MoonLOD2;
        break;
      case TOD_MeshQualityType.Medium:
        mesh1 = this.Resources.SkyLOD1;
        mesh2 = this.Resources.SkyLOD1;
        mesh3 = this.Resources.SkyLOD2;
        mesh4 = this.Resources.CloudsLOD1;
        mesh5 = this.Resources.MoonLOD1;
        break;
      case TOD_MeshQualityType.High:
        mesh1 = this.Resources.SkyLOD0;
        mesh2 = this.Resources.SkyLOD0;
        mesh3 = this.Resources.SkyLOD2;
        mesh4 = this.Resources.CloudsLOD0;
        mesh5 = this.Resources.MoonLOD0;
        break;
    }
    switch (this.StarQuality)
    {
      case TOD_StarQualityType.Low:
        mesh6 = this.Resources.StarsLOD2;
        break;
      case TOD_StarQualityType.Medium:
        mesh6 = this.Resources.StarsLOD1;
        break;
      case TOD_StarQualityType.High:
        mesh6 = this.Resources.StarsLOD0;
        break;
    }
    if ((bool) (UnityEngine.Object) this.Components.SpaceMeshFilter && (UnityEngine.Object) this.Components.SpaceMeshFilter.sharedMesh != (UnityEngine.Object) mesh1)
      this.Components.SpaceMeshFilter.mesh = mesh1;
    if ((bool) (UnityEngine.Object) this.Components.MoonMeshFilter && (UnityEngine.Object) this.Components.MoonMeshFilter.sharedMesh != (UnityEngine.Object) mesh5)
      this.Components.MoonMeshFilter.mesh = mesh5;
    if ((bool) (UnityEngine.Object) this.Components.AtmosphereMeshFilter && (UnityEngine.Object) this.Components.AtmosphereMeshFilter.sharedMesh != (UnityEngine.Object) mesh2)
      this.Components.AtmosphereMeshFilter.mesh = mesh2;
    if ((bool) (UnityEngine.Object) this.Components.ClearMeshFilter && (UnityEngine.Object) this.Components.ClearMeshFilter.sharedMesh != (UnityEngine.Object) mesh3)
      this.Components.ClearMeshFilter.mesh = mesh3;
    if ((bool) (UnityEngine.Object) this.Components.CloudMeshFilter && (UnityEngine.Object) this.Components.CloudMeshFilter.sharedMesh != (UnityEngine.Object) mesh4)
      this.Components.CloudMeshFilter.mesh = mesh4;
    if (!(bool) (UnityEngine.Object) this.Components.StarMeshFilter || !((UnityEngine.Object) this.Components.StarMeshFilter.sharedMesh != (UnityEngine.Object) mesh6))
      return;
    this.Components.StarMeshFilter.mesh = mesh6;
  }

  private void UpdateRenderSettings()
  {
    if (this.Headless)
      return;
    this.UpdateFog();
    if (!Application.isPlaying || (double) this.timeSinceAmbientUpdate >= (double) this.Ambient.UpdateInterval)
    {
      this.timeSinceAmbientUpdate = 0.0f;
      this.UpdateAmbient();
    }
    else
      this.timeSinceAmbientUpdate += Time.deltaTime;
    if (!Application.isPlaying || (double) this.timeSinceReflectionUpdate >= (double) this.Reflection.UpdateInterval)
    {
      this.timeSinceReflectionUpdate = 0.0f;
      this.UpdateReflection();
    }
    else
      this.timeSinceReflectionUpdate += Time.deltaTime;
  }

  private void UpdateShaderKeywords()
  {
    if (this.Headless)
      return;
    switch (this.ColorSpace)
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
    switch (this.ColorRange)
    {
      case TOD_ColorRangeType.Auto:
        if ((bool) (UnityEngine.Object) this.Components.Camera && this.Components.Camera.HDR)
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
    switch (this.ColorOutput)
    {
      case TOD_ColorOutputType.Raw:
        Shader.DisableKeyword("TOD_OUTPUT_DITHERING");
        break;
      case TOD_ColorOutputType.Dithered:
        Shader.EnableKeyword("TOD_OUTPUT_DITHERING");
        break;
    }
    switch (this.SkyQuality)
    {
      case TOD_SkyQualityType.PerVertex:
        Shader.DisableKeyword("TOD_SCATTERING_PER_PIXEL");
        break;
      case TOD_SkyQualityType.PerPixel:
        Shader.EnableKeyword("TOD_SCATTERING_PER_PIXEL");
        break;
    }
    switch (this.CloudQuality)
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
    if (this.Headless)
      return;
    Shader.SetGlobalColor(this.Resources.ID_SunLightColor, this.SunLightColor);
    Shader.SetGlobalColor(this.Resources.ID_MoonLightColor, this.MoonLightColor);
    Shader.SetGlobalColor(this.Resources.ID_SunSkyColor, this.SunSkyColor);
    Shader.SetGlobalColor(this.Resources.ID_MoonSkyColor, this.MoonSkyColor);
    Shader.SetGlobalColor(this.Resources.ID_SunMeshColor, this.SunMeshColor);
    Shader.SetGlobalColor(this.Resources.ID_MoonMeshColor, this.MoonMeshColor);
    Shader.SetGlobalColor(this.Resources.ID_SunCloudColor, this.SunCloudColor);
    Shader.SetGlobalColor(this.Resources.ID_MoonCloudColor, this.MoonCloudColor);
    Shader.SetGlobalColor(this.Resources.ID_FogColor, this.FogColor);
    Shader.SetGlobalColor(this.Resources.ID_GroundColor, this.GroundColor);
    Shader.SetGlobalColor(this.Resources.ID_AmbientColor, this.AmbientColor);
    Shader.SetGlobalVector(this.Resources.ID_SunDirection, (Vector4) this.SunDirection);
    Shader.SetGlobalVector(this.Resources.ID_MoonDirection, (Vector4) this.MoonDirection);
    Shader.SetGlobalVector(this.Resources.ID_LightDirection, (Vector4) this.LightDirection);
    Shader.SetGlobalVector(this.Resources.ID_LocalSunDirection, (Vector4) this.LocalSunDirection);
    Shader.SetGlobalVector(this.Resources.ID_LocalMoonDirection, (Vector4) this.LocalMoonDirection);
    Shader.SetGlobalVector(this.Resources.ID_LocalLightDirection, (Vector4) this.LocalLightDirection);
    Shader.SetGlobalFloat(this.Resources.ID_Contrast, this.Atmosphere.Contrast);
    Shader.SetGlobalFloat(this.Resources.ID_Brightness, this.Atmosphere.Brightness);
    Shader.SetGlobalFloat(this.Resources.ID_Fogginess, this.Atmosphere.Fogginess);
    Shader.SetGlobalFloat(this.Resources.ID_Directionality, this.Atmosphere.Directionality);
    Shader.SetGlobalFloat(this.Resources.ID_MoonHaloPower, 1f / this.Moon.HaloSize);
    Shader.SetGlobalColor(this.Resources.ID_MoonHaloColor, this.MoonHaloColor);
    float num1 = Mathf.Lerp(0.8f, 0.0f, this.Clouds.Coverage);
    float num2 = Mathf.Lerp(3f, 9f, this.Clouds.Sharpness);
    float num3 = Mathf.Lerp(0.0f, 1f, this.Clouds.Attenuation);
    float num4 = Mathf.Lerp(0.0f, 2f, this.Clouds.Saturation);
    Shader.SetGlobalFloat(this.Resources.ID_CloudOpacity, this.Clouds.Opacity);
    Shader.SetGlobalFloat(this.Resources.ID_CloudCoverage, num1);
    Shader.SetGlobalFloat(this.Resources.ID_CloudSharpness, 1f / num2);
    Shader.SetGlobalFloat(this.Resources.ID_CloudDensity, num2);
    Shader.SetGlobalFloat(this.Resources.ID_CloudAttenuation, num3);
    Shader.SetGlobalFloat(this.Resources.ID_CloudSaturation, num4);
    Shader.SetGlobalFloat(this.Resources.ID_CloudScattering, this.Clouds.Scattering);
    Shader.SetGlobalFloat(this.Resources.ID_CloudBrightness, this.Clouds.Brightness);
    Shader.SetGlobalVector(this.Resources.ID_CloudOffset, (Vector4) this.Components.Animation.OffsetUV);
    Shader.SetGlobalVector(this.Resources.ID_CloudWind, (Vector4) this.Components.Animation.CloudUV);
    Shader.SetGlobalVector(this.Resources.ID_CloudSize, (Vector4) new Vector3(this.Clouds.Size * 2f, this.Clouds.Size, this.Clouds.Size * 2f));
    Shader.SetGlobalFloat(this.Resources.ID_StarSize, this.Stars.Size);
    Shader.SetGlobalFloat(this.Resources.ID_StarBrightness, this.Stars.Brightness);
    Shader.SetGlobalFloat(this.Resources.ID_StarVisibility, (float) ((1.0 - (double) this.Atmosphere.Fogginess) * (1.0 - (double) this.LerpValue)));
    Shader.SetGlobalFloat(this.Resources.ID_SunMeshContrast, 1f / Mathf.Max(1f / 1000f, this.Sun.MeshContrast));
    Shader.SetGlobalFloat(this.Resources.ID_SunMeshBrightness, this.Sun.MeshBrightness * (1f - this.Atmosphere.Fogginess));
    Shader.SetGlobalFloat(this.Resources.ID_MoonMeshContrast, 1f / Mathf.Max(1f / 1000f, this.Moon.MeshContrast));
    Shader.SetGlobalFloat(this.Resources.ID_MoonMeshBrightness, this.Moon.MeshBrightness * (1f - this.Atmosphere.Fogginess));
    Shader.SetGlobalVector(this.Resources.ID_kBetaMie, (Vector4) this.kBetaMie);
    Shader.SetGlobalVector(this.Resources.ID_kSun, this.kSun);
    Shader.SetGlobalVector(this.Resources.ID_k4PI, this.k4PI);
    Shader.SetGlobalVector(this.Resources.ID_kRadius, this.kRadius);
    Shader.SetGlobalVector(this.Resources.ID_kScale, this.kScale);
    Shader.SetGlobalMatrix(this.Resources.ID_World2Sky, this.Components.DomeTransform.worldToLocalMatrix);
    Shader.SetGlobalMatrix(this.Resources.ID_Sky2World, this.Components.DomeTransform.localToWorldMatrix);
  }

  private float ShaderScale(float inCos)
  {
    float num = 1f - inCos;
    return 0.25f * Mathf.Exp((float) ((double) num * (0.45899999141693115 + (double) num * (3.8299999237060547 + (double) num * ((double) num * 5.25 - 6.8000001907348633))) - 0.0028699999675154686));
  }

  private float ShaderMiePhase(float eyeCos, float eyeCos2)
  {
    return this.kBetaMie.x * (1f + eyeCos2) / Mathf.Pow(this.kBetaMie.y + this.kBetaMie.z * eyeCos, 1.5f);
  }

  private float ShaderRayleighPhase(float eyeCos2) => (float) (0.75 + 0.75 * (double) eyeCos2);

  private Color ShaderNightSkyColor(Vector3 dir)
  {
    dir.y = Mathf.Max(0.0f, dir.y);
    return this.MoonSkyColor * (float) (1.0 - 0.75 * (double) dir.y);
  }

  private Color ShaderMoonHaloColor(Vector3 dir)
  {
    return this.MoonHaloColor * Mathf.Pow(Mathf.Max(0.0f, Vector3.Dot(dir, this.LocalMoonDirection)), 1f / this.Moon.MeshSize);
  }

  private Color TOD_HDR2LDR(Color color)
  {
    return new Color(1f - Mathf.Pow(2f, -this.Atmosphere.Brightness * color.r), 1f - Mathf.Pow(2f, -this.Atmosphere.Brightness * color.g), 1f - Mathf.Pow(2f, -this.Atmosphere.Brightness * color.b), color.a);
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
    float x1 = this.kRadius.x;
    float y1 = this.kRadius.y;
    float w1 = this.kRadius.w;
    float x2 = this.kScale.x;
    float z1 = this.kScale.z;
    float w2 = this.kScale.w;
    float x3 = this.k4PI.x;
    float y2 = this.k4PI.y;
    float z2 = this.k4PI.z;
    float w3 = this.k4PI.w;
    float x4 = this.kSun.x;
    float y3 = this.kSun.y;
    float z3 = this.kSun.z;
    float w4 = this.kSun.w;
    Vector3 rhs1 = new Vector3(0.0f, x1 + w2, 0.0f);
    float num1 = Mathf.Sqrt(w1 + y1 * dir.y * dir.y - y1) - x1 * dir.y;
    float num2 = Mathf.Exp(z1 * -w2) * this.ShaderScale(Vector3.Dot(dir, rhs1) / (x1 + w2));
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
      float inCos2 = Vector3.Dot(this.LocalSunDirection, rhs2) * num8;
      float num11 = num2 + num9 * (this.ShaderScale(inCos2) - this.ShaderScale(inCos1));
      float num12 = Mathf.Exp((float) (-(double) num11 * ((double) x3 + (double) w3)));
      float num13 = Mathf.Exp((float) (-(double) num11 * ((double) y2 + (double) w3)));
      float num14 = Mathf.Exp((float) (-(double) num11 * ((double) z2 + (double) w3)));
      num5 += num12 * num10;
      num6 += num13 * num10;
      num7 += num14 * num10;
      rhs2 += vector3;
    }
    float num15 = this.SunSkyColor.r * num5 * x4;
    float num16 = this.SunSkyColor.g * num6 * y3;
    float num17 = this.SunSkyColor.b * num7 * z3;
    float num18 = this.SunSkyColor.r * num5 * w4;
    float num19 = this.SunSkyColor.g * num6 * w4;
    float num20 = this.SunSkyColor.b * num7 * w4;
    float num21 = 0.0f;
    float num22 = 0.0f;
    float num23 = 0.0f;
    float eyeCos = Vector3.Dot(this.LocalSunDirection, dir);
    float eyeCos2 = eyeCos * eyeCos;
    float num24 = this.ShaderRayleighPhase(eyeCos2);
    float num25 = num21 + num24 * num15;
    float num26 = num22 + num24 * num16;
    float num27 = num23 + num24 * num17;
    if (directLight)
    {
      float num28 = this.ShaderMiePhase(eyeCos, eyeCos2);
      num25 += num28 * num18;
      num26 += num28 * num19;
      num27 += num28 * num20;
    }
    Color color1 = this.ShaderNightSkyColor(dir);
    float a1 = num25 + color1.r;
    float a2 = num26 + color1.g;
    float a3 = num27 + color1.b;
    if (directLight)
    {
      Color color2 = this.ShaderMoonHaloColor(dir);
      a1 += color2.r;
      a2 += color2.g;
      a3 += color2.b;
    }
    float num29 = Mathf.Lerp(a1, this.FogColor.r, this.Atmosphere.Fogginess);
    float num30 = Mathf.Lerp(a2, this.FogColor.g, this.Atmosphere.Fogginess);
    float num31 = Mathf.Lerp(a3, this.FogColor.b, this.Atmosphere.Fogginess);
    return new Color(Mathf.Pow(num29 * this.Atmosphere.Brightness, this.Atmosphere.Contrast), Mathf.Pow(num30 * this.Atmosphere.Brightness, this.Atmosphere.Contrast), Mathf.Pow(num31 * this.Atmosphere.Brightness, this.Atmosphere.Contrast), 1f);
  }

  private void Initialize()
  {
    this.Components = this.GetComponent<TOD_Components>();
    this.Components.Initialize();
    this.Resources = this.GetComponent<TOD_Resources>();
    this.Resources.Initialize();
    TOD_Sky.instances.Add(this);
    this.Initialized = true;
  }

  private void Cleanup()
  {
    if ((bool) (UnityEngine.Object) this.Probe)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.Probe.gameObject);
    Shader.SetGlobalInt("Pathologic_Ambient", 0);
    TOD_Sky.instances.Remove(this);
    this.Initialized = false;
  }

  protected void OnEnable() => this.LateUpdate();

  protected void OnDisable() => this.Cleanup();

  protected void LateUpdate()
  {
    if (!this.Initialized)
      this.Initialize();
    this.UpdateCelestials();
    this.UpdateScattering();
    this.UpdateQualitySettings();
    this.UpdateRenderSettings();
    this.UpdateShaderKeywords();
    this.UpdateShaderProperties();
  }

  protected void OnValidate()
  {
    if (this.Cycle == null)
      return;
    this.Cycle.DateTime = this.Cycle.DateTime;
  }
}
