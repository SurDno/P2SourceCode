using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(NotificationService))]
public class NotificationService_Generated :
	NotificationService,
	ISerializeStateSave,
	ISerializeStateLoad {
	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.WriteListEnum(writer, "BlockedTypes", blockedTypes);
	}

	public void StateLoad(IDataReader reader, Type type) {
		blockedTypes = DefaultDataReadUtility.ReadListEnum(reader, "BlockedTypes", blockedTypes);
	}
}