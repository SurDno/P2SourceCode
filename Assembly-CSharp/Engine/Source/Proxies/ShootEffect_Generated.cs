using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Engine.Source.Effects.Values;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ShootEffect))]
  public class ShootEffect_Generated : 
    ShootEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ShootEffect_Generated instance = Activator.CreateInstance<ShootEffect_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ShootEffect_Generated shootEffectGenerated = (ShootEffect_Generated) target2;
      shootEffectGenerated.queue = queue;
      shootEffectGenerated.actionType = actionType;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.WriteEnum(writer, "Action", actionType);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      actionType = DefaultDataReadUtility.ReadEnum<ShootEffectEnum>(reader, "Action");
    }
  }
}
