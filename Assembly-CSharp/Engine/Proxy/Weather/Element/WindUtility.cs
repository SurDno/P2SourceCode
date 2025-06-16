using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;

namespace Engine.Proxy.Weather.Element
{
  public class WindUtility
  {
    public static void CopyTo(Wind wind)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if (tod == null)
        return;
      if (tod.Components.Animation == null)
        return;
      TOD_Animation component = tod.Components.Animation.GetComponent<TOD_Animation>();
      wind.Degrees = component.WindDegrees;
      wind.Speed = component.WindSpeed;
    }

    public static void CopyFrom(Wind wind)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if (tod == null)
        return;
      if (tod.Components.Animation == null)
        return;
      TOD_Animation component = tod.Components.Animation.GetComponent<TOD_Animation>();
      component.WindDegrees = wind.Degrees;
      component.WindSpeed = wind.Speed;
    }
  }
}
