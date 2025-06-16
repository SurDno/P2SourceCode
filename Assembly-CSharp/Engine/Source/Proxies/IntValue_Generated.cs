using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(IntValue))]
public class IntValue_Generated :
	IntValue,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<IntValue_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((IntValue_Generated)target2).value = value;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Value", value);
	}

	public void DataRead(IDataReader reader, Type type) {
		value = DefaultDataReadUtility.Read(reader, "Value", value);
	}
}