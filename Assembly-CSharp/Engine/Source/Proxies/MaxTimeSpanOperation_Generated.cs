using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MaxTimeSpanOperation))]
  public class MaxTimeSpanOperation_Generated : 
    MaxTimeSpanOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      MaxTimeSpanOperation_Generated instance = Activator.CreateInstance<MaxTimeSpanOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      MaxTimeSpanOperation_Generated operationGenerated = (MaxTimeSpanOperation_Generated) target2;
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
