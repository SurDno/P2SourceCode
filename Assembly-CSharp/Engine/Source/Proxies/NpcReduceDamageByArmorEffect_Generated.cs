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

[FactoryProxy(typeof(NpcReduceDamageByArmorEffect))]
public class NpcReduceDamageByArmorEffect_Generated :
	NpcReduceDamageByArmorEffect,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<NpcReduceDamageByArmorEffect_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var armorEffectGenerated = (NpcReduceDamageByArmorEffect_Generated)target2;
		armorEffectGenerated.queue = queue;
		armorEffectGenerated.enable = enable;
		armorEffectGenerated.durationType = durationType;
		armorEffectGenerated.realTime = realTime;
		armorEffectGenerated.duration = duration;
		armorEffectGenerated.interval = interval;
		armorEffectGenerated.damageParameterName = damageParameterName;
		armorEffectGenerated.armorParameterName = armorParameterName;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
		DefaultDataWriteUtility.Write(writer, "Enable", enable);
		DefaultDataWriteUtility.WriteEnum(writer, "DurationType", durationType);
		DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
		DefaultDataWriteUtility.Write(writer, "Duration", duration);
		DefaultDataWriteUtility.Write(writer, "Interval", interval);
		DefaultDataWriteUtility.WriteEnum(writer, "DamageParameterName", damageParameterName);
		DefaultDataWriteUtility.WriteEnum(writer, "ArmorParameterName", armorParameterName);
	}

	public void DataRead(IDataReader reader, Type type) {
		queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
		enable = DefaultDataReadUtility.Read(reader, "Enable", enable);
		durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
		realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
		duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
		interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
		damageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "DamageParameterName");
		armorParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ArmorParameterName");
	}
}