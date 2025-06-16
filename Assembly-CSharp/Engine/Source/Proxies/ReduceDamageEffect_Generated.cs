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
  [FactoryProxy(typeof (ReduceDamageEffect))]
  public class ReduceDamageEffect_Generated : 
    ReduceDamageEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ReduceDamageEffect_Generated instance = Activator.CreateInstance<ReduceDamageEffect_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ReduceDamageEffect_Generated damageEffectGenerated = (ReduceDamageEffect_Generated) target2;
      damageEffectGenerated.queue = queue;
      damageEffectGenerated.enable = enable;
      damageEffectGenerated.durationType = durationType;
      damageEffectGenerated.realTime = realTime;
      damageEffectGenerated.duration = duration;
      damageEffectGenerated.interval = interval;
      damageEffectGenerated.damageParameterName = damageParameterName;
      damageEffectGenerated.adsorbtionMaxParameterName = adsorbtionMaxParameterName;
      damageEffectGenerated.maxArmor = maxArmor;
      damageEffectGenerated.durabilityReduceByHit = durabilityReduceByHit;
      damageEffectGenerated.durabilityReduceKoeficient = durabilityReduceKoeficient;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.Write(writer, "Enable", enable);
      DefaultDataWriteUtility.WriteEnum(writer, "DurationType", durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", duration);
      DefaultDataWriteUtility.Write(writer, "Interval", interval);
      DefaultDataWriteUtility.WriteEnum(writer, "DamageParameterName", damageParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "AdsorbtionMaxParameterName", adsorbtionMaxParameterName);
      DefaultDataWriteUtility.Write(writer, "MaxArmor", maxArmor);
      DefaultDataWriteUtility.Write(writer, "DurabilityReduceByHit", durabilityReduceByHit);
      DefaultDataWriteUtility.Write(writer, "DurabilityReduceKoeficient", durabilityReduceKoeficient);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      enable = DefaultDataReadUtility.Read(reader, "Enable", enable);
      durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
      duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
      interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
      damageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "DamageParameterName");
      adsorbtionMaxParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "AdsorbtionMaxParameterName");
      maxArmor = DefaultDataReadUtility.Read(reader, "MaxArmor", maxArmor);
      durabilityReduceByHit = DefaultDataReadUtility.Read(reader, "DurabilityReduceByHit", durabilityReduceByHit);
      durabilityReduceKoeficient = DefaultDataReadUtility.Read(reader, "DurabilityReduceKoeficient", durabilityReduceKoeficient);
    }
  }
}
