using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(SpreadingService))]
public class SpreadingService_Generated :
	SpreadingService,
	ISerializeStateSave,
	ISerializeStateLoad {
	public void StateSave(IDataWriter writer) {
		CustomStateSaveUtility.SaveListReferences(writer, "SpreadingComponents", spreadingComponents);
		CustomStateSaveUtility.SaveListReferences(writer, "RegionComponents", regionComponents);
	}

	public void StateLoad(IDataReader reader, Type type) {
		spreadingComponents =
			CustomStateLoadUtility.LoadListReferences(reader, "SpreadingComponents", spreadingComponents);
		regionComponents = CustomStateLoadUtility.LoadListReferences(reader, "RegionComponents", regionComponents);
	}
}