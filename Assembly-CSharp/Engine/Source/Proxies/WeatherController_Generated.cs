using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Impl.Services;
using Engine.Source.Services;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(WeatherController))]
public class WeatherController_Generated :
	WeatherController,
	ISerializeStateSave,
	ISerializeStateLoad {
	public void StateSave(IDataWriter writer) {
		DefaultStateSaveUtility.SaveSerialize(writer, "WeatherInfo", WeatherInfo);
	}

	public void StateLoad(IDataReader reader, Type type) {
		WeatherInfo = DefaultStateLoadUtility.ReadSerialize<WeatherInfo>(reader, "WeatherInfo");
	}
}