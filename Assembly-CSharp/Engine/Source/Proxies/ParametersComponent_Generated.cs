using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(ParametersComponent))]
public class ParametersComponent_Generated :
	ParametersComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<ParametersComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		CloneableObjectUtility.CopyListTo(((ParametersComponent_Generated)target2).parameters, parameters);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteListSerialize(writer, "Parameters", parameters);
	}

	public void DataRead(IDataReader reader, Type type) {
		parameters = DefaultDataReadUtility.ReadListSerialize(reader, "Parameters", parameters);
	}

	public void StateSave(IDataWriter writer) {
		CustomStateSaveUtility.SaveListParameters(writer, "Parameters", parameters);
	}

	public void StateLoad(IDataReader reader, Type type) {
		parameters = CustomStateLoadUtility.LoadListParameters(reader, "Parameters", parameters);
	}
}