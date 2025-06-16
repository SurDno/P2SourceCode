using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Otimizations;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ConditionMemoryStrategy_Generated strategyGenerated = (ConditionMemoryStrategy_Generated) target2;
      CloneableObjectUtility.FillListTo<MemoryStrategyContextEnum>(strategyGenerated.contexts, this.contexts);
      strategyGenerated.item = CloneableObjectUtility.Clone<IMemoryStrategy>(this.item);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListEnum<MemoryStrategyContextEnum>(writer, "Contexts", this.contexts);
      DefaultDataWriteUtility.WriteSerialize<IMemoryStrategy>(writer, "Item", this.item);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.contexts = DefaultDataReadUtility.ReadListEnum<MemoryStrategyContextEnum>(reader, "Contexts", this.contexts);
      this.item = DefaultDataReadUtility.ReadSerialize<IMemoryStrategy>(reader, "Item");
    }
  }
}
