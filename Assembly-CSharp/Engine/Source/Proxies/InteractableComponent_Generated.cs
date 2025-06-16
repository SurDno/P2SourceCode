using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(InteractableComponent))]
public class InteractableComponent_Generated :
	InteractableComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<InteractableComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var componentGenerated = (InteractableComponent_Generated)target2;
		componentGenerated.isEnabled = isEnabled;
		CloneableObjectUtility.CopyListTo(componentGenerated.items, items);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		DefaultDataWriteUtility.WriteListSerialize(writer, "Items", items);
	}

	public void DataRead(IDataReader reader, Type type) {
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
		items = DefaultDataReadUtility.ReadListSerialize(reader, "Items", items);
	}

	public void StateSave(IDataWriter writer) {
		EngineDataWriteUtility.Write(writer, "Title", Title);
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
	}

	public void StateLoad(IDataReader reader, Type type) {
		Title = EngineDataReadUtility.Read(reader, "Title", Title);
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
	}
}