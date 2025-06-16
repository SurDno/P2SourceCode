using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      IncreaseStaminaInBlockEffect_Generated blockEffectGenerated = (IncreaseStaminaInBlockEffect_Generated) target2;
      blockEffectGenerated.queue = queue;
      blockEffectGenerated.enable = enable;
      blockEffectGenerated.staminaParameterName = staminaParameterName;
      blockEffectGenerated.runParameterName = runParameterName;
      blockEffectGenerated.blockTypeParameterName = blockTypeParameterName;
      blockEffectGenerated.isFightingParameterName = isFightingParameterName;
      blockEffectGenerated.durationType = durationType;
      blockEffectGenerated.realTime = realTime;
      blockEffectGenerated.duration = duration;
      blockEffectGenerated.interval = interval;
      blockEffectGenerated.increaseStaminaStepMaxValue = increaseStaminaStepMaxValue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.Write(writer, "Enable", enable);
      DefaultDataWriteUtility.WriteEnum(writer, "StaminaParameterName", staminaParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "RunParameterName", runParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "BlockTypeParameterName", blockTypeParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "IsFightingParameterName", isFightingParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "DurationType", durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", duration);
      DefaultDataWriteUtility.Write(writer, "Interval", interval);
      DefaultDataWriteUtility.Write(writer, "IncreaseStaminaStepMaxValue", increaseStaminaStepMaxValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      enable = DefaultDataReadUtility.Read(reader, "Enable", enable);
      staminaParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "StaminaParameterName");
      runParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "RunParameterName");
      blockTypeParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "BlockTypeParameterName");
      isFightingParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "IsFightingParameterName");
      durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
      duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
      interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
      increaseStaminaStepMaxValue = DefaultDataReadUtility.Read(reader, "IncreaseStaminaStepMaxValue", increaseStaminaStepMaxValue);
    }
  }
}
