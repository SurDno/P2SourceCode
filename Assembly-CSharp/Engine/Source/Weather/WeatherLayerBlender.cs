using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Common.Weather;
using Engine.Impl.Services.Factories;
using Engine.Source.Blenders;

namespace Engine.Source.Weather
{
  [Factory(typeof (IWeatherLayerBlender))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable)]
  public class WeatherLayerBlender : 
    LayerBlender<IWeatherSnapshot>,
    IWeatherLayerBlender,
    ILayerBlender<IWeatherSnapshot>,
    IObject
  {
  }
}
