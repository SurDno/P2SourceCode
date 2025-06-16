using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Parameters;
using Engine.Source.Connections;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PriorityContainer<List<Typed<IEntity>>>))]
  public class ListTypedEntityPriorityContainer_Generated : 
    ListTypedEntityPriorityContainer,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      ListTypedEntityPriorityContainer_Generated instance = Activator.CreateInstance<ListTypedEntityPriorityContainer_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo(((PriorityContainer<List<Typed<IEntity>>>) target2).items, items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize(writer, "Items", items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      items = DefaultDataReadUtility.ReadListSerialize(reader, "Items", items);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveListSerialize(writer, "Items", items);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      items = DefaultStateLoadUtility.ReadListSerialize(reader, "Items", items);
    }
  }
}
