using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Parameters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PriorityItem<LockState>))]
  public class LockStatePriorityItem_Generated : 
    LockStatePriorityItem,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      LockStatePriorityItem_Generated instance = Activator.CreateInstance<LockStatePriorityItem_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      LockStatePriorityItem_Generated priorityItemGenerated = (LockStatePriorityItem_Generated) target2;
      priorityItemGenerated.Priority = this.Priority;
      priorityItemGenerated.Value = this.Value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<PriorityParameterEnum>(writer, "Priority", this.Priority);
      DefaultDataWriteUtility.WriteEnum<LockState>(writer, "Value", this.Value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
      this.Value = DefaultDataReadUtility.ReadEnum<LockState>(reader, "Value");
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<PriorityParameterEnum>(writer, "Priority", this.Priority);
      DefaultDataWriteUtility.WriteEnum<LockState>(writer, "Value", this.Value);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
      this.Value = DefaultDataReadUtility.ReadEnum<LockState>(reader, "Value");
    }
  }
}
