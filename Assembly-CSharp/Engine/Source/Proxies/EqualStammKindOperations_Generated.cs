using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(EqualStammKindOperations))]
public class EqualStammKindOperations_Generated :
	EqualStammKindOperations,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<EqualStammKindOperations_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var operationsGenerated = (EqualStammKindOperations_Generated)target2;
		operationsGenerated.a = CloneableObjectUtility.Clone(a);
		operationsGenerated.b = CloneableObjectUtility.Clone(b);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "Left", a);
		DefaultDataWriteUtility.WriteSerialize(writer, "Right", b);
	}

	public void DataRead(IDataReader reader, Type type) {
		a = DefaultDataReadUtility.ReadSerialize<IValue<StammKind>>(reader, "Left");
		b = DefaultDataReadUtility.ReadSerialize<IValue<StammKind>>(reader, "Right");
	}
}