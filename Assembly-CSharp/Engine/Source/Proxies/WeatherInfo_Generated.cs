using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (WeatherInfo))]
  public class WeatherInfo_Generated : WeatherInfo, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveListSerialize(writer, "Layers", Layers);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      Layers = DefaultStateLoadUtility.ReadListSerialize(reader, "Layers", Layers);
    }
  }
}
