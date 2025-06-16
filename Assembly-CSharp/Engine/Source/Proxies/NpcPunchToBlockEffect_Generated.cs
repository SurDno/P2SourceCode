using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NpcPunchToBlockEffect))]
  public class NpcPunchToBlockEffect_Generated : 
    NpcPunchToBlockEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NpcPunchToBlockEffect_Generated instance = Activator.CreateInstance<NpcPunchToBlockEffect_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      NpcPunchToBlockEffect_Generated blockEffectGenerated = (NpcPunchToBlockEffect_Generated) target2;
      blockEffectGenerated.name = name;
      blockEffectGenerated.punchEnum = punchEnum;
      blockEffectGenerated.queue = queue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", name);
      DefaultDataWriteUtility.WriteEnum(writer, "punchType", punchEnum);
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      name = DefaultDataReadUtility.Read(reader, "Name", name);
      punchEnum = DefaultDataReadUtility.ReadEnum<PunchTypeEnum>(reader, "punchType");
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
    }
  }
}
