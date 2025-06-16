using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(MessangerComponent))]
public class MessangerComponent_Generated :
	MessangerComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<MessangerComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) { }

	public void DataWrite(IDataWriter writer) { }

	public void DataRead(IDataReader reader, Type type) { }

	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Registred", registred);
	}

	public void StateLoad(IDataReader reader, Type type) {
		registred = DefaultDataReadUtility.Read(reader, "Registred", registred);
	}
}