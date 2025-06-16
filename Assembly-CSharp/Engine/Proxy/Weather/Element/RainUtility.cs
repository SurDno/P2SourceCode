using Rain;
using UnityEngine;

namespace Engine.Proxy.Weather.Element
{
  public class RainUtility
  {
    public static void CopyTo(Engine.Impl.Weather.Element.Rain rain)
    {
      RainManager instance = RainManager.Instance;
      if ((Object) instance == (Object) null)
        return;
      rain.Intensity = instance.rainIntensity;
      rain.TerrainDryTime = instance.terrainDryTime;
      rain.TerrainFillTime = instance.terrainFillTime;
      rain.PuddlesDryTime = instance.puddleDryTime;
      rain.PuddlesFillTime = instance.puddleFillTime;
      rain.Direction = instance.windVector;
    }

    public static void CopyFrom(Engine.Impl.Weather.Element.Rain rain)
    {
      RainManager instance = RainManager.Instance;
      if ((Object) instance == (Object) null)
        return;
      instance.rainIntensity = rain.Intensity;
      instance.terrainDryTime = rain.TerrainDryTime;
      instance.terrainFillTime = rain.TerrainFillTime;
      instance.puddleDryTime = rain.PuddlesDryTime;
      instance.puddleFillTime = rain.PuddlesFillTime;
      instance.windVector = rain.Direction;
    }
  }
}
