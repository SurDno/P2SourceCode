using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PlusFloatPolyOperation))]
  public class PlusFloatPolyOperation_Generated : 
    PlusFloatPolyOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlusFloatPolyOperation_Generated instance = Activator.CreateInstance<PlusFloatPolyOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<IValue<float>>(((PolyOperation<float>) target2).values, this.values);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<IValue<float>>(writer, "Parameters", this.values);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.values = DefaultDataReadUtility.ReadListSerialize<IValue<float>>(reader, "Parameters", this.values);
    }
  }
}
