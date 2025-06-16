using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(FindVisibleDistanceEffect))]
public class FindVisibleDistanceEffect_Generated :
	FindVisibleDistanceEffect,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<FindVisibleDistanceEffect_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var distanceEffectGenerated = (FindVisibleDistanceEffect_Generated)target2;
		distanceEffectGenerated.queue = queue;
		distanceEffectGenerated.enable = enable;
		distanceEffectGenerated.durationType = durationType;
		distanceEffectGenerated.realTime = realTime;
		distanceEffectGenerated.duration = duration;
		distanceEffectGenerated.interval = interval;
		distanceEffectGenerated.VisibileDistanceParameterName = VisibileDistanceParameterName;
		distanceEffectGenerated.FlashlightOnParameterName = FlashlightOnParameterName;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
		DefaultDataWriteUtility.Write(writer, "Enable", enable);
		DefaultDataWriteUtility.WriteEnum(writer, "DurationType", durationType);
		DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
		DefaultDataWriteUtility.Write(writer, "Duration", duration);
		DefaultDataWriteUtility.Write(writer, "Interval", interval);
		DefaultDataWriteUtility.WriteEnum(writer, "VisibileDistanceParameterName", VisibileDistanceParameterName);
		DefaultDataWriteUtility.WriteEnum(writer, "FlashlightOnParameterName", FlashlightOnParameterName);
	}

	public void DataRead(IDataReader reader, Type type) {
		queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
		enable = DefaultDataReadUtility.Read(reader, "Enable", enable);
		durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
		realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
		duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
		interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
		VisibileDistanceParameterName =
			DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "VisibileDistanceParameterName");
		FlashlightOnParameterName =
			DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "FlashlightOnParameterName");
	}
}