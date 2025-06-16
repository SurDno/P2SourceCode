using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;

namespace Engine.Proxy.Weather.Element
{
  public class LocationUtility
  {
    public static void CopyTo(Location location)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if (tod == null)
        return;
      location.Latitude = tod.World.Latitude;
      location.Longitude = tod.World.Longitude;
      location.Utc = tod.World.UTC;
    }

    public static void CopyFrom(Location location)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if (tod == null)
        return;
      tod.World.Latitude = location.Latitude;
      tod.World.Longitude = location.Longitude;
      tod.World.UTC = location.Utc;
    }
  }
}
