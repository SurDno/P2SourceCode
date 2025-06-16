using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Parameters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PriorityContainer<bool>))]
  public class BoolPriorityContainer_Generated : 
    BoolPriorityContainer,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      BoolPriorityContainer_Generated instance = Activator.CreateInstance<BoolPriorityContainer_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<PriorityItem<bool>>(((PriorityContainer<bool>) target2).items, this.items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<PriorityItem<bool>>(writer, "Items", this.items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.items = DefaultDataReadUtility.ReadListSerialize<PriorityItem<bool>>(reader, "Items", this.items);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveListSerialize<PriorityItem<bool>>(writer, "Items", this.items);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.items = DefaultStateLoadUtility.ReadListSerialize<PriorityItem<bool>>(reader, "Items", this.items);
    }
  }
}
