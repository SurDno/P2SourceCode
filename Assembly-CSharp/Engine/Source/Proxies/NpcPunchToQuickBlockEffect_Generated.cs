using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NpcPunchToQuickBlockEffect))]
  public class NpcPunchToQuickBlockEffect_Generated : 
    NpcPunchToQuickBlockEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NpcPunchToQuickBlockEffect_Generated instance = Activator.CreateInstance<NpcPunchToQuickBlockEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      NpcPunchToQuickBlockEffect_Generated blockEffectGenerated = (NpcPunchToQuickBlockEffect_Generated) target2;
      blockEffectGenerated.name = this.name;
      blockEffectGenerated.punchEnum = this.punchEnum;
      blockEffectGenerated.queue = this.queue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<PunchTypeEnum>(writer, "punchType", this.punchEnum);
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.Read(reader, "Name", this.name);
      this.punchEnum = DefaultDataReadUtility.ReadEnum<PunchTypeEnum>(reader, "punchType");
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
    }
  }
}
