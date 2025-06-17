using System.Collections.Generic;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Services
{
  [Factory]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class WeatherInfo
  {
    [StateSaveProxy]
    [StateLoadProxy()]
    [Inspected]
    public List<WeatherLayerInfo> Layers = [];
  }
}
