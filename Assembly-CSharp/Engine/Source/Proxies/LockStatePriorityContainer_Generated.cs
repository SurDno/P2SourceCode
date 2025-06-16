using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Gate;
using Engine.Source.Commons.Parameters;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<PriorityItem<LockState>>(((PriorityContainer<LockState>) target2).items, this.items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<PriorityItem<LockState>>(writer, "Items", this.items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.items = DefaultDataReadUtility.ReadListSerialize<PriorityItem<LockState>>(reader, "Items", this.items);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveListSerialize<PriorityItem<LockState>>(writer, "Items", this.items);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.items = DefaultStateLoadUtility.ReadListSerialize<PriorityItem<LockState>>(reader, "Items", this.items);
    }
  }
}
