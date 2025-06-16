using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(StorageComponent))]
public class StorageComponent_Generated :
	StorageComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<StorageComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var componentGenerated = (StorageComponent_Generated)target2;
		componentGenerated.tag = tag;
		CloneableObjectUtility.CopyListTo(componentGenerated.inventoryTemplates, inventoryTemplates);
		CloneableObjectUtility.CopyListTo(componentGenerated.items, items);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Tag", tag);
		DefaultDataWriteUtility.WriteListSerialize(writer, "InventoryTemplates", inventoryTemplates);
	}

	public void DataRead(IDataReader reader, Type type) {
		tag = DefaultDataReadUtility.Read(reader, "Tag", tag);
		inventoryTemplates = DefaultDataReadUtility.ReadListSerialize(reader, "InventoryTemplates", inventoryTemplates);
	}

	public void StateSave(IDataWriter writer) {
		CustomStateSaveUtility.SaveListReferences(writer, "Items", items);
	}

	public void StateLoad(IDataReader reader, Type type) {
		items = CustomStateLoadUtility.LoadListReferences(reader, "Items", items);
	}
}