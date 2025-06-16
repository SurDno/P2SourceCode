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

[FactoryProxy(typeof(ImmunityInfectionEffect))]
public class ImmunityInfectionEffect_Generated :
	ImmunityInfectionEffect,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<ImmunityInfectionEffect_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var infectionEffectGenerated = (ImmunityInfectionEffect_Generated)target2;
		infectionEffectGenerated.queue = queue;
		infectionEffectGenerated.enable = enable;
		infectionEffectGenerated.durationType = durationType;
		infectionEffectGenerated.realTime = realTime;
		infectionEffectGenerated.duration = duration;
		infectionEffectGenerated.interval = interval;
		infectionEffectGenerated.infectionDamageParameterName = infectionDamageParameterName;
		infectionEffectGenerated.immunityParameterName = immunityParameterName;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
		DefaultDataWriteUtility.Write(writer, "Enable", enable);
		DefaultDataWriteUtility.WriteEnum(writer, "DurationType", durationType);
		DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
		DefaultDataWriteUtility.Write(writer, "Duration", duration);
		DefaultDataWriteUtility.Write(writer, "Interval", interval);
		DefaultDataWriteUtility.WriteEnum(writer, "InfectionDamageParameterName", infectionDamageParameterName);
		DefaultDataWriteUtility.WriteEnum(writer, "ImmunityParameterName", immunityParameterName);
	}

	public void DataRead(IDataReader reader, Type type) {
		queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
		enable = DefaultDataReadUtility.Read(reader, "Enable", enable);
		durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
		realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
		duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
		interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
		infectionDamageParameterName =
			DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "InfectionDamageParameterName");
		immunityParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ImmunityParameterName");
	}
}