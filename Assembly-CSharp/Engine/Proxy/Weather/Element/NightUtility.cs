using Engine.Common.Services;
using Engine.Drawing.Gradient;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;

namespace Engine.Proxy.Weather.Element;

public class NightUtility {
	public static void CopyTo(Night night) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		GradientUtility.Copy(tod.Night.MoonColor, night.MoonColor);
		GradientUtility.Copy(tod.Night.LightColor, night.LightColor);
		GradientUtility.Copy(tod.Night.RayColor, night.RayColor);
		GradientUtility.Copy(tod.Night.SkyColor, night.SkyColor);
		GradientUtility.Copy(tod.Night.CloudColor, night.CloudColor);
		GradientUtility.Copy(tod.Night.FogColor, night.FogColor);
		GradientUtility.Copy(tod.Night.AmbientColor, night.AmbientColor);
		night.RayleighMultiplier = tod.Night.RayleighMultiplier;
		night.MieMultiplier = tod.Night.MieMultiplier;
		night.Brightness = tod.Night.Brightness;
		night.Contrast = tod.Night.Contrast;
		night.Directionality = tod.Night.Directionality;
		night.Fogginess = tod.Night.Fogginess;
		night.LightIntensity = tod.Night.LightIntensity;
		night.ShadowStrength = tod.Night.ShadowStrength;
		night.ReflectionMultiplier = tod.Night.ReflectionMultiplier;
	}

	public static void CopyFrom(Night night) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		GradientUtility.Copy(night.MoonColor, tod.Night.MoonColor);
		GradientUtility.Copy(night.LightColor, tod.Night.LightColor);
		GradientUtility.Copy(night.RayColor, tod.Night.RayColor);
		GradientUtility.Copy(night.SkyColor, tod.Night.SkyColor);
		GradientUtility.Copy(night.CloudColor, tod.Night.CloudColor);
		GradientUtility.Copy(night.FogColor, tod.Night.FogColor);
		GradientUtility.Copy(night.AmbientColor, tod.Night.AmbientColor);
		tod.Night.RayleighMultiplier = night.RayleighMultiplier;
		tod.Night.MieMultiplier = night.MieMultiplier;
		tod.Night.Brightness = night.Brightness;
		tod.Night.Contrast = night.Contrast;
		tod.Night.Directionality = night.Directionality;
		tod.Night.Fogginess = night.Fogginess;
		tod.Night.LightIntensity = night.LightIntensity;
		tod.Night.ShadowStrength = night.ShadowStrength;
		tod.Night.ReflectionMultiplier = night.ReflectionMultiplier;
	}
}