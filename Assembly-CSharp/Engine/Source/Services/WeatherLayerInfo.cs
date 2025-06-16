using Engine.Common.Generator;
using Engine.Common.Weather;
using Inspectors;
using System;

namespace Engine.Source.Services
{
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class WeatherLayerInfo
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public WeatherLayer Layer;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public float Opacity;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public Guid SnapshotTemplateId;
  }
}
