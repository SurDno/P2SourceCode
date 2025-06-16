using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;

namespace Engine.Proxy.Weather.Element;

public class FallingLeavesUtility {
	public static void CopyTo(FallingLeaves fallingLeaves) {
		var fallingLeaves1 = ServiceLocator.GetService<EnvironmentService>().FallingLeaves;
		if (fallingLeaves1 == null)
			return;
		fallingLeaves.PoolCapacity = fallingLeaves1.poolCapacity;
		fallingLeaves.Radius = fallingLeaves1.radius;
		fallingLeaves.MinLandingNormalY = fallingLeaves1.minLandingNormalY;
		fallingLeaves.Deviation = fallingLeaves1.deviation;
		fallingLeaves.Rate = fallingLeaves1.rate;
	}

	public static void CopyFrom(FallingLeaves fallingLeaves) {
		var fallingLeaves1 = ServiceLocator.GetService<EnvironmentService>().FallingLeaves;
		if (fallingLeaves1 == null)
			return;
		fallingLeaves1.poolCapacity = fallingLeaves.PoolCapacity;
		fallingLeaves1.radius = fallingLeaves.Radius;
		fallingLeaves1.minLandingNormalY = fallingLeaves.MinLandingNormalY;
		fallingLeaves1.deviation = fallingLeaves.Deviation;
		fallingLeaves1.rate = fallingLeaves.Rate;
	}
}