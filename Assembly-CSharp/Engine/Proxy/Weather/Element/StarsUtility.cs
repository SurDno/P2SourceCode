using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;
using UnityEngine;

namespace Engine.Proxy.Weather.Element
{
  public class StarsUtility
  {
    public static void CopyTo(Stars stars)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      stars.Size = tod.Stars.Size;
      stars.Brightness = tod.Stars.Brightness;
    }

    public static void CopyFrom(Stars stars)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      tod.Stars.Size = stars.Size;
      tod.Stars.Brightness = stars.Brightness;
    }
  }
}
