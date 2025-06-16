using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MaxFloatOperation))]
  public class MaxFloatOperation_Generated : 
    MaxFloatOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      MaxFloatOperation_Generated instance = Activator.CreateInstance<MaxFloatOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      MaxFloatOperation_Generated operationGenerated = (MaxFloatOperation_Generated) target2;
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
