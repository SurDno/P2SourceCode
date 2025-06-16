using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Services
{
  [Factory]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class WeatherInfo
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public List<WeatherLayerInfo> Layers = new List<WeatherLayerInfo>();
  }
}
