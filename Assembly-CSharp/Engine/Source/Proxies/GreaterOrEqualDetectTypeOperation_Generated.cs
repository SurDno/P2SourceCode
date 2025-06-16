using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Expressions;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(GreaterOrEqualDetectTypeOperation))]
public class GreaterOrEqualDetectTypeOperation_Generated :
	GreaterOrEqualDetectTypeOperation,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<GreaterOrEqualDetectTypeOperation_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var operationGenerated = (GreaterOrEqualDetectTypeOperation_Generated)target2;
		operationGenerated.a = CloneableObjectUtility.Clone(a);
		operationGenerated.b = CloneableObjectUtility.Clone(b);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "Left", a);
		DefaultDataWriteUtility.WriteSerialize(writer, "Right", b);
	}

	public void DataRead(IDataReader reader, Type type) {
		a = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "Left");
		b = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "Right");
	}
}