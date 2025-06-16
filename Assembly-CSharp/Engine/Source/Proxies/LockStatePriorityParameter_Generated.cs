using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Parameters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(LockStatePriorityParameter))]
public class LockStatePriorityParameter_Generated :
	LockStatePriorityParameter,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<LockStatePriorityParameter_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var parameterGenerated = (LockStatePriorityParameter_Generated)target2;
		parameterGenerated.name = name;
		parameterGenerated.container = CloneableObjectUtility.Clone(container);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
		DefaultDataWriteUtility.WriteSerialize(writer, "Container", container);
	}

	public void DataRead(IDataReader reader, Type type) {
		name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
		container = DefaultDataReadUtility.ReadSerialize<PriorityContainer<LockState>>(reader, "Container");
	}

	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
		DefaultStateSaveUtility.SaveSerialize(writer, "Container", container);
	}

	public void StateLoad(IDataReader reader, Type type) {
		container = DefaultStateLoadUtility.ReadSerialize<PriorityContainer<LockState>>(reader, "Container");
	}
}