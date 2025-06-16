using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NpcPrepunchEffect))]
  public class NpcPrepunchEffect_Generated : 
    NpcPrepunchEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NpcPrepunchEffect_Generated instance = Activator.CreateInstance<NpcPrepunchEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      NpcPrepunchEffect_Generated prepunchEffectGenerated = (NpcPrepunchEffect_Generated) target2;
      prepunchEffectGenerated.name = this.name;
      prepunchEffectGenerated.queue = this.queue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.Read(reader, "Name", this.name);
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
    }
  }
}
