using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (GreaterOrEqualFloatOperation))]
  public class GreaterOrEqualFloatOperation_Generated : 
    GreaterOrEqualFloatOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      GreaterOrEqualFloatOperation_Generated instance = Activator.CreateInstance<GreaterOrEqualFloatOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      GreaterOrEqualFloatOperation_Generated operationGenerated = (GreaterOrEqualFloatOperation_Generated) target2;
      operationGenerated.a = CloneableObjectUtility.Clone<IValue<float>>(this.a);
      operationGenerated.b = CloneableObjectUtility.Clone<IValue<float>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<float>>(writer, "Left", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<float>>(writer, "Right", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "Left");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "Right");
    }
  }
}
