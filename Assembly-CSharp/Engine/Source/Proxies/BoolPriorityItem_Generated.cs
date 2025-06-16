using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Parameters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PriorityItem<bool>))]
  public class BoolPriorityItem_Generated : 
    BoolPriorityItem,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      BoolPriorityItem_Generated instance = Activator.CreateInstance<BoolPriorityItem_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      BoolPriorityItem_Generated priorityItemGenerated = (BoolPriorityItem_Generated) target2;
      priorityItemGenerated.Priority = Priority;
      priorityItemGenerated.Value = Value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Priority", Priority);
      DefaultDataWriteUtility.Write(writer, "Value", Value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
      Value = DefaultDataReadUtility.Read(reader, "Value", Value);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Priority", Priority);
      DefaultDataWriteUtility.Write(writer, "Value", Value);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
      Value = DefaultDataReadUtility.Read(reader, "Value", Value);
    }
  }
}
