using Engine.Common.Blenders;

namespace Engine.Common.Weather
{
  public interface IWeatherSmoothBlender : ISmoothBlender<IWeatherSnapshot>, IObject
  {
  }
}
