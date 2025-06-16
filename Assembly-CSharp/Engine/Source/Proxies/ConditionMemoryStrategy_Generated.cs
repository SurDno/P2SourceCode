using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Otimizations;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ConditionMemoryStrategy))]
  public class ConditionMemoryStrategy_Generated : 
    ConditionMemoryStrategy,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ConditionMemoryStrategy_Generated instance = Activator.CreateInstance<ConditionMemoryStrategy_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ConditionMemoryStrategy_Generated strategyGenerated = (ConditionMemoryStrategy_Generated) target2;
      CloneableObjectUtility.FillListTo(strategyGenerated.contexts, contexts);
      strategyGenerated.item = CloneableObjectUtility.Clone(item);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListEnum(writer, "Contexts", contexts);
      DefaultDataWriteUtility.WriteSerialize(writer, "Item", item);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      contexts = DefaultDataReadUtility.ReadListEnum(reader, "Contexts", contexts);
      item = DefaultDataReadUtility.ReadSerialize<IMemoryStrategy>(reader, "Item");
    }
  }
}
