using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(NotEqualFloatOperations))]
public class NotEqualFloatOperations_Generated :
	NotEqualFloatOperations,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<NotEqualFloatOperations_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var operationsGenerated = (NotEqualFloatOperations_Generated)target2;
		operationsGenerated.a = CloneableObjectUtility.Clone(a);
		operationsGenerated.b = CloneableObjectUtility.Clone(b);
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