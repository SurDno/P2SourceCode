using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Otimizations;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SequenceMemoryStrategy))]
  public class SequenceMemoryStrategy_Generated : 
    SequenceMemoryStrategy,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      SequenceMemoryStrategy_Generated instance = Activator.CreateInstance<SequenceMemoryStrategy_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<IMemoryStrategy>(((SequenceMemoryStrategy) target2).items, this.items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<IMemoryStrategy>(writer, "Items", this.items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.items = DefaultDataReadUtility.ReadListSerialize<IMemoryStrategy>(reader, "Items", this.items);
    }
  }
}
