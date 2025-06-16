using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(RegisterComponent))]
public class RegisterComponent_Generated :
	RegisterComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<RegisterComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((RegisterComponent_Generated)target2).tag = tag;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Tag", tag);
	}

	public void DataRead(IDataReader reader, Type type) {
		tag = DefaultDataReadUtility.Read(reader, "Tag", tag);
	}
}