using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Gate;
using Engine.Source.Commons.Parameters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PriorityContainer<LockState>))]
  public class LockStatePriorityContainer_Generated : 
    LockStatePriorityContainer,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      LockStatePriorityContainer_Generated instance = Activator.CreateInstance<LockStatePriorityContainer_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo(((LockStatePriorityContainer_Generated) target2).items, items);
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
