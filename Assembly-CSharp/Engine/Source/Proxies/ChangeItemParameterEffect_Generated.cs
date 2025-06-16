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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ChangeItemParameterEffect_Generated parameterEffectGenerated = (ChangeItemParameterEffect_Generated) target2;
      parameterEffectGenerated.queue = this.queue;
      parameterEffectGenerated.enable = this.enable;
      parameterEffectGenerated.durationType = this.durationType;
      parameterEffectGenerated.realTime = this.realTime;
      parameterEffectGenerated.duration = this.duration;
      parameterEffectGenerated.interval = this.interval;
      parameterEffectGenerated.itemParameterName = this.itemParameterName;
      parameterEffectGenerated.itemParameterChange = this.itemParameterChange;
      parameterEffectGenerated.difficultyMultiplierParameterName = this.difficultyMultiplierParameterName;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Enable", this.enable);
      DefaultDataWriteUtility.WriteEnum<DurationTypeEnum>(writer, "DurationType", this.durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", this.realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", this.duration);
      DefaultDataWriteUtility.Write(writer, "Interval", this.interval);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "ItemParameterName", this.itemParameterName);
      DefaultDataWriteUtility.Write(writer, "ItemParameterChange", this.itemParameterChange);
      DefaultDataWriteUtility.Write(writer, "DifficultyMultiplierParameterName", this.difficultyMultiplierParameterName);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.enable = DefaultDataReadUtility.Read(reader, "Enable", this.enable);
      this.durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      this.realTime = DefaultDataReadUtility.Read(reader, "RealTime", this.realTime);
      this.duration = DefaultDataReadUtility.Read(reader, "Duration", this.duration);
      this.interval = DefaultDataReadUtility.Read(reader, "Interval", this.interval);
      this.itemParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ItemParameterName");
      this.itemParameterChange = DefaultDataReadUtility.Read(reader, "ItemParameterChange", this.itemParameterChange);
      this.difficultyMultiplierParameterName = DefaultDataReadUtility.Read(reader, "DifficultyMultiplierParameterName", this.difficultyMultiplierParameterName);
    }
  }
}
