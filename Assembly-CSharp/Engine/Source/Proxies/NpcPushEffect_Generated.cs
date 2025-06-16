using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      NpcPushEffect_Generated pushEffectGenerated = (NpcPushEffect_Generated) target2;
      pushEffectGenerated.name = name;
      pushEffectGenerated.queue = queue;
      pushEffectGenerated.self = self;
      pushEffectGenerated.velocity = velocity;
      pushEffectGenerated.time = time;
      pushEffectGenerated.npcPushScale = npcPushScale;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", name);
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.Write(writer, "Self", self);
      DefaultDataWriteUtility.Write(writer, "Velocity", velocity);
      DefaultDataWriteUtility.Write(writer, "Time", time);
      DefaultDataWriteUtility.Write(writer, "NpcPushScale", npcPushScale);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      name = DefaultDataReadUtility.Read(reader, "Name", name);
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      self = DefaultDataReadUtility.Read(reader, "Self", self);
      velocity = DefaultDataReadUtility.Read(reader, "Velocity", velocity);
      time = DefaultDataReadUtility.Read(reader, "Time", time);
      npcPushScale = DefaultDataReadUtility.Read(reader, "NpcPushScale", npcPushScale);
    }
  }
}
