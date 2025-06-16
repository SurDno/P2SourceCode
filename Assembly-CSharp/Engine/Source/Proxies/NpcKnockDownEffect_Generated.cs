using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NpcKnockDownEffect))]
  public class NpcKnockDownEffect_Generated : 
    NpcKnockDownEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NpcKnockDownEffect_Generated instance = Activator.CreateInstance<NpcKnockDownEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      NpcKnockDownEffect_Generated downEffectGenerated = (NpcKnockDownEffect_Generated) target2;
      downEffectGenerated.name = this.name;
      downEffectGenerated.queue = this.queue;
      downEffectGenerated.holdTime = this.holdTime;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "HoldTime", this.holdTime);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.Read(reader, "Name", this.name);
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.holdTime = DefaultDataReadUtility.Read(reader, "HoldTime", this.holdTime);
    }
  }
}
