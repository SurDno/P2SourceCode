using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;

namespace Engine.Proxy.Weather.Element;

public class WindUtility {
	public static void CopyTo(Wind wind) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		if (tod.Components.Animation == null)
			return;
		var component = tod.Components.Animation.GetComponent<TOD_Animation>();
		wind.Degrees = component.WindDegrees;
		wind.Speed = component.WindSpeed;
	}

	public static void CopyFrom(Wind wind) {
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod == null)
			return;
		if (tod.Components.Animation == null)
			return;
		var component = tod.Components.Animation.GetComponent<TOD_Animation>();
		component.WindDegrees = wind.Degrees;
		component.WindSpeed = wind.Speed;
	}
}