using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (IncreaseThirstEffect))]
  public class IncreaseThirstEffect_Generated : 
    IncreaseThirstEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      IncreaseThirstEffect_Generated instance = Activator.CreateInstance<IncreaseThirstEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      IncreaseThirstEffect_Generated thirstEffectGenerated = (IncreaseThirstEffect_Generated) target2;
      thirstEffectGenerated.queue = this.queue;
      thirstEffectGenerated.enable = this.enable;
      thirstEffectGenerated.staminaParameterName = this.staminaParameterName;
      thirstEffectGenerated.thirstParameterName = this.thirstParameterName;
      thirstEffectGenerated.runParameterName = this.runParameterName;
      thirstEffectGenerated.lowStaminaParameterName = this.lowStaminaParameterName;
      thirstEffectGenerated.durationType = this.durationType;
      thirstEffectGenerated.realTime = this.realTime;
      thirstEffectGenerated.duration = this.duration;
      thirstEffectGenerated.interval = this.interval;
      thirstEffectGenerated.increaseThirstStepLowStaminaValue = this.increaseThirstStepLowStaminaValue;
      thirstEffectGenerated.increaseThirstStepMiddleStaminaValue = this.increaseThirstStepMiddleStaminaValue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Enable", this.enable);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "StaminaParameterName", this.staminaParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "ThirstParameterName", this.thirstParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "RunParameterName", this.runParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "LowStaminaParameterName", this.lowStaminaParameterName);
      DefaultDataWriteUtility.WriteEnum<DurationTypeEnum>(writer, "DurationType", this.durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", this.realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", this.duration);
      DefaultDataWriteUtility.Write(writer, "Interval", this.interval);
      DefaultDataWriteUtility.Write(writer, "IncreaseThirstStepLowStaminaValue", this.increaseThirstStepLowStaminaValue);
      DefaultDataWriteUtility.Write(writer, "IncreaseThirstStepMiddleStaminaValue", this.increaseThirstStepMiddleStaminaValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.enable = DefaultDataReadUtility.Read(reader, "Enable", this.enable);
      this.staminaParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "StaminaParameterName");
      this.thirstParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ThirstParameterName");
      this.runParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "RunParameterName");
      this.lowStaminaParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "LowStaminaParameterName");
      this.durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      this.realTime = DefaultDataReadUtility.Read(reader, "RealTime", this.realTime);
      this.duration = DefaultDataReadUtility.Read(reader, "Duration", this.duration);
      this.interval = DefaultDataReadUtility.Read(reader, "Interval", this.interval);
      this.increaseThirstStepLowStaminaValue = DefaultDataReadUtility.Read(reader, "IncreaseThirstStepLowStaminaValue", this.increaseThirstStepLowStaminaValue);
      this.increaseThirstStepMiddleStaminaValue = DefaultDataReadUtility.Read(reader, "IncreaseThirstStepMiddleStaminaValue", this.increaseThirstStepMiddleStaminaValue);
    }
  }
}
