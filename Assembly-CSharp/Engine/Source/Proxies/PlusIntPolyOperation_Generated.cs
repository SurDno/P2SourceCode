using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<IValue<int>>(((PolyOperation<int>) target2).values, this.values);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<IValue<int>>(writer, "Parameters", this.values);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.values = DefaultDataReadUtility.ReadListSerialize<IValue<int>>(reader, "Parameters", this.values);
    }
  }
}
