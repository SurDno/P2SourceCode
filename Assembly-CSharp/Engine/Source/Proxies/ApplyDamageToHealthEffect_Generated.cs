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
  [FactoryProxy(typeof (ApplyDamageToHealthEffect))]
  public class ApplyDamageToHealthEffect_Generated : 
    ApplyDamageToHealthEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ApplyDamageToHealthEffect_Generated instance = Activator.CreateInstance<ApplyDamageToHealthEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ApplyDamageToHealthEffect_Generated healthEffectGenerated = (ApplyDamageToHealthEffect_Generated) target2;
      healthEffectGenerated.queue = this.queue;
      healthEffectGenerated.enable = this.enable;
      healthEffectGenerated.healthParameterName = this.healthParameterName;
      healthEffectGenerated.isCombatIgnoredParameterName = this.isCombatIgnoredParameterName;
      healthEffectGenerated.immortalParameterName = this.immortalParameterName;
      healthEffectGenerated.ballisticDamageParameterName = this.ballisticDamageParameterName;
      healthEffectGenerated.fireDamageParameterName = this.fireDamageParameterName;
      healthEffectGenerated.meleeDamageParameterName = this.meleeDamageParameterName;
      healthEffectGenerated.fallDamageParameterName = this.fallDamageParameterName;
      healthEffectGenerated.fistsDamageParameterName = this.fistsDamageParameterName;
      healthEffectGenerated.durationType = this.durationType;
      healthEffectGenerated.realTime = this.realTime;
      healthEffectGenerated.duration = this.duration;
      healthEffectGenerated.interval = this.interval;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Enable", this.enable);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "HealthParameterName", this.healthParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "IsCombatIgnoredParameterName", this.isCombatIgnoredParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "ImmortalParameterName", this.immortalParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "BallisticDamageParameterName", this.ballisticDamageParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "FireDamageParameterName", this.fireDamageParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "MeleeDamageParameterName", this.meleeDamageParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "FallDamageParameterName", this.fallDamageParameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "FistsDamageParameterName", this.fistsDamageParameterName);
      DefaultDataWriteUtility.WriteEnum<DurationTypeEnum>(writer, "DurationType", this.durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", this.realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", this.duration);
      DefaultDataWriteUtility.Write(writer, "Interval", this.interval);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.enable = DefaultDataReadUtility.Read(reader, "Enable", this.enable);
      this.healthParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "HealthParameterName");
      this.isCombatIgnoredParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "IsCombatIgnoredParameterName");
      this.immortalParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ImmortalParameterName");
      this.ballisticDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "BallisticDamageParameterName");
      this.fireDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "FireDamageParameterName");
      this.meleeDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "MeleeDamageParameterName");
      this.fallDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "FallDamageParameterName");
      this.fistsDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "FistsDamageParameterName");
      this.durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      this.realTime = DefaultDataReadUtility.Read(reader, "RealTime", this.realTime);
      this.duration = DefaultDataReadUtility.Read(reader, "Duration", this.duration);
      this.interval = DefaultDataReadUtility.Read(reader, "Interval", this.interval);
    }
  }
}
