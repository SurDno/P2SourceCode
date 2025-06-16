using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(PlusFloatPolyOperation))]
public class PlusFloatPolyOperation_Generated :
	PlusFloatPolyOperation,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<PlusFloatPolyOperation_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		CloneableObjectUtility.CopyListTo(((PlusFloatPolyOperation_Generated)target2).values, values);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteListSerialize(writer, "Parameters", values);
	}

	public void DataRead(IDataReader reader, Type type) {
		values = DefaultDataReadUtility.ReadListSerialize(reader, "Parameters", values);
	}
}