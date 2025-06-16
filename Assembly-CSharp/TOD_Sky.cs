using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

[ExecuteInEditMode]
[RequireComponent(typeof(TOD_Resources))]
[RequireComponent(typeof(TOD_Components))]
public class TOD_Sky : MonoBehaviour {
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

	[Tooltip(
		"Low: Only recommended for very old mobile devices.\nMedium: Simplified cloud shading.\nHigh: Physically based cloud shading.")]
	public TOD_CloudQualityType CloudQuality = TOD_CloudQualityType.High;

	[Tooltip(
		"Low: Only recommended for very old mobile devices.\nMedium: Simplified mesh geometry.\nHigh: Detailed mesh geometry.")]
	public TOD_MeshQualityType MeshQuality = TOD_MeshQualityType.High;

	[Tooltip(
		"Low: Recommended for most mobile devices.\nMedium: Includes most visible stars.\nHigh: Includes all visible stars.")]
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

	private void UpdateScattering() {
		if (Atmosphere == null)
			return;
		var num1 = -Atmosphere.Directionality;
		var num2 = num1 * num1;
		kBetaMie.x = (float)(1.5 * ((1.0 - num2) / (2.0 + num2)));
		kBetaMie.y = 1f + num2;
		kBetaMie.z = 2f * num1;
		var num3 = 1f / 500f * Atmosphere.MieMultiplier;
		var num4 = 1f / 500f * Atmosphere.RayleighMultiplier;
		var num5 = (float)(num4 * 40.0 * 5.2701644897460938);
		var num6 = (float)(num4 * 40.0 * 9.4732837677001953);
		var num7 = (float)(num4 * 40.0 * 19.643802642822266);
		var num8 = num3 * 40f;
		kSun.x = num5;
		kSun.y = num6;
		kSun.z = num7;
		kSun.w = num8;
		var num9 = (float)(num4 * 4.0 * 3.1415927410125732 * 5.2701644897460938);
		var num10 = (float)(num4 * 4.0 * 3.1415927410125732 * 9.4732837677001953);
		var num11 = (float)(num4 * 4.0 * 3.1415927410125732 * 19.643802642822266);
		var num12 = (float)(num3 * 4.0 * 3.1415927410125732);
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

	private void UpdateCelestials() {
		if (World == null || Components == null || Components.SpaceTransform == null)
			return;
		var f1 = (float)Math.PI / 180f * World.Latitude;
		var num1 = Mathf.Sin(f1);
		var num2 = Mathf.Cos(f1);
		var longitude = World.Longitude;
		var num3 = 1.57079637f;
		var year = Cycle.Year;
		var month = Cycle.Month;
		var day = Cycle.Day;
		var num4 = Cycle.Hour - World.UTC;
		var num5 = 367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530 + num4 / 24f;
		var num6 = 367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530 + 0.5f;
		var f2 = (float)Math.PI / 180f * (float)(23.439300537109375 - 3.5630000638775527E-07 * num5);
		var num7 = Mathf.Sin(f2);
		var num8 = Mathf.Cos(f2);
		var num9 = (float)(282.94039916992188 + 4.7093501052586362E-05 * num6);
		var num10 = (float)(0.016708999872207642 - 1.1509999620074041E-09 * num6);
		var f3 = (float)Math.PI / 180f * (float)(356.0469970703125 + 0.98560023307800293 * num6);
		var num11 = Mathf.Sin(f3);
		var num12 = Mathf.Cos(f3);
		var f4 = f3 + (float)(num10 * (double)num11 * (1.0 + num10 * (double)num12));
		var num13 = Mathf.Sin(f4);
		var x1 = Mathf.Cos(f4) - num10;
		var y1 = Mathf.Sqrt((float)(1.0 - num10 * (double)num10)) * num13;
		var num14 = 57.29578f * Mathf.Atan2(y1, x1);
		var num15 = Mathf.Sqrt((float)(x1 * (double)x1 + y1 * (double)y1));
		var f5 = (float)Math.PI / 180f * (num14 + num9);
		var num16 = Mathf.Sin(f5);
		var num17 = Mathf.Cos(f5);
		var num18 = num15 * num17;
		var num19 = num15 * num16;
		var x2 = num18;
		var y2 = num19 * num8;
		var y3 = num19 * num7;
		var num20 = 57.29578f * Mathf.Atan2(y2, x2);
		var f6 = Mathf.Atan2(y3, Mathf.Sqrt((float)(x2 * (double)x2 + y2 * (double)y2)));
		var num21 = Mathf.Sin(f6);
		var num22 = Mathf.Cos(f6);
		var num23 = num14 + num9 + 180f;
		var num24 = num20 - num23 - longitude;
		var num25 = 57.29578f * Mathf.Acos((float)((Mathf.Sin((float)Math.PI / 180f * -6f) - num1 * (double)num21) /
		                                           (num2 * (double)num22)));
		SunsetTime = (float)((24.0 + (num24 + (double)num25) / 15.0 % 24.0) % 24.0);
		SunriseTime = (float)((24.0 + (num24 - (double)num25) / 15.0 % 24.0) % 24.0);
		var num26 = (float)(282.94039916992188 + 4.7093501052586362E-05 * num5);
		var num27 = (float)(0.016708999872207642 - 1.1509999620074041E-09 * num5);
		var f7 = (float)Math.PI / 180f * (float)(356.0469970703125 + 0.98560023307800293 * num5);
		var num28 = Mathf.Sin(f7);
		var num29 = Mathf.Cos(f7);
		var f8 = f7 + (float)(num27 * (double)num28 * (1.0 + num27 * (double)num29));
		var num30 = Mathf.Sin(f8);
		var x3 = Mathf.Cos(f8) - num27;
		var y4 = Mathf.Sqrt((float)(1.0 - num27 * (double)num27)) * num30;
		var num31 = 57.29578f * Mathf.Atan2(y4, x3);
		var num32 = Mathf.Sqrt((float)(x3 * (double)x3 + y4 * (double)y4));
		var f9 = (float)Math.PI / 180f * (num31 + num26);
		var num33 = Mathf.Sin(f9);
		var num34 = Mathf.Cos(f9);
		var num35 = num32 * num34;
		var num36 = num32 * num33;
		var x4 = num35;
		var y5 = num36 * num8;
		var y6 = num36 * num7;
		var num37 = Mathf.Atan2(y5, x4);
		var f10 = Mathf.Atan2(y6, Mathf.Sqrt((float)(x4 * (double)x4 + y5 * (double)y5)));
		var num38 = Mathf.Sin(f10);
		var num39 = Mathf.Cos(f10);
		var num40 = num31 + num26 + 180f + 15f * num4;
		var num41 = (float)(Math.PI / 180.0 * (num40 + (double)longitude));
		LocalSiderealTime = (float)((num40 + (double)longitude) / 15.0);
		var f11 = num41 - num37;
		var num42 = Mathf.Sin(f11);
		var num43 = Mathf.Cos(f11) * num39;
		var num44 = num42 * num39;
		var num45 = num38;
		var x5 = (float)(num43 * (double)num1 - num45 * (double)num2);
		var y7 = num44;
		var y8 = (float)(num43 * (double)num2 + num45 * (double)num1);
		var num46 = Mathf.Atan2(y7, x5) + 3.14159274f;
		var num47 = Mathf.Atan2(y8, Mathf.Sqrt((float)(x5 * (double)x5 + y7 * (double)y7)));
		var num48 = num3 - num47;
		var num49 = num47;
		var phi1 = num46;
		SunZenith = 57.29578f * num48;
		SunAltitude = 57.29578f * num49;
		SunAzimuth = 57.29578f * phi1;
		float num50;
		float num51;
		float phi2;
		if (Moon.Position == TOD_MoonPositionType.Realistic) {
			var num52 = (float)(125.122802734375 - 0.0529538094997406 * num5);
			var num53 = 5.1454f;
			var num54 = (float)(318.06338500976563 + 0.16435731947422028 * num5);
			var num55 = 60.2666f;
			var num56 = 0.0549f;
			var num57 = (float)(115.36540222167969 + 13.064992904663086 * num5);
			var f12 = (float)Math.PI / 180f * num52;
			var num58 = Mathf.Sin(f12);
			var num59 = Mathf.Cos(f12);
			var f13 = (float)Math.PI / 180f * num53;
			var num60 = Mathf.Sin(f13);
			var num61 = Mathf.Cos(f13);
			var f14 = (float)Math.PI / 180f * num57;
			var num62 = Mathf.Sin(f14);
			var num63 = Mathf.Cos(f14);
			var f15 = f14 + (float)(num56 * (double)num62 * (1.0 + num56 * (double)num63));
			var num64 = Mathf.Sin(f15);
			var num65 = Mathf.Cos(f15);
			var x6 = num55 * (num65 - num56);
			var y9 = num55 * (Mathf.Sqrt((float)(1.0 - num56 * (double)num56)) * num64);
			var num66 = 57.29578f * Mathf.Atan2(y9, x6);
			var num67 = Mathf.Sqrt((float)(x6 * (double)x6 + y9 * (double)y9));
			var f16 = (float)Math.PI / 180f * (num66 + num54);
			var num68 = Mathf.Sin(f16);
			var num69 = Mathf.Cos(f16);
			var num70 = num67 * (float)(num59 * (double)num69 - num58 * (double)num68 * num61);
			var num71 = num67 * (float)(num58 * (double)num69 + num59 * (double)num68 * num61);
			var num72 = num67 * (num68 * num60);
			var num73 = num70;
			var num74 = num71;
			var num75 = num72;
			var x7 = num73;
			var y10 = (float)(num74 * (double)num8 - num75 * (double)num7);
			var y11 = (float)(num74 * (double)num7 + num75 * (double)num8);
			var num76 = Mathf.Atan2(y10, x7);
			var f17 = Mathf.Atan2(y11, Mathf.Sqrt((float)(x7 * (double)x7 + y10 * (double)y10)));
			var num77 = Mathf.Sin(f17);
			var num78 = Mathf.Cos(f17);
			var f18 = num41 - num76;
			var num79 = Mathf.Sin(f18);
			var num80 = Mathf.Cos(f18) * num78;
			var num81 = num79 * num78;
			var num82 = num77;
			var x8 = (float)(num80 * (double)num1 - num82 * (double)num2);
			var y12 = num81;
			var y13 = (float)(num80 * (double)num2 + num82 * (double)num1);
			var num83 = Mathf.Atan2(y12, x8) + 3.14159274f;
			var num84 = Mathf.Atan2(y13, Mathf.Sqrt((float)(x8 * (double)x8 + y12 * (double)y12)));
			num50 = num3 - num84;
			num51 = num84;
			phi2 = num83;
		} else {
			num50 = num48 - 3.14159274f;
			num51 = num49 - 3.14159274f;
			phi2 = phi1;
		}

		MoonZenith = 57.29578f * num50;
		MoonAltitude = 57.29578f * num51;
		MoonAzimuth = 57.29578f * phi2;
		var quaternion = Quaternion.Euler(90f - World.Latitude, 0.0f, 0.0f) *
		                 Quaternion.Euler(0.0f, World.Longitude, 0.0f) *
		                 Quaternion.Euler(0.0f, num41 * 57.29578f, 0.0f);
		if (Stars.Position == TOD_StarsPositionType.Rotating) {
			Components.SpaceTransform.localRotation = quaternion;
			Components.StarTransform.localRotation = quaternion;
		} else {
			Components.SpaceTransform.localRotation = Quaternion.identity;
			Components.StarTransform.localRotation = Quaternion.identity;
		}

		Components.SunTransform.localPosition = OrbitalToLocal(num48, phi1);
		Components.SunTransform.LookAt(Components.DomeTransform.position, Components.SunTransform.up);
		var local = OrbitalToLocal(num50, phi2);
		var worldUp = quaternion * -Vector3.right;
		Components.MoonTransform.localPosition = local;
		Components.MoonTransform.LookAt(Components.DomeTransform.position, worldUp);
		var num85 = 8f * Mathf.Tan((float)Math.PI / 360f * Sun.MeshSize);
		Components.SunTransform.localScale = new Vector3(num85, num85, num85);
		var num86 = 4f * Mathf.Tan((float)Math.PI / 360f * Moon.MeshSize);
		Components.MoonTransform.localScale = new Vector3(num86, num86, num86);
		var flag = (1.0 - Atmosphere.Fogginess) * (1.0 - LerpValue) > 0.0;
		Components.SpaceRenderer.enabled = flag;
		Components.StarRenderer.enabled = flag;
		Components.SunRenderer.enabled = Components.SunTransform.localPosition.y > -(double)num85;
		Components.MoonRenderer.enabled = Components.MoonTransform.localPosition.y > -(double)num86;
		Components.AtmosphereRenderer.enabled = true;
		Components.ClearRenderer.enabled = Components.Rays != null;
		Components.CloudRenderer.enabled = Clouds.Coverage > 0.0 && Clouds.Opacity > 0.0;
		LerpValue = Mathf.InverseLerp(105f, 90f, SunZenith);
		var time1 = Mathf.Clamp01(SunZenith / 90f);
		var time2 = Mathf.Clamp01((float)((SunZenith - 90.0) / 90.0));
		var t = time2 <= 0.0 ? time1 : 1f - time2;
		Atmosphere.RayleighMultiplier = Mathf.Lerp(Day.RayleighMultiplier, Night.RayleighMultiplier, t);
		Atmosphere.MieMultiplier = Mathf.Lerp(Day.MieMultiplier, Night.MieMultiplier, t);
		Atmosphere.Brightness = Mathf.Lerp(Day.Brightness, Night.Brightness, t);
		Atmosphere.Contrast = Mathf.Max(1E-06f, Mathf.Lerp(Day.Contrast, Night.Contrast, t));
		Atmosphere.Directionality = Mathf.Lerp(Day.Directionality, Night.Directionality, t);
		Atmosphere.Fogginess = Mathf.Lerp(Day.Fogginess, Night.Fogginess, t);
		var num87 = Mathf.Clamp01((float)((LerpValue - 0.10000000149011612) / 0.89999997615814209));
		var num88 = Mathf.Clamp01((float)((0.10000000149011612 - LerpValue) / 0.10000000149011612));
		var num89 = Mathf.Clamp01((float)((90.0 - num50 * 57.295780181884766) / 5.0));
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
		var b1 = TOD_Util.ApplyAlpha(Day.FogColor.Evaluate(time1));
		FogColor = Color.Lerp(TOD_Util.ApplyAlpha(Night.FogColor.Evaluate(time2)), b1, LerpValue);
		var b2 = TOD_Util.ApplyAlpha(Day.AmbientColor.Evaluate(time1));
		var a = TOD_Util.ApplyAlpha(Night.AmbientColor.Evaluate(time2));
		AmbientColor = Color.Lerp(a, b2, LerpValue);
		var b3 = b2;
		GroundColor = Color.Lerp(a, b3, LerpValue);
		MoonHaloColor = TOD_Util.MulRGB(MoonSkyColor, Moon.HaloBrightness * num89);
		float shadowStrength;
		float num90;
		Color color;
		if (LerpValue > 0.10000000149011612) {
			IsDay = true;
			IsNight = false;
			shadowStrength = Day.ShadowStrength;
			num90 = Mathf.Lerp(0.0f, Day.LightIntensity, SunVisibility);
			color = SunLightColor;
			if (Resources.CloudDensityTexture != null) {
				var num91 = TOD_Clouds.Instance.LightAttenuation(Resources.CloudDensityTexture,
					OrbitalToLocal(num48, phi1), 4f * Mathf.Tan((float)Math.PI / 360f * Sun.MeshSize));
				num90 *= (float)(0.25 + num91 * 0.75);
				var component = Components.LightSource.GetComponent<NGSS_Directional>();
				if (component != null)
					component.NGSS_SOFTNESS = (float)(2.0 - num91 * 1.75);
			}
		} else {
			IsDay = false;
			IsNight = true;
			shadowStrength = Night.ShadowStrength;
			num90 = Mathf.Lerp(0.0f, Night.LightIntensity, MoonVisibility);
			color = MoonLightColor;
			if (Resources.CloudDensityTexture != null) {
				var num92 = TOD_Clouds.Instance.LightAttenuation(Resources.CloudDensityTexture,
					OrbitalToLocal(num50, phi2), 4f * Mathf.Tan((float)Math.PI / 360f * Moon.MeshSize));
				num90 *= num92;
				var component = Components.LightSource.GetComponent<NGSS_Directional>();
				if (component != null)
					component.NGSS_SOFTNESS = (float)(2.0 - num92 * 1.75);
			}
		}

		Components.LightSource.color = color;
		Components.LightSource.intensity = num90;
		Components.LightSource.shadowStrength = shadowStrength;
		Components.LightSource.enabled = num90 > 0.0 && color != Color.black;
		if (!Application.isPlaying || timeSinceLightUpdate >= (double)Light.UpdateInterval) {
			timeSinceLightUpdate = 0.0f;
			Components.LightTransform.localPosition = IsNight
				? OrbitalToLocal(Mathf.Min(num50, (float)((1.0 - Light.MinimumHeight) * 3.1415927410125732 / 2.0)),
					phi2)
				: OrbitalToLocal(Mathf.Min(num48, (float)((1.0 - Light.MinimumHeight) * 3.1415927410125732 / 2.0)),
					phi1);
			Components.LightTransform.LookAt(Components.DomeTransform.position);
		} else
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

	public Vector3 OrbitalToUnity(float radius, float theta, float phi) {
		var num1 = Mathf.Sin(theta);
		var num2 = Mathf.Cos(theta);
		var num3 = Mathf.Sin(phi);
		var num4 = Mathf.Cos(phi);
		Vector3 unity;
		unity.z = radius * num1 * num4;
		unity.y = radius * num2;
		unity.x = radius * num1 * num3;
		return unity;
	}

	public Vector3 OrbitalToLocal(float theta, float phi) {
		var num1 = Mathf.Sin(theta);
		var num2 = Mathf.Cos(theta);
		var num3 = Mathf.Sin(phi);
		var num4 = Mathf.Cos(phi);
		Vector3 local;
		local.z = num1 * num4;
		local.y = num2;
		local.x = num1 * num3;
		return local;
	}

	public Color SampleAtmosphere(Vector3 direction, bool directLight = true) {
		return TOD_LINEAR2GAMMA(TOD_HDR2LDR(
			ShaderScatteringColor(Components.DomeTransform.InverseTransformDirection(direction), directLight)));
	}

	public SphericalHarmonicsL2 RenderToSphericalHarmonics() {
		var sphericalHarmonics = new SphericalHarmonicsL2();
		var directLight = false;
		var color1 = TOD_Util.ChangeSaturation(AmbientColor.linear, Ambient.Saturation);
		var vector3 = new Vector3(0.612372458f, 0.5f, 0.612372458f);
		var up = Vector3.up;
		var linear = SampleAtmosphere(up, directLight).linear;
		sphericalHarmonics.AddDirectionalLight(up, linear, 0.428571433f);
		var direction1 = new Vector3(-vector3.x, vector3.y, -vector3.z);
		var color2 = TOD_Util.ChangeSaturation(SampleAtmosphere(direction1, directLight).linear, Ambient.Saturation);
		sphericalHarmonics.AddDirectionalLight(direction1, color2, 0.2857143f);
		var direction2 = new Vector3(vector3.x, vector3.y, -vector3.z);
		var color3 = TOD_Util.ChangeSaturation(SampleAtmosphere(direction2, directLight).linear, Ambient.Saturation);
		sphericalHarmonics.AddDirectionalLight(direction2, color3, 0.2857143f);
		var direction3 = new Vector3(-vector3.x, vector3.y, vector3.z);
		var color4 = TOD_Util.ChangeSaturation(SampleAtmosphere(direction3, directLight).linear, Ambient.Saturation);
		sphericalHarmonics.AddDirectionalLight(direction3, color4, 0.2857143f);
		var direction4 = new Vector3(vector3.x, vector3.y, vector3.z);
		var color5 = TOD_Util.ChangeSaturation(SampleAtmosphere(direction4, directLight).linear, Ambient.Saturation);
		sphericalHarmonics.AddDirectionalLight(direction4, color5, 0.2857143f);
		var left = Vector3.left;
		var color6 = TOD_Util.ChangeSaturation(SampleAtmosphere(left, directLight).linear, Ambient.Saturation);
		sphericalHarmonics.AddDirectionalLight(left, color6, 0.142857149f);
		var right = Vector3.right;
		var color7 = TOD_Util.ChangeSaturation(SampleAtmosphere(right, directLight).linear, Ambient.Saturation);
		sphericalHarmonics.AddDirectionalLight(right, color7, 0.142857149f);
		var back = Vector3.back;
		var color8 = TOD_Util.ChangeSaturation(SampleAtmosphere(back, directLight).linear, Ambient.Saturation);
		sphericalHarmonics.AddDirectionalLight(back, color8, 0.142857149f);
		var forward = Vector3.forward;
		var color9 = TOD_Util.ChangeSaturation(SampleAtmosphere(forward, directLight).linear, Ambient.Saturation);
		sphericalHarmonics.AddDirectionalLight(forward, color9, 0.142857149f);
		var direction5 = new Vector3(-vector3.x, -vector3.y, -vector3.z);
		sphericalHarmonics.AddDirectionalLight(direction5, color1, 0.2857143f);
		var direction6 = new Vector3(vector3.x, -vector3.y, -vector3.z);
		sphericalHarmonics.AddDirectionalLight(direction6, color1, 0.2857143f);
		var direction7 = new Vector3(-vector3.x, -vector3.y, vector3.z);
		sphericalHarmonics.AddDirectionalLight(direction7, color1, 0.2857143f);
		var direction8 = new Vector3(vector3.x, -vector3.y, vector3.z);
		sphericalHarmonics.AddDirectionalLight(direction8, color1, 0.2857143f);
		var down = Vector3.down;
		sphericalHarmonics.AddDirectionalLight(down, color1, 0.428571433f);
		return sphericalHarmonics;
	}

	public void RenderToCubemap(RenderTexture targetTexture = null) {
		if (!(bool)(Object)Probe) {
			Probe = new GameObject().AddComponent<ReflectionProbe>();
			Probe.name = gameObject.name + " Reflection Probe";
			Probe.mode = ReflectionProbeMode.Realtime;
		}

		if (probeRenderID >= 0 && !Probe.IsFinishedRendering(probeRenderID))
			return;
		var maxValue = float.MaxValue;
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

	public Color SampleFogColor(bool directLight = true) {
		var a = Vector3.forward;
		if (Components.Camera != null)
			a = Quaternion.Euler(0.0f, Components.Camera.transform.rotation.eulerAngles.y, 0.0f) * a;
		var color = SampleAtmosphere(Vector3.Lerp(a, Vector3.up, Fog.HeightBias).normalized, directLight);
		return new Color(color.r, color.g, color.b, 1f);
	}

	public Color SampleSkyColor() {
		var sunDirection = SunDirection;
		sunDirection.y = Mathf.Abs(sunDirection.y);
		var color = SampleAtmosphere(sunDirection.normalized, false);
		return new Color(color.r, color.g, color.b, 1f);
	}

	public Color SampleEquatorColor() {
		var color = SampleAtmosphere((SunDirection with {
			y = 0.0f
		}).normalized, false);
		return new Color(color.r, color.g, color.b, 1f);
	}

	public void UpdateFog() {
		if (Fog == null)
			return;
		switch (Fog.Mode) {
			case TOD_FogType.Color:
				RenderSettings.fogColor = SampleFogColor(false);
				break;
			case TOD_FogType.Directional:
				RenderSettings.fogColor = SampleFogColor();
				break;
		}

		if (Components.Camera == null)
			return;
		var scattering = Components.Scattering;
		if (scattering == null)
			return;
		scattering.GlobalDensity = Fog.GlobalDensity;
		scattering.StartDistance = Fog.StartDistance;
		scattering.TransparentDensityMultiplier = Fog.TransparentDensityMultiplier;
	}

	public void UpdateAmbient() {
		if (Ambient == null)
			return;
		var saturation = Ambient.Saturation;
		var num = Mathf.Lerp(Night.AmbientMultiplier, Day.AmbientMultiplier, LerpValue);
		var color1 = TOD_Util.ChangeSaturation(AmbientColor, Ambient.Saturation);
		switch (Ambient.Mode) {
			case TOD_AmbientType.Color:
				Shader.SetGlobalInt("Pathologic_Ambient", 0);
				RenderSettings.ambientMode = AmbientMode.Flat;
				RenderSettings.ambientLight = color1;
				RenderSettings.ambientIntensity = num;
				break;
			case TOD_AmbientType.Gradient:
				Shader.SetGlobalInt("Pathologic_Ambient", 1);
				var color2 = (TOD_Util.ChangeSaturation(SampleEquatorColor(), saturation) + color1) * 0.5f;
				var color3 = TOD_Util.ChangeSaturation(SampleSkyColor(), saturation);
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

	public void UpdateReflection() {
		if (Reflection.Mode != TOD_ReflectionType.Cubemap)
			return;
		var num = Mathf.Lerp(Night.ReflectionMultiplier, Day.ReflectionMultiplier, LerpValue);
		RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
		RenderSettings.reflectionIntensity = num;
		if (!Application.isPlaying)
			return;
		RenderToCubemap();
	}

	public void LoadParameters(string xml) {
		(new XmlSerializer(typeof(TOD_Parameters)).Deserialize(new XmlTextReader(new StringReader(xml))) as
			TOD_Parameters).ToSky(this);
	}

	private void UpdateQualitySettings() {
		if (Headless)
			return;
		Mesh mesh1 = null;
		Mesh mesh2 = null;
		Mesh mesh3 = null;
		Mesh mesh4 = null;
		Mesh mesh5 = null;
		Mesh mesh6 = null;
		switch (MeshQuality) {
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

		switch (StarQuality) {
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

		if ((bool)(Object)Components.SpaceMeshFilter && Components.SpaceMeshFilter.sharedMesh != mesh1)
			Components.SpaceMeshFilter.mesh = mesh1;
		if ((bool)(Object)Components.MoonMeshFilter && Components.MoonMeshFilter.sharedMesh != mesh5)
			Components.MoonMeshFilter.mesh = mesh5;
		if ((bool)(Object)Components.AtmosphereMeshFilter && Components.AtmosphereMeshFilter.sharedMesh != mesh2)
			Components.AtmosphereMeshFilter.mesh = mesh2;
		if ((bool)(Object)Components.ClearMeshFilter && Components.ClearMeshFilter.sharedMesh != mesh3)
			Components.ClearMeshFilter.mesh = mesh3;
		if ((bool)(Object)Components.CloudMeshFilter && Components.CloudMeshFilter.sharedMesh != mesh4)
			Components.CloudMeshFilter.mesh = mesh4;
		if (!(bool)(Object)Components.StarMeshFilter || !(Components.StarMeshFilter.sharedMesh != mesh6))
			return;
		Components.StarMeshFilter.mesh = mesh6;
	}

	private void UpdateRenderSettings() {
		if (Headless)
			return;
		UpdateFog();
		if (!Application.isPlaying || timeSinceAmbientUpdate >= (double)Ambient.UpdateInterval) {
			timeSinceAmbientUpdate = 0.0f;
			UpdateAmbient();
		} else
			timeSinceAmbientUpdate += Time.deltaTime;

		if (!Application.isPlaying || timeSinceReflectionUpdate >= (double)Reflection.UpdateInterval) {
			timeSinceReflectionUpdate = 0.0f;
			UpdateReflection();
		} else
			timeSinceReflectionUpdate += Time.deltaTime;
	}

	private void UpdateShaderKeywords() {
		if (Headless)
			return;
		switch (ColorSpace) {
			case TOD_ColorSpaceType.Auto:
				if (QualitySettings.activeColorSpace == UnityEngine.ColorSpace.Linear) {
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

		switch (ColorRange) {
			case TOD_ColorRangeType.Auto:
				if ((bool)(Object)Components.Camera && Components.Camera.HDR) {
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

		switch (ColorOutput) {
			case TOD_ColorOutputType.Raw:
				Shader.DisableKeyword("TOD_OUTPUT_DITHERING");
				break;
			case TOD_ColorOutputType.Dithered:
				Shader.EnableKeyword("TOD_OUTPUT_DITHERING");
				break;
		}

		switch (SkyQuality) {
			case TOD_SkyQualityType.PerVertex:
				Shader.DisableKeyword("TOD_SCATTERING_PER_PIXEL");
				break;
			case TOD_SkyQualityType.PerPixel:
				Shader.EnableKeyword("TOD_SCATTERING_PER_PIXEL");
				break;
		}

		switch (CloudQuality) {
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

	private void UpdateShaderProperties() {
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
		var num1 = Mathf.Lerp(0.8f, 0.0f, Clouds.Coverage);
		var num2 = Mathf.Lerp(3f, 9f, Clouds.Sharpness);
		var num3 = Mathf.Lerp(0.0f, 1f, Clouds.Attenuation);
		var num4 = Mathf.Lerp(0.0f, 2f, Clouds.Saturation);
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
		Shader.SetGlobalFloat(Resources.ID_StarVisibility, (float)((1.0 - Atmosphere.Fogginess) * (1.0 - LerpValue)));
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

	private float ShaderScale(float inCos) {
		var num = 1f - inCos;
		return 0.25f *
		       Mathf.Exp((float)(num * (0.45899999141693115 +
		                                num * (3.8299999237060547 + num * (num * 5.25 - 6.8000001907348633))) -
		                         0.0028699999675154686));
	}

	private float ShaderMiePhase(float eyeCos, float eyeCos2) {
		return kBetaMie.x * (1f + eyeCos2) / Mathf.Pow(kBetaMie.y + kBetaMie.z * eyeCos, 1.5f);
	}

	private float ShaderRayleighPhase(float eyeCos2) {
		return (float)(0.75 + 0.75 * eyeCos2);
	}

	private Color ShaderNightSkyColor(Vector3 dir) {
		dir.y = Mathf.Max(0.0f, dir.y);
		return MoonSkyColor * (float)(1.0 - 0.75 * dir.y);
	}

	private Color ShaderMoonHaloColor(Vector3 dir) {
		return MoonHaloColor * Mathf.Pow(Mathf.Max(0.0f, Vector3.Dot(dir, LocalMoonDirection)), 1f / Moon.MeshSize);
	}

	private Color TOD_HDR2LDR(Color color) {
		return new Color(1f - Mathf.Pow(2f, -Atmosphere.Brightness * color.r),
			1f - Mathf.Pow(2f, -Atmosphere.Brightness * color.g), 1f - Mathf.Pow(2f, -Atmosphere.Brightness * color.b),
			color.a);
	}

	private Color TOD_GAMMA2LINEAR(Color color) {
		return new Color(color.r * color.r, color.g * color.g, color.b * color.b, color.a);
	}

	private Color TOD_LINEAR2GAMMA(Color color) {
		return new Color(Mathf.Sqrt(color.r), Mathf.Sqrt(color.g), Mathf.Sqrt(color.b), color.a);
	}

	private Color ShaderScatteringColor(Vector3 dir, bool directLight = true) {
		dir.y = Mathf.Max(0.0f, dir.y);
		var x1 = kRadius.x;
		var y1 = kRadius.y;
		var w1 = kRadius.w;
		var x2 = kScale.x;
		var z1 = kScale.z;
		var w2 = kScale.w;
		var x3 = k4PI.x;
		var y2 = k4PI.y;
		var z2 = k4PI.z;
		var w3 = k4PI.w;
		var x4 = kSun.x;
		var y3 = kSun.y;
		var z3 = kSun.z;
		var w4 = kSun.w;
		var rhs1 = new Vector3(0.0f, x1 + w2, 0.0f);
		var num1 = Mathf.Sqrt(w1 + y1 * dir.y * dir.y - y1) - x1 * dir.y;
		var num2 = Mathf.Exp(z1 * -w2) * ShaderScale(Vector3.Dot(dir, rhs1) / (x1 + w2));
		var num3 = num1 / 2f;
		var num4 = num3 * x2;
		var vector3 = dir * num3;
		var rhs2 = rhs1 + vector3 * 0.5f;
		var num5 = 0.0f;
		var num6 = 0.0f;
		var num7 = 0.0f;
		for (var index = 0; index < 2; ++index) {
			var magnitude = rhs2.magnitude;
			var num8 = 1f / magnitude;
			var num9 = Mathf.Exp(z1 * (x1 - magnitude));
			var num10 = num9 * num4;
			var inCos1 = Vector3.Dot(dir, rhs2) * num8;
			var inCos2 = Vector3.Dot(LocalSunDirection, rhs2) * num8;
			var num11 = num2 + num9 * (ShaderScale(inCos2) - ShaderScale(inCos1));
			var num12 = Mathf.Exp((float)(-(double)num11 * (x3 + (double)w3)));
			var num13 = Mathf.Exp((float)(-(double)num11 * (y2 + (double)w3)));
			var num14 = Mathf.Exp((float)(-(double)num11 * (z2 + (double)w3)));
			num5 += num12 * num10;
			num6 += num13 * num10;
			num7 += num14 * num10;
			rhs2 += vector3;
		}

		var num15 = SunSkyColor.r * num5 * x4;
		var num16 = SunSkyColor.g * num6 * y3;
		var num17 = SunSkyColor.b * num7 * z3;
		var num18 = SunSkyColor.r * num5 * w4;
		var num19 = SunSkyColor.g * num6 * w4;
		var num20 = SunSkyColor.b * num7 * w4;
		var num21 = 0.0f;
		var num22 = 0.0f;
		var num23 = 0.0f;
		var eyeCos = Vector3.Dot(LocalSunDirection, dir);
		var eyeCos2 = eyeCos * eyeCos;
		var num24 = ShaderRayleighPhase(eyeCos2);
		var num25 = num21 + num24 * num15;
		var num26 = num22 + num24 * num16;
		var num27 = num23 + num24 * num17;
		if (directLight) {
			var num28 = ShaderMiePhase(eyeCos, eyeCos2);
			num25 += num28 * num18;
			num26 += num28 * num19;
			num27 += num28 * num20;
		}

		var color1 = ShaderNightSkyColor(dir);
		var a1 = num25 + color1.r;
		var a2 = num26 + color1.g;
		var a3 = num27 + color1.b;
		if (directLight) {
			var color2 = ShaderMoonHaloColor(dir);
			a1 += color2.r;
			a2 += color2.g;
			a3 += color2.b;
		}

		var num29 = Mathf.Lerp(a1, FogColor.r, Atmosphere.Fogginess);
		var num30 = Mathf.Lerp(a2, FogColor.g, Atmosphere.Fogginess);
		var num31 = Mathf.Lerp(a3, FogColor.b, Atmosphere.Fogginess);
		return new Color(Mathf.Pow(num29 * Atmosphere.Brightness, Atmosphere.Contrast),
			Mathf.Pow(num30 * Atmosphere.Brightness, Atmosphere.Contrast),
			Mathf.Pow(num31 * Atmosphere.Brightness, Atmosphere.Contrast), 1f);
	}

	private void Initialize() {
		Components = GetComponent<TOD_Components>();
		Components.Initialize();
		Resources = GetComponent<TOD_Resources>();
		Resources.Initialize();
		instances.Add(this);
		Initialized = true;
	}

	private void Cleanup() {
		if ((bool)(Object)Probe)
			Destroy(Probe.gameObject);
		Shader.SetGlobalInt("Pathologic_Ambient", 0);
		instances.Remove(this);
		Initialized = false;
	}

	protected void OnEnable() {
		LateUpdate();
	}

	protected void OnDisable() {
		Cleanup();
	}

	protected void LateUpdate() {
		if (!Initialized)
			Initialize();
		UpdateCelestials();
		UpdateScattering();
		UpdateQualitySettings();
		UpdateRenderSettings();
		UpdateShaderKeywords();
		UpdateShaderProperties();
	}

	protected void OnValidate() {
		if (Cycle == null)
			return;
		Cycle.DateTime = Cycle.DateTime;
	}
}