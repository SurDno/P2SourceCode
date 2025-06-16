using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Engine;
using Expressions;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(EffectContextIntValueSubtractionAssignment))]
public class EffectContextIntValueSubtractionAssignment_Generated :
	EffectContextIntValueSubtractionAssignment,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<EffectContextIntValueSubtractionAssignment_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var assignmentGenerated = (EffectContextIntValueSubtractionAssignment_Generated)target2;
		assignmentGenerated.a = CloneableObjectUtility.Clone(a);
		assignmentGenerated.b = CloneableObjectUtility.Clone(b);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "A", a);
		DefaultDataWriteUtility.WriteSerialize(writer, "Source", b);
	}

	public void DataRead(IDataReader reader, Type type) {
		a = DefaultDataReadUtility.ReadSerialize<IValueSetter<int>>(reader, "A");
		b = DefaultDataReadUtility.ReadSerialize<IValue<int>>(reader, "Source");
	}
}