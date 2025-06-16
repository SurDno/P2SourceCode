using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (WeatherInfo))]
  public class WeatherInfo_Generated : WeatherInfo, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveListSerialize<WeatherLayerInfo>(writer, "Layers", this.Layers);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Layers = DefaultStateLoadUtility.ReadListSerialize<WeatherLayerInfo>(reader, "Layers", this.Layers);
    }
  }
}
