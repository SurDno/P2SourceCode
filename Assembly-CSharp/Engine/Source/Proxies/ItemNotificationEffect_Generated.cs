using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ItemNotificationEffect))]
  public class ItemNotificationEffect_Generated : 
    ItemNotificationEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ItemNotificationEffect_Generated instance = Activator.CreateInstance<ItemNotificationEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((ItemNotificationEffect) target2).queue = this.queue;

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
