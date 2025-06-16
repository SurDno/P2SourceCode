using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(DropBagService))]
public class DropBagService_Generated : DropBagService, ISerializeStateSave, ISerializeStateLoad {
	public void StateSave(IDataWriter writer) {
		CustomStateSaveUtility.SaveListReferences(writer, "Bags", bags);
	}

	public void StateLoad(IDataReader reader, Type type) {
		bags = CustomStateLoadUtility.LoadListReferences(reader, "Bags", bags);
	}
}