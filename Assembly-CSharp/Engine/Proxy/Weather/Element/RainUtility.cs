using Rain;

namespace Engine.Proxy.Weather.Element;

public class RainUtility {
	public static void CopyTo(Impl.Weather.Element.Rain rain) {
		var instance = RainManager.Instance;
		if (instance == null)
			return;
		rain.Intensity = instance.rainIntensity;
		rain.TerrainDryTime = instance.terrainDryTime;
		rain.TerrainFillTime = instance.terrainFillTime;
		rain.PuddlesDryTime = instance.puddleDryTime;
		rain.PuddlesFillTime = instance.puddleFillTime;
		rain.Direction = instance.windVector;
	}

	public static void CopyFrom(Impl.Weather.Element.Rain rain) {
		var instance = RainManager.Instance;
		if (instance == null)
			return;
		instance.rainIntensity = rain.Intensity;
		instance.terrainDryTime = rain.TerrainDryTime;
		instance.terrainFillTime = rain.TerrainFillTime;
		instance.puddleDryTime = rain.PuddlesDryTime;
		instance.puddleFillTime = rain.PuddlesFillTime;
		instance.windVector = rain.Direction;
	}
}