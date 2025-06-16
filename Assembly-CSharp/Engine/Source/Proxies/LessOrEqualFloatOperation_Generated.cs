using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(LessOrEqualFloatOperation))]
public class LessOrEqualFloatOperation_Generated :
	LessOrEqualFloatOperation,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<LessOrEqualFloatOperation_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var operationGenerated = (LessOrEqualFloatOperation_Generated)target2;
		operationGenerated.a = CloneableObjectUtility.Clone(a);
		operationGenerated.b = CloneableObjectUtility.Clone(b);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "Left", a);
		DefaultDataWriteUtility.WriteSerialize(writer, "Right", b);
	}

	public void DataRead(IDataReader reader, Type type) {
		a = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "Left");
		b = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "Right");
	}
}