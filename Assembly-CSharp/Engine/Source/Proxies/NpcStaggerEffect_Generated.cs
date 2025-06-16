using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NpcStaggerEffect))]
  public class NpcStaggerEffect_Generated : 
    NpcStaggerEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NpcStaggerEffect_Generated instance = Activator.CreateInstance<NpcStaggerEffect_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      NpcStaggerEffect_Generated staggerEffectGenerated = (NpcStaggerEffect_Generated) target2;
      staggerEffectGenerated.name = name;
      staggerEffectGenerated.queue = queue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", name);
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      name = DefaultDataReadUtility.Read(reader, "Name", name);
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
    }
  }
}
