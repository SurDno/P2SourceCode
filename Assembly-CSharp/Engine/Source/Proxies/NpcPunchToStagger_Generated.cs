using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NpcPunchToStagger))]
  public class NpcPunchToStagger_Generated : 
    NpcPunchToStagger,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NpcPunchToStagger_Generated instance = Activator.CreateInstance<NpcPunchToStagger_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      NpcPunchToStagger_Generated staggerGenerated = (NpcPunchToStagger_Generated) target2;
      staggerGenerated.name = this.name;
      staggerGenerated.punchEnum = this.punchEnum;
      staggerGenerated.queue = this.queue;
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
