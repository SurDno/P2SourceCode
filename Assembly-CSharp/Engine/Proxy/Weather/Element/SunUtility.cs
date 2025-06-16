using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;

namespace Engine.Proxy.Weather.Element;

public class SunUtility {
	public static void CopyTo(Sun sun) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		sun.Size = tod.Sun.MeshSize;
		sun.Brightness = tod.Sun.MeshBrightness;
		sun.Contrast = tod.Sun.MeshContrast;
	}

	public static void CopyFrom(Sun sun) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		tod.Sun.MeshSize = sun.Size;
		tod.Sun.MeshBrightness = sun.Brightness;
		tod.Sun.MeshContrast = sun.Contrast;
	}
}