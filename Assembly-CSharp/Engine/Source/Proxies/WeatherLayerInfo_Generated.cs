using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Common.Weather;
using Engine.Source.Services;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (WeatherLayerInfo))]
  public class WeatherLayerInfo_Generated : 
    WeatherLayerInfo,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<WeatherLayer>(writer, "Layer", this.Layer);
      DefaultDataWriteUtility.Write(writer, "Opacity", this.Opacity);
      DefaultDataWriteUtility.Write(writer, "SnapshotTemplateId", this.SnapshotTemplateId);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Layer = DefaultDataReadUtility.ReadEnum<WeatherLayer>(reader, "Layer");
      this.Opacity = DefaultDataReadUtility.Read(reader, "Opacity", this.Opacity);
      this.SnapshotTemplateId = DefaultDataReadUtility.Read(reader, "SnapshotTemplateId", this.SnapshotTemplateId);
    }
  }
}
