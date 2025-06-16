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
  [FactoryProxy(typeof (NpcReduceDamageByArmorEffect))]
  public class NpcReduceDamageByArmorEffect_Generated : 
    NpcReduceDamageByArmorEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NpcReduceDamageByArmorEffect_Generated instance = Activator.CreateInstance<NpcReduceDamageByArmorEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      NpcReduceDamageByArmorEffect_Generated armorEffectGenerated = (NpcReduceDamageByArmorEffect_Generated) target2;
      armorEffectGenerated.queue = this.queue;
      armorEffectGenerated.enable = this.enable;
      armorEffectGenerated.durationType = this.durationType;
      armorEffectGenerated.realTime = this.realTime;
      armorEffectGenerated.duration = this.duration;
      armorEffectGenerated.interval = this.interval;
      armorEffectGenerated.damageParameterName = this.damageParameterName;
      armorEffectGenerated.armorParameterName = this.armorParameterName;
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
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "ArmorParameterName", this.armorParameterName);
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
      this.armorParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ArmorParameterName");
    }
  }
}
