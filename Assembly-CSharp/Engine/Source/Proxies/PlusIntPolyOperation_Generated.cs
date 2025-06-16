using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PlusIntPolyOperation))]
  public class PlusIntPolyOperation_Generated : 
    PlusIntPolyOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlusIntPolyOperation_Generated instance = Activator.CreateInstance<PlusIntPolyOperation_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo(((PolyOperation<int>) target2).values, values);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize(writer, "Parameters", values);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      values = DefaultDataReadUtility.ReadListSerialize(reader, "Parameters", values);
    }
  }
}
