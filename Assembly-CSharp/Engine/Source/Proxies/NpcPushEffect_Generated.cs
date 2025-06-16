using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NpcPushEffect))]
  public class NpcPushEffect_Generated : 
    NpcPushEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NpcPushEffect_Generated instance = Activator.CreateInstance<NpcPushEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      NpcPushEffect_Generated pushEffectGenerated = (NpcPushEffect_Generated) target2;
      pushEffectGenerated.name = this.name;
      pushEffectGenerated.queue = this.queue;
      pushEffectGenerated.self = this.self;
      pushEffectGenerated.velocity = this.velocity;
      pushEffectGenerated.time = this.time;
      pushEffectGenerated.npcPushScale = this.npcPushScale;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Self", this.self);
      DefaultDataWriteUtility.Write(writer, "Velocity", this.velocity);
      DefaultDataWriteUtility.Write(writer, "Time", this.time);
      DefaultDataWriteUtility.Write(writer, "NpcPushScale", this.npcPushScale);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.Read(reader, "Name", this.name);
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.self = DefaultDataReadUtility.Read(reader, "Self", this.self);
      this.velocity = DefaultDataReadUtility.Read(reader, "Velocity", this.velocity);
      this.time = DefaultDataReadUtility.Read(reader, "Time", this.time);
      this.npcPushScale = DefaultDataReadUtility.Read(reader, "NpcPushScale", this.npcPushScale);
    }
  }
}
