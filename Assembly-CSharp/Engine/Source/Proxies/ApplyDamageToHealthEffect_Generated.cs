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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ApplyDamageToHealthEffect_Generated healthEffectGenerated = (ApplyDamageToHealthEffect_Generated) target2;
      healthEffectGenerated.queue = queue;
      healthEffectGenerated.enable = enable;
      healthEffectGenerated.healthParameterName = healthParameterName;
      healthEffectGenerated.isCombatIgnoredParameterName = isCombatIgnoredParameterName;
      healthEffectGenerated.immortalParameterName = immortalParameterName;
      healthEffectGenerated.ballisticDamageParameterName = ballisticDamageParameterName;
      healthEffectGenerated.fireDamageParameterName = fireDamageParameterName;
      healthEffectGenerated.meleeDamageParameterName = meleeDamageParameterName;
      healthEffectGenerated.fallDamageParameterName = fallDamageParameterName;
      healthEffectGenerated.fistsDamageParameterName = fistsDamageParameterName;
      healthEffectGenerated.durationType = durationType;
      healthEffectGenerated.realTime = realTime;
      healthEffectGenerated.duration = duration;
      healthEffectGenerated.interval = interval;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.Write(writer, "Enable", enable);
      DefaultDataWriteUtility.WriteEnum(writer, "HealthParameterName", healthParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "IsCombatIgnoredParameterName", isCombatIgnoredParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "ImmortalParameterName", immortalParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "BallisticDamageParameterName", ballisticDamageParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "FireDamageParameterName", fireDamageParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "MeleeDamageParameterName", meleeDamageParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "FallDamageParameterName", fallDamageParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "FistsDamageParameterName", fistsDamageParameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "DurationType", durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", duration);
      DefaultDataWriteUtility.Write(writer, "Interval", interval);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      enable = DefaultDataReadUtility.Read(reader, "Enable", enable);
      healthParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "HealthParameterName");
      isCombatIgnoredParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "IsCombatIgnoredParameterName");
      immortalParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ImmortalParameterName");
      ballisticDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "BallisticDamageParameterName");
      fireDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "FireDamageParameterName");
      meleeDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "MeleeDamageParameterName");
      fallDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "FallDamageParameterName");
      fistsDamageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "FistsDamageParameterName");
      durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
      duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
      interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
    }
  }
}
