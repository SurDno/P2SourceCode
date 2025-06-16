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
  [FactoryProxy(typeof (ApplyDamageToInfectionNPCEffect))]
  public class ApplyDamageToInfectionNPCEffect_Generated : 
    ApplyDamageToInfectionNPCEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ApplyDamageToInfectionNPCEffect_Generated instance = Activator.CreateInstance<ApplyDamageToInfectionNPCEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ApplyDamageToInfectionNPCEffect_Generated npcEffectGenerated = (ApplyDamageToInfectionNPCEffect_Generated) target2;
      npcEffectGenerated.queue = this.queue;
      npcEffectGenerated.enable = this.enable;
      npcEffectGenerated.durationType = this.durationType;
      npcEffectGenerated.realTime = this.realTime;
      npcEffectGenerated.duration = this.duration;
      npcEffectGenerated.interval = this.interval;
      npcEffectGenerated.infectionDamageParameterName = this.infectionDamageParameterName;
      npcEffectGenerated.infectionParameterName = this.infectionParameterName;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Enable", this.enable);
      DefaultDataWriteUtility.WriteEnum<DurationTypeEnum>(writer, "DurationType", this.durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", this.realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", this.duration);
      DefaultDataWriteUtility.Write(writer, "Interval", this.interval);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "InfectionDamageParameterName", this.infectionDamageParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "InfectionParameterName", this.infectionParameterName);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.enable = DefaultDataReadUtility.Read(reader, "Enable", this.enable);
      this.durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      this.realTime = DefaultDataReadUtility.Read(reader, "RealTime", this.realTime);
      this.duration = DefaultDataReadUtility.Read(reader, "Duration", this.duration);
      this.interval = DefaultDataReadUtility.Read(reader, "Interval", this.interval);
      this.infectionDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "InfectionDamageParameterName");
      this.infectionParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "InfectionParameterName");
    }
  }
}
