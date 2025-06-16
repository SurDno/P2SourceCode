using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Otimizations;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo(((SequenceMemoryStrategy_Generated) target2).items, items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize(writer, "Items", items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      items = DefaultDataReadUtility.ReadListSerialize(reader, "Items", items);
    }
  }
}
