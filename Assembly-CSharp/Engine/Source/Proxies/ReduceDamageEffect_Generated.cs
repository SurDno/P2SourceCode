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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ReduceDamageEffect_Generated damageEffectGenerated = (ReduceDamageEffect_Generated) target2;
      damageEffectGenerated.queue = this.queue;
      damageEffectGenerated.enable = this.enable;
      damageEffectGenerated.durationType = this.durationType;
      damageEffectGenerated.realTime = this.realTime;
      damageEffectGenerated.duration = this.duration;
      damageEffectGenerated.interval = this.interval;
      damageEffectGenerated.damageParameterName = this.damageParameterName;
      damageEffectGenerated.adsorbtionMaxParameterName = this.adsorbtionMaxParameterName;
      damageEffectGenerated.maxArmor = this.maxArmor;
      damageEffectGenerated.durabilityReduceByHit = this.durabilityReduceByHit;
      damageEffectGenerated.durabilityReduceKoeficient = this.durabilityReduceKoeficient;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Enable", this.enable);
      DefaultDataWriteUtility.WriteEnum<DurationTypeEnum>(writer, "DurationType", this.durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", this.realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", this.duration);
      DefaultDataWriteUtility.Write(writer, "Interval", this.interval);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "DamageParameterName", this.damageParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "AdsorbtionMaxParameterName", this.adsorbtionMaxParameterName);
      DefaultDataWriteUtility.Write(writer, "MaxArmor", this.maxArmor);
      DefaultDataWriteUtility.Write(writer, "DurabilityReduceByHit", this.durabilityReduceByHit);
      DefaultDataWriteUtility.Write(writer, "DurabilityReduceKoeficient", this.durabilityReduceKoeficient);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.enable = DefaultDataReadUtility.Read(reader, "Enable", this.enable);
      this.durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      this.realTime = DefaultDataReadUtility.Read(reader, "RealTime", this.realTime);
      this.duration = DefaultDataReadUtility.Read(reader, "Duration", this.duration);
      this.interval = DefaultDataReadUtility.Read(reader, "Interval", this.interval);
      this.damageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "DamageParameterName");
      this.adsorbtionMaxParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "AdsorbtionMaxParameterName");
      this.maxArmor = DefaultDataReadUtility.Read(reader, "MaxArmor", this.maxArmor);
      this.durabilityReduceByHit = DefaultDataReadUtility.Read(reader, "DurabilityReduceByHit", this.durabilityReduceByHit);
      this.durabilityReduceKoeficient = DefaultDataReadUtility.Read(reader, "DurabilityReduceKoeficient", this.durabilityReduceKoeficient);
    }
  }
}
