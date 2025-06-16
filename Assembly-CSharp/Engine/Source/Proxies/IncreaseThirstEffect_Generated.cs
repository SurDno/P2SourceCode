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

[FactoryProxy(typeof(IncreaseThirstEffect))]
public class IncreaseThirstEffect_Generated :
	IncreaseThirstEffect,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<IncreaseThirstEffect_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var thirstEffectGenerated = (IncreaseThirstEffect_Generated)target2;
		thirstEffectGenerated.queue = queue;
		thirstEffectGenerated.enable = enable;
		thirstEffectGenerated.staminaParameterName = staminaParameterName;
		thirstEffectGenerated.thirstParameterName = thirstParameterName;
		thirstEffectGenerated.runParameterName = runParameterName;
		thirstEffectGenerated.lowStaminaParameterName = lowStaminaParameterName;
		thirstEffectGenerated.durationType = durationType;
		thirstEffectGenerated.realTime = realTime;
		thirstEffectGenerated.duration = duration;
		thirstEffectGenerated.interval = interval;
		thirstEffectGenerated.increaseThirstStepLowStaminaValue = increaseThirstStepLowStaminaValue;
		thirstEffectGenerated.increaseThirstStepMiddleStaminaValue = increaseThirstStepMiddleStaminaValue;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
		DefaultDataWriteUtility.Write(writer, "Enable", enable);
		DefaultDataWriteUtility.WriteEnum(writer, "StaminaParameterName", staminaParameterName);
		DefaultDataWriteUtility.WriteEnum(writer, "ThirstParameterName", thirstParameterName);
		DefaultDataWriteUtility.WriteEnum(writer, "RunParameterName", runParameterName);
		DefaultDataWriteUtility.WriteEnum(writer, "LowStaminaParameterName", lowStaminaParameterName);
		DefaultDataWriteUtility.WriteEnum(writer, "DurationType", durationType);
		DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
		DefaultDataWriteUtility.Write(writer, "Duration", duration);
		DefaultDataWriteUtility.Write(writer, "Interval", interval);
		DefaultDataWriteUtility.Write(writer, "IncreaseThirstStepLowStaminaValue", increaseThirstStepLowStaminaValue);
		DefaultDataWriteUtility.Write(writer, "IncreaseThirstStepMiddleStaminaValue",
			increaseThirstStepMiddleStaminaValue);
	}

	public void DataRead(IDataReader reader, Type type) {
		queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
		enable = DefaultDataReadUtility.Read(reader, "Enable", enable);
		staminaParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "StaminaParameterName");
		thirstParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ThirstParameterName");
		runParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "RunParameterName");
		lowStaminaParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "LowStaminaParameterName");
		durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
		realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
		duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
		interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
		increaseThirstStepLowStaminaValue = DefaultDataReadUtility.Read(reader, "IncreaseThirstStepLowStaminaValue",
			increaseThirstStepLowStaminaValue);
		increaseThirstStepMiddleStaminaValue = DefaultDataReadUtility.Read(reader,
			"IncreaseThirstStepMiddleStaminaValue", increaseThirstStepMiddleStaminaValue);
	}
}