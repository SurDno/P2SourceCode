using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Engine.Source.Effects.Values;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ShootEffect_Generated shootEffectGenerated = (ShootEffect_Generated) target2;
      shootEffectGenerated.queue = this.queue;
      shootEffectGenerated.actionType = this.actionType;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.WriteEnum<ShootEffectEnum>(writer, "Action", this.actionType);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.actionType = DefaultDataReadUtility.ReadEnum<ShootEffectEnum>(reader, "Action");
    }
  }
}
