using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DisableEntityEffect))]
  public class DisableEntityEffect_Generated : 
    DisableEntityEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      DisableEntityEffect_Generated instance = Activator.CreateInstance<DisableEntityEffect_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((DisableEntityEffect_Generated) target2).queue = queue;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
    }
  }
}
