using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PlayerActionEffect))]
  public class PlayerActionEffect_Generated : 
    PlayerActionEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlayerActionEffect_Generated instance = Activator.CreateInstance<PlayerActionEffect_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      PlayerActionEffect_Generated actionEffectGenerated = (PlayerActionEffect_Generated) target2;
      actionEffectGenerated.queue = queue;
      actionEffectGenerated.action = action;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.WriteEnum(writer, "Action", action);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      action = DefaultDataReadUtility.ReadEnum<ActionEnum>(reader, "Action");
    }
  }
}
