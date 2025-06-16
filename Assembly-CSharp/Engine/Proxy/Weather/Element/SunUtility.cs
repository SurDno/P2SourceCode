using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;
using UnityEngine;

namespace Engine.Proxy.Weather.Element
{
  public class SunUtility
  {
    public static void CopyTo(Sun sun)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      sun.Size = tod.Sun.MeshSize;
      sun.Brightness = tod.Sun.MeshBrightness;
      sun.Contrast = tod.Sun.MeshContrast;
    }

    public static void CopyFrom(Sun sun)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      tod.Sun.MeshSize = sun.Size;
      tod.Sun.MeshBrightness = sun.Brightness;
      tod.Sun.MeshContrast = sun.Contrast;
    }
  }
}
