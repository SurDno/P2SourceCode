using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (GreaterOrEqualTimeSpanOperation))]
  public class GreaterOrEqualTimeSpanOperation_Generated : 
    GreaterOrEqualTimeSpanOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      GreaterOrEqualTimeSpanOperation_Generated instance = Activator.CreateInstance<GreaterOrEqualTimeSpanOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      GreaterOrEqualTimeSpanOperation_Generated operationGenerated = (GreaterOrEqualTimeSpanOperation_Generated) target2;
      operationGenerated.a = CloneableObjectUtility.Clone<IValue<TimeSpan>>(this.a);
      operationGenerated.b = CloneableObjectUtility.Clone<IValue<TimeSpan>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<TimeSpan>>(writer, "Left", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<TimeSpan>>(writer, "Right", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValue<TimeSpan>>(reader, "Left");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<TimeSpan>>(reader, "Right");
    }
  }
}
