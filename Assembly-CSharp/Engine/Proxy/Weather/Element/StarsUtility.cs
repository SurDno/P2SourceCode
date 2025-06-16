using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;

namespace Engine.Proxy.Weather.Element;

public class StarsUtility {
	public static void CopyTo(Stars stars) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		stars.Size = tod.Stars.Size;
		stars.Brightness = tod.Stars.Brightness;
	}

	public static void CopyFrom(Stars stars) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		tod.Stars.Size = stars.Size;
		tod.Stars.Brightness = stars.Brightness;
	}
}