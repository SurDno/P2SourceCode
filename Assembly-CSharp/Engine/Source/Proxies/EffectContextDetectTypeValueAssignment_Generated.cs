using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Engine.Source.Effects.Engine;
using Expressions;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(EffectContextDetectTypeValueAssignment))]
public class EffectContextDetectTypeValueAssignment_Generated :
	EffectContextDetectTypeValueAssignment,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<EffectContextDetectTypeValueAssignment_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var assignmentGenerated = (EffectContextDetectTypeValueAssignment_Generated)target2;
		assignmentGenerated.a = CloneableObjectUtility.Clone(a);
		assignmentGenerated.b = CloneableObjectUtility.Clone(b);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "A", a);
		DefaultDataWriteUtility.WriteSerialize(writer, "Source", b);
	}

	public void DataRead(IDataReader reader, Type type) {
		a = DefaultDataReadUtility.ReadSerialize<IValueSetter<DetectType>>(reader, "A");
		b = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "Source");
	}
}