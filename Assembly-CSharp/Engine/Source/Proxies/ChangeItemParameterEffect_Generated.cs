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
  [FactoryProxy(typeof (ChangeItemParameterEffect))]
  public class ChangeItemParameterEffect_Generated : 
    ChangeItemParameterEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ChangeItemParameterEffect_Generated instance = Activator.CreateInstance<ChangeItemParameterEffect_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ChangeItemParameterEffect_Generated parameterEffectGenerated = (ChangeItemParameterEffect_Generated) target2;
      parameterEffectGenerated.queue = queue;
      parameterEffectGenerated.enable = enable;
      parameterEffectGenerated.durationType = durationType;
      parameterEffectGenerated.realTime = realTime;
      parameterEffectGenerated.duration = duration;
      parameterEffectGenerated.interval = interval;
      parameterEffectGenerated.itemParameterName = itemParameterName;
      parameterEffectGenerated.itemParameterChange = itemParameterChange;
      parameterEffectGenerated.difficultyMultiplierParameterName = difficultyMultiplierParameterName;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.Write(writer, "Enable", enable);
      DefaultDataWriteUtility.WriteEnum(writer, "DurationType", durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", duration);
      DefaultDataWriteUtility.Write(writer, "Interval", interval);
      DefaultDataWriteUtility.WriteEnum(writer, "ItemParameterName", itemParameterName);
      DefaultDataWriteUtility.Write(writer, "ItemParameterChange", itemParameterChange);
      DefaultDataWriteUtility.Write(writer, "DifficultyMultiplierParameterName", difficultyMultiplierParameterName);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      enable = DefaultDataReadUtility.Read(reader, "Enable", enable);
      durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
      duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
      interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
      itemParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ItemParameterName");
      itemParameterChange = DefaultDataReadUtility.Read(reader, "ItemParameterChange", itemParameterChange);
      difficultyMultiplierParameterName = DefaultDataReadUtility.Read(reader, "DifficultyMultiplierParameterName", difficultyMultiplierParameterName);
    }
  }
}
