using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;

namespace Engine.Proxy.Weather.Element;

public class MoonUtility {
	public static void CopyTo(Moon moon) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		moon.Size = tod.Moon.MeshSize;
		moon.Brightness = tod.Moon.MeshBrightness;
		moon.Contrast = tod.Moon.MeshContrast;
	}

	public static void CopyFrom(Moon moon) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		tod.Moon.MeshSize = moon.Size;
		tod.Moon.MeshBrightness = moon.Brightness;
		tod.Moon.MeshContrast = moon.Contrast;
	}
}