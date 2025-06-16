using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Parameters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(PriorityItem<LockState>))]
public class LockStatePriorityItem_Generated :
	LockStatePriorityItem,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<LockStatePriorityItem_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var priorityItemGenerated = (LockStatePriorityItem_Generated)target2;
		priorityItemGenerated.Priority = Priority;
		priorityItemGenerated.Value = Value;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Priority", Priority);
		DefaultDataWriteUtility.WriteEnum(writer, "Value", Value);
	}

	public void DataRead(IDataReader reader, Type type) {
		Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
		Value = DefaultDataReadUtility.ReadEnum<LockState>(reader, "Value");
	}

	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Priority", Priority);
		DefaultDataWriteUtility.WriteEnum(writer, "Value", Value);
	}

	public void StateLoad(IDataReader reader, Type type) {
		Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
		Value = DefaultDataReadUtility.ReadEnum<LockState>(reader, "Value");
	}
}