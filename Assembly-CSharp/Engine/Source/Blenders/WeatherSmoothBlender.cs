using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Common.Weather;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Blenders
{
  [Factory(typeof (IWeatherSmoothBlender))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable)]
  public class WeatherSmoothBlender : 
    SmoothBlender<IWeatherSnapshot>,
    IWeatherSmoothBlender,
    ISmoothBlender<IWeatherSnapshot>,
    IObject
  {
  }
}
