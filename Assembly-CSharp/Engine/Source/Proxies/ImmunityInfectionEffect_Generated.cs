// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ImmunityInfectionEffect_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ImmunityInfectionEffect))]
  public class ImmunityInfectionEffect_Generated : 
    ImmunityInfectionEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ImmunityInfectionEffect_Generated instance = Activator.CreateInstance<ImmunityInfectionEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ImmunityInfectionEffect_Generated infectionEffectGenerated = (ImmunityInfectionEffect_Generated) target2;
      infectionEffectGenerated.queue = this.queue;
      infectionEffectGenerated.enable = this.enable;
      infectionEffectGenerated.durationType = this.durationType;
      infectionEffectGenerated.realTime = this.realTime;
      infectionEffectGenerated.duration = this.duration;
      infectionEffectGenerated.interval = this.interval;
      infectionEffectGenerated.infectionDamageParameterName = this.infectionDamageParameterName;
      infectionEffectGenerated.immunityParameterName = this.immunityParameterName;
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
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "ImmunityParameterName", this.immunityParameterName);
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
      this.immunityParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ImmunityParameterName");
    }
  }
}
