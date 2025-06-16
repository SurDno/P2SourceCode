using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(Entity))]
public class Entity_Generated :
	Entity,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		return ServiceCache.Factory.Instantiate(this);
	}

	public void CopyTo(object target2) {
		var entityGenerated = (Entity_Generated)target2;
		entityGenerated.name = name;
		CloneableObjectUtility.CopyListTo(entityGenerated.components, components);
		entityGenerated.isEnabled = isEnabled;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.WriteListSerialize(writer, "Components", components);
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
	}

	public void DataRead(IDataReader reader, Type type) {
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		components = DefaultDataReadUtility.ReadListSerialize(reader, "Components", components);
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
	}

	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "Name", name);
		CustomStateSaveUtility.SaveListComponents(writer, "Components", components);
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		DefaultDataWriteUtility.Write(writer, "HierarchyPath", HierarchyPath);
	}

	public void StateLoad(IDataReader reader, Type type) {
		name = DefaultDataReadUtility.Read(reader, "Name", name);
		components = CustomStateLoadUtility.LoadListComponents(reader, "Components", components);
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
	}
}