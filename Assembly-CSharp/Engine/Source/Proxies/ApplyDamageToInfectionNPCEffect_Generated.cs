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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ApplyDamageToInfectionNPCEffect_Generated npcEffectGenerated = (ApplyDamageToInfectionNPCEffect_Generated) target2;
      npcEffectGenerated.queue = queue;
      npcEffectGenerated.enable = enable;
      npcEffectGenerated.durationType = durationType;
      npcEffectGenerated.realTime = realTime;
      npcEffectGenerated.duration = duration;
      npcEffectGenerated.interval = interval;
      npcEffectGenerated.infectionDamageParameterName = infectionDamageParameterName;
      npcEffectGenerated.infectionParameterName = infectionParameterName;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.Write(writer, "Enable", enable);
      DefaultDataWriteUtility.WriteEnum(writer, "DurationType", durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", duration);
      DefaultDataWriteUtility.Write(writer, "Interval", interval);
      DefaultDataWriteUtility.WriteEnum(writer, "InfectionDamageParameterName", infectionDamageParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "InfectionParameterName", infectionParameterName);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      enable = DefaultDataReadUtility.Read(reader, "Enable", enable);
      durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
      duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
      interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
      infectionDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "InfectionDamageParameterName");
      infectionParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "InfectionParameterName");
    }
  }
}
