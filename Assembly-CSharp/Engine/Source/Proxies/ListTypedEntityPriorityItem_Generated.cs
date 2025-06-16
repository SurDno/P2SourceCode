using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Parameters;
using Engine.Source.Connections;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(PriorityItem<List<Typed<IEntity>>>))]
public class ListTypedEntityPriorityItem_Generated :
	ListTypedEntityPriorityItem,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<ListTypedEntityPriorityItem_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var priorityItemGenerated = (ListTypedEntityPriorityItem_Generated)target2;
		priorityItemGenerated.Priority = Priority;
		CloneableObjectUtility.FillListTo(priorityItemGenerated.Value, Value);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Priority", Priority);
		UnityDataWriteUtility.WriteList(writer, "Value", Value);
	}

	public void DataRead(IDataReader reader, Type type) {
		Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
		Value = UnityDataReadUtility.ReadList(reader, "Value", Value);
	}

	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Priority", Priority);
		UnityDataWriteUtility.WriteList(writer, "Value", Value);
	}

	public void StateLoad(IDataReader reader, Type type) {
		Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
		Value = UnityDataReadUtility.ReadList(reader, "Value", Value);
	}
}