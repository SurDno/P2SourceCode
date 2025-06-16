using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;

namespace Engine.Proxy.Weather.Element
{
  public class CloudsUtility
  {
    public static void CopyTo(Clouds clouds)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      clouds.Size = tod.Clouds.Size;
      clouds.Opacity = tod.Clouds.Opacity;
      clouds.Coverage = tod.Clouds.Coverage;
      clouds.Sharpness = tod.Clouds.Sharpness;
      clouds.Attenuation = tod.Clouds.Attenuation;
      clouds.Saturation = tod.Clouds.Saturation;
      clouds.Scattering = tod.Clouds.Scattering;
      clouds.Brightness = tod.Clouds.Brightness;
    }

    public static void CopyFrom(Clouds clouds)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      tod.Clouds.Size = clouds.Size;
      tod.Clouds.Opacity = clouds.Opacity;
      tod.Clouds.Coverage = clouds.Coverage;
      tod.Clouds.Sharpness = clouds.Sharpness;
      tod.Clouds.Attenuation = clouds.Attenuation;
      tod.Clouds.Saturation = clouds.Saturation;
      tod.Clouds.Scattering = clouds.Scattering;
      tod.Clouds.Brightness = clouds.Brightness;
    }
  }
}
