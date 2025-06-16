using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Common.Weather;
using Engine.Source.Services;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(WeatherLayerInfo))]
public class WeatherLayerInfo_Generated :
	WeatherLayerInfo,
	ISerializeStateSave,
	ISerializeStateLoad {
	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Layer", Layer);
		DefaultDataWriteUtility.Write(writer, "Opacity", Opacity);
		DefaultDataWriteUtility.Write(writer, "SnapshotTemplateId", SnapshotTemplateId);
	}

	public void StateLoad(IDataReader reader, Type type) {
		Layer = DefaultDataReadUtility.ReadEnum<WeatherLayer>(reader, "Layer");
		Opacity = DefaultDataReadUtility.Read(reader, "Opacity", Opacity);
		SnapshotTemplateId = DefaultDataReadUtility.Read(reader, "SnapshotTemplateId", SnapshotTemplateId);
	}
}