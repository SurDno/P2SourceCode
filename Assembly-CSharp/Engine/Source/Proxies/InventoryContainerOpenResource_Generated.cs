using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(InventoryContainerOpenResource))]
public class InventoryContainerOpenResource_Generated :
	InventoryContainerOpenResource,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<InventoryContainerOpenResource_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var resourceGenerated = (InventoryContainerOpenResource_Generated)target2;
		resourceGenerated.resource = resource;
		resourceGenerated.amount = amount;
	}

	public void DataWrite(IDataWriter writer) {
		UnityDataWriteUtility.Write(writer, "Resource", resource);
		DefaultDataWriteUtility.Write(writer, "Amount", amount);
	}

	public void DataRead(IDataReader reader, Type type) {
		resource = UnityDataReadUtility.Read(reader, "Resource", resource);
		amount = DefaultDataReadUtility.Read(reader, "Amount", amount);
	}
}