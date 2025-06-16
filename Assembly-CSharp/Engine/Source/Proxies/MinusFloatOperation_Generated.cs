using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MinusFloatOperation))]
  public class MinusFloatOperation_Generated : 
    MinusFloatOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      MinusFloatOperation_Generated instance = Activator.CreateInstance<MinusFloatOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      MinusFloatOperation_Generated operationGenerated = (MinusFloatOperation_Generated) target2;
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
