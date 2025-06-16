using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(VisualFlameEffect))]
public class VisualFlameEffect_Generated :
	VisualFlameEffect,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<VisualFlameEffect_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var flameEffectGenerated = (VisualFlameEffect_Generated)target2;
		flameEffectGenerated.queue = queue;
		flameEffectGenerated.single = single;
		flameEffectGenerated.realTime = realTime;
		flameEffectGenerated.duration = duration;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
		DefaultDataWriteUtility.Write(writer, "Single", single);
		DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
		DefaultDataWriteUtility.Write(writer, "Duration", duration);
	}

	public void DataRead(IDataReader reader, Type type) {
		queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
		single = DefaultDataReadUtility.Read(reader, "Single", single);
		realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
		duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
	}
}