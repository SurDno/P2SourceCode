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
  [FactoryProxy(typeof (IncreaseStaminaInBlockEffect))]
  public class IncreaseStaminaInBlockEffect_Generated : 
    IncreaseStaminaInBlockEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      IncreaseStaminaInBlockEffect_Generated instance = Activator.CreateInstance<IncreaseStaminaInBlockEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      IncreaseStaminaInBlockEffect_Generated blockEffectGenerated = (IncreaseStaminaInBlockEffect_Generated) target2;
      blockEffectGenerated.queue = this.queue;
      blockEffectGenerated.enable = this.enable;
      blockEffectGenerated.staminaParameterName = this.staminaParameterName;
      blockEffectGenerated.runParameterName = this.runParameterName;
      blockEffectGenerated.blockTypeParameterName = this.blockTypeParameterName;
      blockEffectGenerated.isFightingParameterName = this.isFightingParameterName;
      blockEffectGenerated.durationType = this.durationType;
      blockEffectGenerated.realTime = this.realTime;
      blockEffectGenerated.duration = this.duration;
      blockEffectGenerated.interval = this.interval;
      blockEffectGenerated.increaseStaminaStepMaxValue = this.increaseStaminaStepMaxValue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Enable", this.enable);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "StaminaParameterName", this.staminaParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "RunParameterName", this.runParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "BlockTypeParameterName", this.blockTypeParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "IsFightingParameterName", this.isFightingParameterName);
      DefaultDataWriteUtility.WriteEnum<DurationTypeEnum>(writer, "DurationType", this.durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", this.realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", this.duration);
      DefaultDataWriteUtility.Write(writer, "Interval", this.interval);
      DefaultDataWriteUtility.Write(writer, "IncreaseStaminaStepMaxValue", this.increaseStaminaStepMaxValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.enable = DefaultDataReadUtility.Read(reader, "Enable", this.enable);
      this.staminaParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "StaminaParameterName");
      this.runParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "RunParameterName");
      this.blockTypeParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "BlockTypeParameterName");
      this.isFightingParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "IsFightingParameterName");
      this.durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      this.realTime = DefaultDataReadUtility.Read(reader, "RealTime", this.realTime);
      this.duration = DefaultDataReadUtility.Read(reader, "Duration", this.duration);
      this.interval = DefaultDataReadUtility.Read(reader, "Interval", this.interval);
      this.increaseStaminaStepMaxValue = DefaultDataReadUtility.Read(reader, "IncreaseStaminaStepMaxValue", this.increaseStaminaStepMaxValue);
    }
  }
}
