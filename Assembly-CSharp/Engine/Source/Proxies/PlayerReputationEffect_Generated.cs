using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PlayerReputationEffect))]
  public class PlayerReputationEffect_Generated : 
    PlayerReputationEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlayerReputationEffect_Generated instance = Activator.CreateInstance<PlayerReputationEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((PlayerReputationEffect) target2).queue = this.queue;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
    }
  }
}
