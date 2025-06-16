using Engine.Common.Services;
using Engine.Drawing.Gradient;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;

namespace Engine.Proxy.Weather.Element;

public class DayUtility {
	public static void CopyTo(Day day) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		GradientUtility.Copy(tod.Day.SunColor, day.SunColor);
		GradientUtility.Copy(tod.Day.LightColor, day.LightColor);
		GradientUtility.Copy(tod.Day.RayColor, day.RayColor);
		GradientUtility.Copy(tod.Day.SkyColor, day.SkyColor);
		GradientUtility.Copy(tod.Day.CloudColor, day.CloudColor);
		GradientUtility.Copy(tod.Day.FogColor, day.FogColor);
		GradientUtility.Copy(tod.Day.AmbientColor, day.AmbientColor);
		day.RayleighMultiplier = tod.Day.RayleighMultiplier;
		day.MieMultiplier = tod.Day.MieMultiplier;
		day.Brightness = tod.Day.Brightness;
		day.Contrast = tod.Day.Contrast;
		day.Directionality = tod.Day.Directionality;
		day.Fogginess = tod.Day.Fogginess;
		day.LightIntensity = tod.Day.LightIntensity;
		day.ShadowStrength = tod.Day.ShadowStrength;
		day.ReflectionMultiplier = tod.Day.ReflectionMultiplier;
	}

	public static void CopyFrom(Day day) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		GradientUtility.Copy(day.SunColor, tod.Day.SunColor);
		GradientUtility.Copy(day.LightColor, tod.Day.LightColor);
		GradientUtility.Copy(day.RayColor, tod.Day.RayColor);
		GradientUtility.Copy(day.SkyColor, tod.Day.SkyColor);
		GradientUtility.Copy(day.CloudColor, tod.Day.CloudColor);
		GradientUtility.Copy(day.FogColor, tod.Day.FogColor);
		GradientUtility.Copy(day.AmbientColor, tod.Day.AmbientColor);
		tod.Day.RayleighMultiplier = day.RayleighMultiplier;
		tod.Day.MieMultiplier = day.MieMultiplier;
		tod.Day.Brightness = day.Brightness;
		tod.Day.Contrast = day.Contrast;
		tod.Day.Directionality = day.Directionality;
		tod.Day.Fogginess = day.Fogginess;
		tod.Day.LightIntensity = day.LightIntensity;
		tod.Day.ShadowStrength = day.ShadowStrength;
		tod.Day.ReflectionMultiplier = day.ReflectionMultiplier;
	}
}