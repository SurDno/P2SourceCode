using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(MapService))]
public class MapService_Generated : MapService, ISerializeStateSave, ISerializeStateLoad {
	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "BullModeAvailable", BullModeAvailable);
		DefaultDataWriteUtility.Write(writer, "BullModeForced", BullModeForced);
	}

	public void StateLoad(IDataReader reader, Type type) {
		BullModeAvailable = DefaultDataReadUtility.Read(reader, "BullModeAvailable", BullModeAvailable);
		BullModeForced = DefaultDataReadUtility.Read(reader, "BullModeForced", BullModeForced);
	}
}